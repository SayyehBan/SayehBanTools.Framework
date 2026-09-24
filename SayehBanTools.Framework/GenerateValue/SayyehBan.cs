using System;
using System.Linq;
using System.Text.RegularExpressions;
namespace SayehBanTools.Framework.GenerateValue
{
    /// <summary>
    /// کلاس بررسی و تولید کد سایه‌بان
    /// </summary>
    public class SayyehBan
    {
        /// <summary>
        /// بررسی صحت کد سایه‌بان
        /// </summary>
        /// <param name="nationalCode">کد سایه‌بان 13 رقمی</param>
        /// <returns>درستی یا نادرستی کد</returns>
        /// <exception cref="Exception">در صورت نادرست بودن فرمت کد</exception>
        public static bool IsValidCode(string nationalCode)
        {
            // بررسی تهی بودن
            if (string.IsNullOrEmpty(nationalCode))
                throw new Exception("لطفا کد سایه‌بان را صحیح وارد نمایید");

            // بررسی طول کد
            if (nationalCode.Length != 13)
                throw new Exception("طول کد سایه‌بان باید سیزده کاراکتر باشد");

            // بررسی عددی بودن
            var regex = new Regex(@"\d{13}");
            if (!regex.IsMatch(nationalCode))
                throw new Exception("کد سایه‌بان باید شامل سیزده رقم عددی باشد");

            // بررسی یکسان بودن تمام ارقام
            var allDigitEqual = new[] { "0000000000000", "1111111111111", "2222222222222", "3333333333333",
                                        "4444444444444", "5555555555555", "6666666666666", "7777777777777",
                                        "8888888888888", "9999999999999" };
            if (allDigitEqual.Contains(nationalCode))
                return false;

            // محاسبه الگوریتم
            var chArray = nationalCode.ToCharArray();
            int sum = 0;
            int[] weights = { 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            for (int i = 0; i < 12; i++)
            {
                sum += Convert.ToInt32(chArray[i].ToString()) * weights[i];
            }
            int a = Convert.ToInt32(chArray[12].ToString());
            int c = sum % 14;

            return (c < 2 && a == c) || (c >= 2 && (14 - c) == a);
        }

        /// <summary>
        /// تولید کد سایه‌بان تصادفی و معتبر
        /// </summary>
        /// <returns>یک کد سایه‌بان 13 رقمی معتبر</returns>
        public static string GenerateRandom()
        {
            Random rnd = new Random();
            int[] digits = new int[12];

            // تولید 12 رقم اول به‌صورت تصادفی
            for (int i = 0; i < 12; i++)
            {
                digits[i] = rnd.Next(0, 10); // اعداد بین 0 تا 9
            }

            // محاسبه مجموع وزن‌دار
            int sum = 0;
            int[] weights = { 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            for (int i = 0; i < 12; i++)
            {
                sum += digits[i] * weights[i];
            }

            // محاسبه رقم کنترلی
            int c = sum % 14;
            int controlDigit = c < 2 ? c : 14 - c;

            // اگر controlDigit دو رقمی است، دوباره تولید کن
            if (controlDigit > 9)
            {
                return GenerateRandom();
            }

            // ترکیب ارقام
            string code = string.Join("", digits) + controlDigit;

            // اطمینان از معتبر بودن کد
            if (!IsValidCode(code))
            {
                return GenerateRandom();
            }

            return code;
        }

        /// <summary>
        /// تولید کد سایه‌بان رند و معتبر
        /// </summary>
        /// <returns>یک کد سایه‌بان 13 رقمی معتبر با الگوی رند</returns>
        public static string GenerateRound()
        {
            Random rnd = new Random();
            int[] digits = new int[12];

            // الگوی رند: شروع با "00" و ادامه با اعداد محدود
            digits[0] = 0; // رقم اول
            digits[1] = 0; // رقم دوم
            for (int i = 2; i < 12; i++)
            {
                digits[i] = rnd.Next(0, 5); // محدود به 0 تا 4 برای الگوی منظم‌تر
            }

            // محاسبه مجموع وزن‌دار
            int sum = 0;
            int[] weights = { 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            for (int i = 0; i < 12; i++)
            {
                sum += digits[i] * weights[i];
            }

            // محاسبه رقم کنترلی
            int c = sum % 14;
            int controlDigit = c < 2 ? c : 14 - c;

            // اگر controlDigit دو رقمی است، دوباره تولید کن
            if (controlDigit > 9)
            {
                return GenerateRound();
            }

            // ترکیب ارقام
            string code = string.Join("", digits) + controlDigit;

            // اطمینان از معتبر بودن کد
            if (!IsValidCode(code))
            {
                return GenerateRound();
            }

            return code;
        }
    }
}