using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SayehBanTools.Framework.Converter
{
    /// <summary>
    /// افزونه مدیریت رشته ها
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// نرمال‌سازی رشته بر اساس کد زبان.
        /// ۹۹ زبان پشتیبانی می‌شود (فهرست کامل در <see cref="SupportedLanguages"/>)؛
        /// کد دوحرفی (fa) و کد منطقه‌ای (fa-IR) هر دو قابل قبول است.
        /// </summary>
        /// <param name="str">رشته ورودی</param>
        /// <param name="languageCode">کد زبان</param>
        /// <param name="toNativeDigits">
        /// true: همه ارقام (از هر خطی) به رقم بومی زبان تبدیل می‌شوند (برای زبان‌های بدون رقم بومی، رقم انگلیسی).
        /// false: همه ارقام به انگلیسی تبدیل می‌شوند و جداکننده‌های عربی (٫ ٬) به . و , تبدیل می‌شوند.
        /// </param>
        public static string NormalizeByLanguage(this string str, string languageCode, bool toNativeDigits = true)
        {
            var language = SupportedLanguages.Find(languageCode);

            if (string.IsNullOrEmpty(str))
                return toNativeDigits ? (language?.DefaultDigit ?? "0") : "0";

            if (language == null)
                Debug.WriteLine($"Warning: Language code '{languageCode}' is not supported. Using default rules.");

            char digitZero = toNativeDigits && language?.NativeDigitZero != null ? language.NativeDigitZero.Value : '0';
            bool preserveJoiners = language?.PreserveJoiners ?? false;
            var replacements = language?.CharReplacements;

            var sb = new StringBuilder(str.Length);
            foreach (char c in str)
            {
                // هر رقم دهدهی یونیکد (فارسی، عربی، هندی، تایلندی، تمام‌عرض چینی و ...)
                if (char.IsDigit(c))
                {
                    int value = CharUnicodeInfo.GetDecimalDigitValue(c);
                    if (value >= 0)
                    {
                        sb.Append((char)(digitZero + value));
                        continue;
                    }
                }

                switch (c)
                {
                    case '\u00A0': // فاصله نشکن
                        sb.Append(' ');
                        continue;
                    case '\u200C': // نیم‌فاصله (ZWNJ)
                        if (!preserveJoiners)
                        {
                            sb.Append(' ');
                            continue;
                        }
                        break;
                    case '\u066B': // ٫ جداکننده اعشار عربی/فارسی
                        if (!toNativeDigits)
                        {
                            sb.Append('.');
                            continue;
                        }
                        break;
                    case '\u066C': // ٬ جداکننده هزارگان عربی/فارسی
                        if (!toNativeDigits)
                        {
                            sb.Append(',');
                            continue;
                        }
                        break;
                }

                sb.Append(replacements != null && replacements.TryGetValue(c, out var replaced) ? replaced : c);
            }

            return sb.ToString();
        }

        /// <summary>
        /// تبدیل ارقام هر زبانی (فارسی، عربی، هندی، بنگالی، تایلندی، ...) به ارقام انگلیسی بدون تغییر حروف
        /// </summary>
        public static string ToEnglishDigits(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str ?? "";

            var chars = str.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (char.IsDigit(chars[i]))
                {
                    int value = CharUnicodeInfo.GetDecimalDigitValue(chars[i]);
                    if (value >= 0)
                        chars[i] = (char)('0' + value);
                }
            }
            return new string(chars);
        }

        /// <summary>
        /// تبدیل ارقام به رقم بومی زبان بدون تغییر حروف (برای زبان‌های بدون رقم بومی، ارقام انگلیسی)
        /// </summary>
        public static string ToNativeDigits(this string str, string languageCode)
        {
            if (string.IsNullOrEmpty(str))
                return str ?? "";

            char zero = SupportedLanguages.Find(languageCode)?.NativeDigitZero ?? '0';
            var chars = str.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (char.IsDigit(chars[i]))
                {
                    int value = CharUnicodeInfo.GetDecimalDigitValue(chars[i]);
                    if (value >= 0)
                        chars[i] = (char)(zero + value);
                }
            }
            return new string(chars);
        }

        /// <summary>
        /// آیا کد زبان جزو ۹۹ زبان پشتیبانی‌شده است
        /// </summary>
        public static bool IsSupportedLanguage(this string languageCode) => SupportedLanguages.IsSupported(languageCode);

        /// <summary>
        /// آیا زبان راست‌به‌چپ است
        /// </summary>
        public static bool IsRtlLanguage(this string languageCode) => SupportedLanguages.Find(languageCode)?.IsRtl ?? false;

        /// <summary>
        /// جهت متن زبان: rtl یا ltr (برای زبان ناشناخته ltr)
        /// </summary>
        public static string GetTextDirection(this string languageCode) => languageCode.IsRtlLanguage() ? "rtl" : "ltr";

        /// <summary>
        /// پاک‌سازی رشته با توجه به زبان
        /// </summary>
        public static string CleanString(this string str, string languageCode = "en")
        {
            if (string.IsNullOrEmpty(str))
                return "";

            return str.NormalizeByLanguage(languageCode, false).Trim();
        }

        /// <summary>
        /// تمیز کردن برحسب json یکباره
        /// </summary>
        public static string CleanJsonByLanguage(this string json, string defaultLanguage = "en")
        {
            if (string.IsNullOrEmpty(json))
                return json;

            try
            {
                var translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                if (translations == null)
                    return json;

                var cleanedTranslations = translations.ToDictionary(
                    kvp => kvp.Key,
                    // فرض بر این است که CleanForSql در کلاس دیگری تعریف شده است.
                    kvp => kvp.Value.CleanString(kvp.Key)
                );

                return JsonConvert.SerializeObject(cleanedTranslations);
            }
            catch
            {
                // اگر پردازش JSON با خطا مواجه شد، مقدار اصلی را برگردان
                return json;
            }
        }

        /// <summary>
        /// تبدیل رشته به عدد صحیح
        /// </summary>
        public static int ToInt(this string value, string languageCode = "en")
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            return int.Parse(value.NormalizeByLanguage(languageCode, false));
        }

        /// <summary>
        /// تبدیل رشته به عدد اعشاری (Decimal) با حفظ دقیق نقطه اعشار
        /// </summary>
        public static decimal ToDecimal(this string value, string languageCode = "en")
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            string cleanValue = value.NormalizeByLanguage(languageCode, false).Replace(",", ""); // فقط کاما حذف شود، نقطه بماند
            return decimal.Parse(cleanValue, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// تبدیل رشته به عدد اعشاری (Double / Float) با حفظ دقیق نقطه اعشار
        /// </summary>
        public static double ToDouble(this string value, string languageCode = "en")
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            string cleanValue = value.NormalizeByLanguage(languageCode, false).Replace(",", ""); // فقط کاما حذف شود، نقطه بماند
            return double.Parse(cleanValue, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// فرمت‌بندی عدد صحیح به‌صورت عددی با جداکننده
        /// </summary>
        public static string ToNumeric(this int value, string languageCode = "en")
        {
            var culture = GetCultureInfo(languageCode);
            return value.ToString("N0", culture).NormalizeByLanguage(languageCode, true);
        }

        /// <summary>
        /// فرمت‌بندی عدد اعشاری به‌صورت عددی با جداکننده (حفظ اعشار در نمایش)
        /// </summary>
        public static string ToNumeric(this decimal value, string languageCode = "en")
        {
            var culture = GetCultureInfo(languageCode);
            // N3 یعنی تا ۳ رقم اعشار را نشان می‌دهد، صفرهای اضافی حذف می‌شوند.
            return value.ToString("0.###", culture).NormalizeByLanguage(languageCode, true);
        }

        /// <summary>
        /// فرمت‌بندی عدد صحیح به‌صورت ارز
        /// </summary>
        public static string ToCurrency(this int value, string languageCode = "en")
        {
            var culture = GetCultureInfo(languageCode);
            return value.ToString("C0", culture).NormalizeByLanguage(languageCode, true);
        }

        /// <summary>
        /// فرمت‌بندی عدد اعشاری به‌صورت ارز
        /// </summary>
        public static string ToCurrency(this decimal value, string languageCode = "en")
        {
            var culture = GetCultureInfo(languageCode);
            return value.ToString("C0", culture).NormalizeByLanguage(languageCode, true);
        }

        /// <summary>
        /// حذف جداکننده‌ها از رشته (بدون حذف نقطه اعشار برای پشتیبانی از Float/Percent)
        /// </summary>
        public static string RemovePoint(this string str)
        {
            // نقطه (.) دیگر حذف نمی‌شود تا مقادیر اعشاری خراب نشوند.
            return str?.Replace(",", "").Replace("/", "") ?? "";
        }

        /// <summary>
        /// پاک‌سازی پیام خطا
        /// </summary>
        public static string Msg(this string str, string languageCode = "en")
        {
            return str?.Replace("Core .Net SqlClient Data Provider-", "").CleanString(languageCode) ?? "";
        }

        /// <summary>
        /// بررسی وجود مقدار
        /// </summary>
        public static bool HasValue(this string value, bool ignoreWhiteSpace = true)
        {
            return ignoreWhiteSpace ? !string.IsNullOrWhiteSpace(value) : !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// تبدیل رشته خالی به null
        /// </summary>
        public static string NullIfEmpty(this string str)
        {
            return str?.Length == 0 ? null : str;
        }

        /// <summary>
        /// حذف تگ‌های HTML
        /// </summary>
        public static string HtmlTags(this string str)
        {
            if (string.IsNullOrEmpty(str)) return "";
            string pattern = @"<[^>]*(>|$)|‌|»|«";
            return Regex.Replace(str.Replace(" ", " ").Trim(), pattern, string.Empty);
        }

        /// <summary>
        /// جایگزینی خطوط جدید با فاصله
        /// </summary>
        public static string ASCII(this string str)
        {
            if (string.IsNullOrEmpty(str)) return "";
            string pattern = @"\r\n";
            return Regex.Replace(str.Trim(), pattern, " ");
        }

        /// <summary>
        /// تبدیل فاصله به خط‌تیره
        /// </summary>
        public static string Slug(this string str)
        {
            return str?.Replace(" ", "-") ?? "";
        }

        /// <summary>
        /// حذف خط‌تیره و تبدیل به فاصله
        /// </summary>
        public static string RemoveSlug(this string str)
        {
            return str?.Replace("-", " ") ?? "";
        }

        /// <summary>
        /// جایگزین کردن Coma
        /// </summary>
        public static string ReplaceComa(this string str)
        {
            return str.Replace("/", ",");
        }

        /// <summary>
        /// حذف عبارت wwwroot/ از مسیر بدون حساسیت به حروف کوچک و بزرگ
        /// </summary>
        public static string RemoveDirectWWWROOT(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            // استفاده از Regex جهت اعمال حالت IgnoreCase در .NET Framework
            return Regex.Replace(str, @"wwwroot/", "", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// جایگزینی ::1 با 127.0.0.1
        /// </summary>
        public static string AnonymousIP(this string str)
        {
            return str.Replace("::1", "127.0.0.1") ?? "";
        }

        /// <summary>
        /// متدهای قدیمی برای سازگاری
        /// </summary>
        public static string En2Fa(this string str) => str.NormalizeByLanguage("fa", true);

        /// <summary>
        /// تبدیل عدد فارسی به انگلیسی
        /// </summary>
        public static string Fa2En(this string str) => str.NormalizeByLanguage("fa", false);

        /// <summary>
        /// فیکس کردن به زبان فارسی
        /// </summary>
        public static string FixPersianChars(this string str) => str.NormalizeByLanguage("fa", false);


        /// <summary>
        /// دریافت CultureInfo بر اساس کد زبان
        /// </summary>
        private static CultureInfo GetCultureInfo(string languageCode)
        {
            var language = SupportedLanguages.Find(languageCode);
            if (language != null)
                return language.Culture;

            try
            {
                return CultureInfo.GetCultureInfo(string.IsNullOrWhiteSpace(languageCode) ? "en-US" : languageCode.Trim());
            }
            catch (CultureNotFoundException)
            {
                return CultureInfo.InvariantCulture; // پیش‌فرض
            }
        }

        /// <summary>
        /// حذف کامل تگ‌های HTML و entityهای رایج
        /// </summary>
        public static string StripHtml(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            string text = Regex.Replace(input, "<.*?>", string.Empty);  // حذف تگ‌ها
            text = Regex.Replace(text, @"&[a-zA-Z]+;", " ");            // حذف entityها (ساده)
            text = Regex.Replace(text, @"&laquo;", "«");
            text = Regex.Replace(text, @"&raquo;", "»");
            text = Regex.Replace(text, @"&zwnj;", "‌");
            text = Regex.Replace(text, @"&nbsp;", " ");
            text = Regex.Replace(text, @"&amp;", "&");

            return text.Trim();
        }

        /// <summary>
        /// تمیزسازی مقادیر سلول‌ها جهت جلوگیری از به‌هم‌ریختگی فرمت فایل
        /// </summary>
        public static string CleanValue(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            // حذف Tab، Enter و کاراکترهای کنترل که ساختار فایل TSV را خراب می‌کنند
            return value.Replace("\t", " ")
                        .Replace("\r\n", " ")
                        .Replace("\r", " ")
                        .Replace("\n", " ");
        }
    }
}
/*
        // تبدیل مدل به JSON و تمیز کردن هر مقدار بر اساس کد زبانش
        var translationsJson = JsonConvert.SerializeObject(model.Translations)
            .CleanJsonByLanguage();
     */