using SayehBanTools.Framework.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SayehBanTools.Framework.GenerateValue
{
    /// <summary>
    /// کلاس تولید و بررسی کد ملی ۱۰ رقمی (الگوریتم ثبت احوال) و کد ملی ۲۰ رقمی
    /// </summary>
    /// <remarks>
    /// <para><b>کد ملی ۱۰ رقمی:</b> [LLL][SSSSSS][K]</para>
    /// <para>LLL = کد محل صدور (۳ رقم) / SSSSSS = سریال (۶ رقم) / K = رقم کنترل</para>
    /// <para>رقم کنترل: مجموع رقم‌های ۱ تا ۹ ضرب در وزن‌های ۱۰ تا ۲؛ r = مجموع % 11 ؛ اگر r کمتر از ۲ بود K = r وگرنه K = 11 - r</para>
    ///
    /// <para><b>کد ملی ۲۰ رقمی:</b> [CCC][LLL][SSSSSSS][EEEEE][K1][K2]</para>
    /// <para>CCC = کد کشور (ISO 3166-1 عددی، ایران = 364) برای پیوند کشورهای دیگر در آینده</para>
    /// <para>LLL = کد محل صدور (همان سه رقم اول کد ۱۰ رقمی)</para>
    /// <para>SSSSSSS = سریال ۷ رقمی / EEEEE = پسوند ۵ رقمی</para>
    /// <para>K1 = رقم کنترل اول با همان قاعده ثبت احوال روی رقم‌های ۱ تا ۱۸ با وزن‌های چرخشی ۱۰..۲</para>
    /// <para>K2 = رقم کنترل دوم با همان قاعده روی رقم‌های ۱ تا ۱۹ با وزن‌های چرخشی ۱..۱۰</para>
    /// <para>
    /// کد تبدیل‌شده از کد ۱۰ رقمی: LLL+SSSSSSS دقیقاً همان کد ۱۰ رقمی قبلی است و EEEEE = 00000؛
    /// مثلاً 0014401509 ← 364 0014401509 00000 K1K2 (برگشت‌پذیر).
    /// کدهای جدید همیشه پسوند غیرصفر دارند، پس هیچ‌وقت با کدهای تبدیل‌شده تداخل ندارند.
    /// </para>
    /// </remarks>
    public class NationalCode
    {
        /// <summary>کد کشور ایران (ISO 3166-1 عددی)</summary>
        public const string IranCountryCode = "364";

        /// <summary>پسوند کدهای ۲۰ رقمی که از کد ۱۰ رقمی تبدیل شده‌اند</summary>
        public const string LegacyExtension = "00000";

        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

        #region ---------- کد ملی ۱۰ رقمی ----------

        /// <summary>
        /// تولید کد ملی تصادفی معتبر
        /// </summary>
        /// <returns>کد ملی 10 رقمی معتبر</returns>
        public static string GenerateRandom()
        {
            return GenerateRandom(null);
        }

        /// <summary>
        /// تولید کد ملی تصادفی معتبر برای یک محل صدور مشخص
        /// </summary>
        /// <param name="locationCode">کد محل صدور ۳ رقمی (مثلاً 001)؛ null یعنی محل تصادفی</param>
        /// <returns>کد ملی 10 رقمی معتبر</returns>
        public static string GenerateRandom(string locationCode)
        {
            string location = ResolveLocation(locationCode, round: false);
            while (true)
            {
                string code = Complete10(location + RandomDigits(6));
                if (!IsAllSameDigit(code))
                    return code;
            }
        }

        /// <summary>
        /// تولید کد ملی رند و معتبر
        /// </summary>
        /// <returns>کد ملی 10 رقمی رند و معتبر</returns>
        public static string GenerateRound()
        {
            return GenerateRound(null);
        }

        /// <summary>
        /// تولید کد ملی رند و معتبر برای یک محل صدور مشخص
        /// </summary>
        /// <param name="locationCode">کد محل صدور ۳ رقمی؛ null یعنی محل رند تصادفی</param>
        /// <returns>کد ملی 10 رقمی رند و معتبر</returns>
        public static string GenerateRound(string locationCode)
        {
            string location = ResolveLocation(locationCode, round: true);
            while (true)
            {
                string code = Complete10(location + RoundDigits(6));
                if (!IsAllSameDigit(code))
                    return code;
            }
        }

        /// <summary>
        /// محاسبه رقم کنترل کد ملی ۱۰ رقمی
        /// </summary>
        /// <param name="first9Digits">9 رقم اول کد ملی</param>
        /// <returns>رقم کنترل (0 تا 9)</returns>
        public static int CalculateControlDigit(string first9Digits)
        {
            string digits = NormalizeInput(first9Digits);
            if (digits.Length != 9 || !IsAsciiDigits(digits))
                throw new ArgumentException("برای محاسبه رقم کنترل دقیقاً ۹ رقم لازم است", nameof(first9Digits));

            int sum = 0;
            for (int i = 0; i < 9; i++)
                sum += (digits[i] - '0') * (10 - i);

            return Mod11Check(sum);
        }

        /// <summary>
        /// بررسی صحت کد ملی ۱۰ رقمی (ارقام فارسی/عربی، فاصله و خط‌تیره هم پذیرفته می‌شود)
        /// </summary>
        /// <param name="nationalCode">کد ملی</param>
        /// <returns>درست یا نادرست</returns>
        /// <exception cref="Exception">در صورت ورودی نامعتبر</exception>
        public static bool IsValidNationalCode(string nationalCode)
        {
            if (string.IsNullOrWhiteSpace(nationalCode))
                throw new Exception("لطفا کد ملی را صحیح وارد نمایید");

            string code = NormalizeInput(nationalCode);

            if (code.Length != 10)
                throw new Exception("طول کد ملی باید ده کاراکتر باشد");

            if (!IsAsciiDigits(code))
                throw new Exception("کد ملی تشکیل شده از ده رقم عددی می‌باشد؛ لطفا کد ملی را صحیح وارد نمایید");

            return IsValid10Core(code);
        }

        #endregion

        #region ---------- کد ملی ۲۰ رقمی ----------

        /// <summary>
        /// تولید کد ملی ۲۰ رقمی تصادفی معتبر
        /// </summary>
        /// <param name="countryCode">کد کشور ۳ رقمی (پیش‌فرض ایران 364)</param>
        /// <param name="locationCode">کد محل صدور ۳ رقمی؛ null یعنی محل تصادفی</param>
        /// <returns>کد ملی ۲۰ رقمی معتبر</returns>
        public static string GenerateRandom20(string countryCode = IranCountryCode, string locationCode = null)
        {
            string country = ResolveCountry(countryCode);
            string location = ResolveLocation(locationCode, round: false);
            while (true)
            {
                string body = country + location + RandomDigits(12);
                if (body.Substring(13, 5) == LegacyExtension)
                    continue; // پسوند 00000 مخصوص کدهای تبدیل‌شده است

                string code = Complete20(body);
                if (!IsAllSameDigit(code))
                    return code;
            }
        }

        /// <summary>
        /// تولید کد ملی ۲۰ رقمی رند و معتبر
        /// </summary>
        /// <param name="countryCode">کد کشور ۳ رقمی (پیش‌فرض ایران 364)</param>
        /// <param name="locationCode">کد محل صدور ۳ رقمی؛ null یعنی محل رند تصادفی</param>
        /// <returns>کد ملی ۲۰ رقمی رند و معتبر</returns>
        public static string GenerateRound20(string countryCode = IranCountryCode, string locationCode = null)
        {
            string country = ResolveCountry(countryCode);
            string location = ResolveLocation(locationCode, round: true);
            while (true)
            {
                string body = country + location + RoundDigits(12);
                if (body.Substring(13, 5) == LegacyExtension)
                    continue;

                string code = Complete20(body);
                if (!IsAllSameDigit(code))
                    return code;
            }
        }

        /// <summary>
        /// تبدیل کد ملی ۱۰ رقمی به کد ملی ۲۰ رقمی نزدیک به آن (برگشت‌پذیر، هر کد ۱۰ رقمی فقط یک کد ۲۰ رقمی دارد).
        /// مثال: 0014401509 ← 364 0014401509 00000 + دو رقم کنترل
        /// </summary>
        /// <param name="nationalCode10">کد ملی ۱۰ رقمی معتبر</param>
        /// <param name="countryCode">کد کشور ۳ رقمی (پیش‌فرض ایران 364)</param>
        /// <returns>کد ملی ۲۰ رقمی</returns>
        public static string ConvertTo20(string nationalCode10, string countryCode = IranCountryCode)
        {
            if (!IsValidNationalCode(nationalCode10))
                throw new ArgumentException("کد ملی ۱۰ رقمی معتبر نیست", nameof(nationalCode10));

            return Complete20(ResolveCountry(countryCode) + NormalizeInput(nationalCode10) + LegacyExtension);
        }

        /// <summary>
        /// تولید کد ملی ۲۰ رقمی جدید در همان محل صدورِ کد ملی ۱۰ رقمی (با سریال تصادفی و پسوند غیرصفر)
        /// </summary>
        /// <param name="nationalCode10">کد ملی ۱۰ رقمی معتبر</param>
        /// <param name="round">true برای کد رند</param>
        /// <param name="countryCode">کد کشور ۳ رقمی (پیش‌فرض ایران 364)</param>
        public static string GenerateNear20(string nationalCode10, bool round = false, string countryCode = IranCountryCode)
        {
            if (!IsValidNationalCode(nationalCode10))
                throw new ArgumentException("کد ملی ۱۰ رقمی معتبر نیست", nameof(nationalCode10));

            string location = NormalizeInput(nationalCode10).Substring(0, 3);
            return round ? GenerateRound20(countryCode, location) : GenerateRandom20(countryCode, location);
        }

        /// <summary>
        /// برگرداندن کد ملی ۱۰ رقمی از کد ۲۰ رقمیِ تبدیل‌شده؛ اگر کد ۲۰ رقمی از کد ۱۰ رقمی ساخته نشده باشد null
        /// </summary>
        public static string ConvertTo10(string nationalCode20)
        {
            string code = NormalizeInput(nationalCode20);
            if (!IsValid20Core(code) || !IsLegacy20Core(code))
                return null;

            return code.Substring(3, 10);
        }

        /// <summary>
        /// آیا کد ۲۰ رقمی از روی یک کد ملی ۱۰ رقمی ساخته شده است
        /// </summary>
        public static bool IsConvertedFrom10(string nationalCode20)
        {
            string code = NormalizeInput(nationalCode20);
            return IsValid20Core(code) && IsLegacy20Core(code);
        }

        /// <summary>
        /// محاسبه دو رقم کنترل کد ملی ۲۰ رقمی
        /// </summary>
        /// <param name="first18Digits">۱۸ رقم اول</param>
        /// <returns>دو رقم کنترل به صورت رشته</returns>
        public static string CalculateControlDigits20(string first18Digits)
        {
            string digits = NormalizeInput(first18Digits);
            if (digits.Length != 18 || !IsAsciiDigits(digits))
                throw new ArgumentException("برای محاسبه رقم کنترل دقیقاً ۱۸ رقم لازم است", nameof(first18Digits));

            // K1: وزن‌های ۱۰..۲ تکرار شونده (همان الگوریتم ثبت احوال روی دو بلوک ۹ رقمی)
            int sum1 = 0;
            for (int i = 0; i < 18; i++)
                sum1 += (digits[i] - '0') * (10 - (i % 9));
            int k1 = Mod11Check(sum1);

            // K2: وزن‌های ۱..۱۰ تکرار شونده روی ۱۹ رقم (شامل K1)؛
            // ترکیب دو وزن (دوره ۹ و ۱۰) برای هر جایگاه یکتاست، پس جابه‌جایی هر دو رقم شناسایی می‌شود
            int sum2 = 0;
            for (int i = 0; i < 18; i++)
                sum2 += (digits[i] - '0') * ((i % 10) + 1);
            sum2 += k1 * ((18 % 10) + 1);
            int k2 = Mod11Check(sum2);

            return k1.ToString() + k2.ToString();
        }

        /// <summary>
        /// بررسی صحت کد ملی ۲۰ رقمی (ارقام فارسی/عربی، فاصله و خط‌تیره هم پذیرفته می‌شود)
        /// </summary>
        /// <exception cref="Exception">در صورت ورودی نامعتبر</exception>
        public static bool IsValidNationalCode20(string nationalCode)
        {
            if (string.IsNullOrWhiteSpace(nationalCode))
                throw new Exception("لطفا کد ملی را صحیح وارد نمایید");

            string code = NormalizeInput(nationalCode);

            if (code.Length != 20)
                throw new Exception("طول کد ملی ۲۰ رقمی باید بیست کاراکتر باشد");

            if (!IsAsciiDigits(code))
                throw new Exception("کد ملی تشکیل شده از بیست رقم عددی می‌باشد؛ لطفا کد ملی را صحیح وارد نمایید");

            return IsValid20Core(code);
        }

        #endregion

        #region ---------- عمومی ----------

        /// <summary>
        /// بررسی صحت کد ملی ۱۰ یا ۲۰ رقمی بدون پرتاب استثنا
        /// </summary>
        public static bool IsValid(string nationalCode)
        {
            string code = NormalizeInput(nationalCode);
            if (!IsAsciiDigits(code))
                return false;

            switch (code.Length)
            {
                case 10: return IsValid10Core(code);
                case 20: return IsValid20Core(code);
                default: return false;
            }
        }

        /// <summary>
        /// کد محل صدور (۳ رقم) از کد ملی ۱۰ یا ۲۰ رقمی؛ برای کد نامعتبر null
        /// </summary>
        public static string GetLocationCode(string nationalCode)
        {
            string code = NormalizeInput(nationalCode);
            if (!IsValid(code))
                return null;
            return code.Length == 10 ? code.Substring(0, 3) : code.Substring(3, 3);
        }

        /// <summary>
        /// کد کشور از کد ملی ۲۰ رقمی (برای کد ۱۰ رقمی معتبر، کد ایران)؛ برای کد نامعتبر null
        /// </summary>
        public static string GetCountryCode(string nationalCode)
        {
            string code = NormalizeInput(nationalCode);
            if (!IsValid(code))
                return null;
            return code.Length == 10 ? IranCountryCode : code.Substring(0, 3);
        }

        /// <summary>
        /// قالب‌بندی برای نمایش: 001-440150-9 یا 364-001-4401509-00000-82
        /// </summary>
        public static string Format(string nationalCode)
        {
            string code = NormalizeInput(nationalCode);
            if (!IsAsciiDigits(code))
                return nationalCode;

            if (code.Length == 10)
                return $"{code.Substring(0, 3)}-{code.Substring(3, 6)}-{code.Substring(9, 1)}";
            if (code.Length == 20)
                return $"{code.Substring(0, 3)}-{code.Substring(3, 3)}-{code.Substring(6, 7)}-{code.Substring(13, 5)}-{code.Substring(18, 2)}";
            return nationalCode;
        }

        #endregion

        #region ---------- پیاده‌سازی داخلی ----------

        private static bool IsValid10Core(string code)
        {
            if (code.Length != 10 || IsAllSameDigit(code))
                return false;

            return CalculateControlDigit(code.Substring(0, 9)) == code[9] - '0';
        }

        private static bool IsValid20Core(string code)
        {
            if (code.Length != 20 || !IsAsciiDigits(code) || IsAllSameDigit(code))
                return false;

            if (code.Substring(0, 3) == "000")
                return false; // کد کشور 000 وجود ندارد

            if (CalculateControlDigits20(code.Substring(0, 18)) != code.Substring(18, 2))
                return false;

            // کد تبدیل‌شده از کد ملی ایران باید کد ۱۰ رقمی معتبری درون خود داشته باشد
            if (IsLegacy20Core(code) && code.Substring(0, 3) == IranCountryCode)
                return IsValid10Core(code.Substring(3, 10));

            return true;
        }

        private static bool IsLegacy20Core(string code) => code.Substring(13, 5) == LegacyExtension;

        private static int Mod11Check(int sum)
        {
            int remainder = sum % 11;
            return remainder < 2 ? remainder : 11 - remainder;
        }

        private static string Complete10(string first9) => first9 + CalculateControlDigit(first9);

        private static string Complete20(string first18) => first18 + CalculateControlDigits20(first18);

        private static string NormalizeInput(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            var sb = new StringBuilder(value.Length);
            foreach (char c in value.ToEnglishDigits())
            {
                if (!char.IsWhiteSpace(c) && c != '-' && c != '‌')
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private static bool IsAsciiDigits(string value) => value.Length > 0 && value.All(c => c >= '0' && c <= '9');

        private static bool IsAllSameDigit(string value) => value.All(c => c == value[0]);

        private static string ResolveCountry(string countryCode)
        {
            string country = NormalizeInput(countryCode ?? IranCountryCode);
            if (country.Length != 3 || !IsAsciiDigits(country) || country == "000")
                throw new ArgumentException("کد کشور باید ۳ رقم (001 تا 999) باشد", nameof(countryCode));
            return country;
        }

        private static string ResolveLocation(string locationCode, bool round)
        {
            if (locationCode == null)
            {
                while (true)
                {
                    string generated = round ? RoundDigits(3) : RandomDigits(3);
                    if (generated != "000")
                        return generated;
                }
            }

            string location = NormalizeInput(locationCode);
            if (location.Length != 3 || !IsAsciiDigits(location) || location == "000")
                throw new ArgumentException("کد محل صدور باید ۳ رقم (001 تا 999) باشد", nameof(locationCode));
            return location;
        }

        private static int NextInt(int maxExclusive)
        {
            // عدد تصادفی امن و Thread-safe بدون سوگیری
            var bytes = new byte[4];
            uint limit = uint.MaxValue - (uint.MaxValue % (uint)maxExclusive);
            uint value;
            do
            {
                lock (Rng)
                    Rng.GetBytes(bytes);
                value = BitConverter.ToUInt32(bytes, 0);
            } while (value >= limit);
            return (int)(value % (uint)maxExclusive);
        }

        private static string RandomDigits(int length)
        {
            var chars = new char[length];
            for (int i = 0; i < length; i++)
                chars[i] = (char)('0' + NextInt(10));
            return new string(chars);
        }

        /// <summary>
        /// تولید رشته رند با یکی از الگوهای: تکراری، دوتایی، سه‌تایی، نیمه‌نیمه، متوالی صعودی/نزولی، آینه‌ای
        /// </summary>
        private static string RoundDigits(int length)
        {
            int a = NextInt(10);
            int b = (a + 1 + NextInt(9)) % 10; // b ≠ a
            int c = NextInt(10);
            var patterns = new List<Func<int, int>>
            {
                i => a,                                       // aaaaaa
                i => i % 2 == 0 ? a : b,                      // ababab
                i => i % 3 == 0 ? a : (i % 3 == 1 ? b : c),   // abcabc
                i => i < length / 2 ? a : b,                  // aaabbb
                i => (a + i) % 10,                            // 123456 (متوالی صعودی)
                i => (a - i + 100) % 10,                      // 654321 (متوالی نزولی)
                i => i < (length + 1) / 2 ? (a + i) % 10 : (a + (length - 1 - i)) % 10, // 123321 (آینه‌ای)
                i => i == 0 ? a : 0,                          // a00000
                i => (i / 2) % 2 == 0 ? a : b,                // aabbaabb
            };

            var pattern = patterns[NextInt(patterns.Count)];
            var chars = new char[length];
            for (int i = 0; i < length; i++)
                chars[i] = (char)('0' + pattern(i));
            return new string(chars);
        }

        #endregion
    }
}
