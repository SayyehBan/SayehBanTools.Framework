using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SayehBanTools.Framework.Validations.Attributes
{
    /// <summary>
    /// Attribute برای اعتبار سنجی رمز عبور ضعیف
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class CommonPasswordAttribute : ValidationAttribute
    {
        private readonly string _filePath;
        private readonly List<string> _commonPasswords = new List<string>();

        /// <summary>
        /// سازنده با مسیر فایل و پیام خطای پیش فرض
        /// </summary>
        /// <param name="filePath">مسیر فایل worst-passwords.txt</param>
        public CommonPasswordAttribute(string filePath) : base("پسورد شما قابل شناسایی توسط ربات های هکر است! لطفا یک پسورد قوی انتخاب کنید")
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            LoadCommonPasswords();
        }

        /// <summary>
        /// سازنده با مسیر فایل و پیام خطای سفارشی
        /// </summary>
        /// <param name="filePath">مسیر فایل worst-passwords.txt</param>
        /// <param name="errorMessage">پیام خطای سفارشی</param>
        public CommonPasswordAttribute(string filePath, string errorMessage) : base(errorMessage)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            LoadCommonPasswords();
        }

        /// <summary>
        /// لود لیست هش های پسوردهای ضعیف از فایل (سنکرون)
        /// </summary>
        private void LoadCommonPasswords()
        {
            if (_commonPasswords.Count == 0 && File.Exists(_filePath))
            {
                using (StreamReader reader = new StreamReader(_filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (line != null)
                        {
                            string hashedLine = ComputeSha256Hash(line);
                            _commonPasswords.Add(hashedLine);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// محاسبه هش SHA256
        /// </summary>
        /// <param name="input">ورودی</param>
        /// <returns>هش SHA256</returns>
        private string ComputeSha256Hash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashedBytes = sha256.ComputeHash(inputBytes);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        /// <summary>
        /// بررسی صحت رمز عبور
        /// </summary>
        /// <param name="value">مقداری که باید بررسی شود</param>
        /// <param name="validationContext">زمینه اعتبار سنجی</param>
        /// <returns>نتیجه اعتبار سنجی</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("رمز عبور نمی تواند null باشد.");
            }

            // تغییر یافته برای سازگاری با C# 7.3
            if (!(value is string password))
            {
                return new ValidationResult("مقدار باید یک رشته باشد.");
            }

            // بررسی حداقل 8 کاراکتر
            if (password.Length < 8)
            {
                return new ValidationResult("رمز عبور باید حداقل 8 کاراکتر باشد.");
            }

            // محاسبه هش و بررسی در لیست
            string hashedPassword = ComputeSha256Hash(password);
            if (_commonPasswords.Contains(hashedPassword))
            {
                return new ValidationResult(ErrorMessage ?? "پسورد شما قابل شناسایی توسط ربات های هکر است! لطفا یک پسورد قوی انتخاب کنید");
            }

            return ValidationResult.Success;
        }
    }
}