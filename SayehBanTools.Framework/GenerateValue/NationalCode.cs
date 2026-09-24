using System;
using System.Linq;
using System.Text.RegularExpressions;
namespace SayehBanTools.Framework.GenerateValue
{
    /// <summary>
    /// کلاس تولید و بررسی کد ملی
    /// </summary>
    public class NationalCode
    {
        private static readonly Random _random = new Random();
        private static readonly string[] InvalidCodes = new[] { "0000000000", "1111111111", "2222222222", "3333333333", "4444444444", "5555555555", "6666666666", "7777777777", "8888888888", "9999999999" };

        /// <summary>
        /// تولید کد ملی تصادفی معتبر
        /// </summary>
        /// <returns>کد ملی 10 رقمی معتبر</returns>
        public static string GenerateRandom()
        {
            while (true)
            {
                // تولید 9 رقم تصادفی
                char[] digits = new char[9];
                for (int i = 0; i < 9; i++)
                {
                    digits[i] = (char)('0' + _random.Next(0, 10));
                }
                string code = new string(digits);

                // محاسبه رقم کنترل
                int controlDigit = CalculateControlDigit(code);
                if (controlDigit < 0 || controlDigit > 9)
                    continue; // اگر رقم کنترل نامعتبر است، دوباره تولید کن

                string nationalCode = code + controlDigit;
                if (InvalidCodes.Contains(nationalCode))
                    continue; // اگر کد جزو کدهای نامعتبر است، دوباره تولید کن

                if (IsValidNationalCode(nationalCode))
                    return nationalCode;
            }
        }

        /// <summary>
        /// تولید کد ملی رند و معتبر
        /// </summary>
        /// <returns>کد ملی 10 رقمی رند و معتبر</returns>
         public static string GenerateRound()
        {
            string ss = "";
            string number123 = "0123456789";
            Random rnd = new Random();
        
            // الگوهای رند (9 رقم اول)
            string[] roundPatterns = new[]
            {
                "123456789", // متوالی
                "987654321", // متوالی معکوس
                "112233445", // تکراری
                "554433221", // تکراری معکوس
                "121212121"  // متناوب
            };

            while (true)
            {
                // انتخاب تصادفی یک الگو
                string pattern = roundPatterns[rnd.Next(roundPatterns.Length)];
            
                // استخراج ارقام از الگو
                object code1 = int.Parse(pattern[0].ToString());
                object code2 = int.Parse(pattern[1].ToString());
                object code3 = int.Parse(pattern[2].ToString());
                object code4 = int.Parse(pattern[3].ToString());
                object code5 = int.Parse(pattern[4].ToString());
                object code6 = int.Parse(pattern[5].ToString());
                object code7 = int.Parse(pattern[6].ToString());
                object code8 = int.Parse(pattern[7].ToString());
                object code9 = int.Parse(pattern[8].ToString());

                string numbers1 = number123.Substring(Convert.ToInt32(code1), 1);
                string numbers2 = number123.Substring(Convert.ToInt32(code2), 1);
                string numbers3 = number123.Substring(Convert.ToInt32(code3), 1);
                string numbers4 = number123.Substring(Convert.ToInt32(code4), 1);
                string numbers5 = number123.Substring(Convert.ToInt32(code5), 1);
                string numbers6 = number123.Substring(Convert.ToInt32(code6), 1);
                string numbers7 = number123.Substring(Convert.ToInt32(code7), 1);
                string numbers8 = number123.Substring(Convert.ToInt32(code8), 1);
                string numbers9 = number123.Substring(Convert.ToInt32(code9), 1);
                string sumnumber = numbers1 + numbers2 + numbers3 + numbers4 + numbers5 + numbers6 + numbers7 + numbers8 + numbers9;
                var code = sumnumber.ToArray();
                if (code.Length == 9)
                {
                    int numberPosition = 10;
                    int sum = 0;
                    while (numberPosition >= 2)
                    {
                        for (int i = 0; i <= 8; i++)
                        {
                            int number = Convert.ToInt32(code[i].ToString()) * numberPosition;
                            sum = sum + number;
                            numberPosition--;
                        }
                    }

                    int numberControl = (11 - (sum % 11));
                    ss = sumnumber + Convert.ToString(numberControl);
                    if (ss.Length == 10)
                    {
                        return ss; // خروجی بدون بررسی اضافی، مشابه GenerateRandom
                    }
                }
            }
        }
        /// <summary>
        /// محاسبه رقم کنترل کد ملی
        /// </summary>
        /// <param name="code">9 رقم اول کد ملی</param>
        /// <returns>رقم کنترل (0 تا 9) یا -1 اگر نامعتبر باشد</returns>
        private static int CalculateControlDigit(string code)
        {
            if (code.Length != 9)
                return -1;

            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += (code[i] - '0') * (10 - i);
            }

            int remainder = sum % 11;
            return remainder < 2 ? remainder : 11 - remainder;
        }

        /// <summary>
        /// بررسی صحت کد ملی
        /// </summary>
        /// <param name="nationalCode">کد ملی</param>
        /// <returns>درست یا نادرست</returns>
        /// <exception cref="Exception">در صورت ورودی نامعتبر</exception>
        public static bool IsValidNationalCode(string nationalCode)
        {
            if (string.IsNullOrEmpty(nationalCode))
                throw new Exception("لطفا کد ملی را صحیح وارد نمایید");

            if (nationalCode.Length != 10)
                throw new Exception("طول کد ملی باید ده کاراکتر باشد");

            var regex = new Regex(@"\d{10}");
            if (!regex.IsMatch(nationalCode))
                throw new Exception("کد ملی تشکیل شده از ده رقم عددی می‌باشد؛ لطفا کد ملی را صحیح وارد نمایید");

            if (InvalidCodes.Contains(nationalCode))
                return false;

            var chArray = nationalCode.ToCharArray();
            var num0 = Convert.ToInt32(chArray[0].ToString()) * 10;
            var num2 = Convert.ToInt32(chArray[1].ToString()) * 9;
            var num3 = Convert.ToInt32(chArray[2].ToString()) * 8;
            var num4 = Convert.ToInt32(chArray[3].ToString()) * 7;
            var num5 = Convert.ToInt32(chArray[4].ToString()) * 6;
            var num6 = Convert.ToInt32(chArray[5].ToString()) * 5;
            var num7 = Convert.ToInt32(chArray[6].ToString()) * 4;
            var num8 = Convert.ToInt32(chArray[7].ToString()) * 3;
            var num9 = Convert.ToInt32(chArray[8].ToString()) * 2;
            var a = Convert.ToInt32(chArray[9].ToString());

            var b = num0 + num2 + num3 + num4 + num5 + num6 + num7 + num8 + num9;
            var c = b % 11;

            return (c < 2 && a == c) || (c >= 2 && (11 - c) == a);
        }
    }
}