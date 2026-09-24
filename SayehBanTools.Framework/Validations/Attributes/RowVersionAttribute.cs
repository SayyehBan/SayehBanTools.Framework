using System;
using System.ComponentModel.DataAnnotations;

namespace SayehBanTools.Framework.Validations.Attributes
{
    /// <summary>
    /// Attribute برای اعتبار‌سنجی مقدار RowVersion (Timestamp) به صورت رشته Base64
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class RowVersionAttribute : ValidationAttribute
    {
        /// <summary>
        /// پیام خطای پیش‌فرض
        /// </summary>
        private const string DefaultErrorMessage = "مقدار RowVersion نامعتبر است. باید یک رشته Base64 معتبر با طول 12 کاراکتر باشد.";

        /// <summary>
        /// آیا فیلد اجباری است یا می‌تواند null باشد؟
        /// پیش‌فرض: true (اجباری)
        /// </summary>
        public bool Required { get; set; } = true;

        /// <summary>
        /// سازنده پیش‌فرض (Required = true)
        /// </summary>
        public RowVersionAttribute() : base(DefaultErrorMessage)
        {
        }

        /// <summary>
        /// سازنده با پیام خطای سفارشی (Required = true)
        /// </summary>
        /// <param name="errorMessage">پیام خطای سفارشی</param>
        public RowVersionAttribute(string errorMessage) : base(errorMessage)
        {
        }
        /// <summary>
        /// اعتبار‌سنجی مقدار RowVersion
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // اگر Required نباشد و مقدار null باشد → معتبر است
            if (!Required && value == null)
            {
                return ValidationResult.Success;
            }

            // اگر Required باشد و مقدار null باشد → نامعتبر
            if (Required && value == null)
            {
                return new ValidationResult(ErrorMessage ?? DefaultErrorMessage);
            }

            // در این مرحله value حتماً null نیست
            if (!(value is string rowVersion))
            {
                return new ValidationResult("مقدار RowVersion باید یک رشته باشد.");
            }

            // بررسی خالی نبودن رشته
            if (string.IsNullOrWhiteSpace(rowVersion))
            {
                return new ValidationResult(ErrorMessage ?? DefaultErrorMessage);
            }

            // بررسی معتبر بودن Base64 و طول دقیق
            try
            {
                if (rowVersion.Length != 12)
                {
                    return new ValidationResult(ErrorMessage ?? DefaultErrorMessage);
                }

                byte[] decodedBytes = Convert.FromBase64String(rowVersion);

                if (decodedBytes.Length != 8)
                {
                    return new ValidationResult(ErrorMessage ?? DefaultErrorMessage);
                }
            }
            catch (FormatException)
            {
                return new ValidationResult(ErrorMessage ?? DefaultErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}