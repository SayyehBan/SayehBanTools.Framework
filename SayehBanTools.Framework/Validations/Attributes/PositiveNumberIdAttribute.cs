using System;
using System.ComponentModel.DataAnnotations;

namespace SayehBanTools.Framework.Validations.Attributes
{
    /// <summary>
    /// Attribute برای اعتبار‌سنجی شناسه عددی مثبت (غیرصفر و غیرمنفی)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class PositiveNumberIdAttribute : ValidationAttribute
    {
        /// <summary>
        /// پیام خطای پیش‌فرض برای زمانی که شناسه عددی نامعتبر است
        /// </summary>
        private const string DefaultErrorMessage = "شناسه نمی‌تواند خالی، صفر یا منفی باشد.";

        /// <summary>
        /// سازنده پیش‌فرض
        /// </summary>
        public PositiveNumberIdAttribute() : base(DefaultErrorMessage)
        {
        }

        /// <summary>
        /// سازنده با پیام خطای سفارشی
        /// </summary>
        /// <param name="errorMessage">پیام خطای سفارشی</param>
        public PositiveNumberIdAttribute(string errorMessage) : base(errorMessage)
        {
        }

        /// <summary>
        /// بررسی صحت شناسه عددی
        /// </summary>
        /// <param name="value">مقداری که باید بررسی شود</param>
        /// <param name="validationContext">زمینه اعتبار‌سنجی</param>
        /// <returns>نتیجه اعتبار‌سنجی</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // اگر مقدار null باشد، نامعتبر است
            if (value == null)
            {
                return new ValidationResult(ErrorMessage ?? DefaultErrorMessage);
            }

            // بررسی نوع مقدار (باید عدد صحیح باشد)
            if (!(value is int id))
            {
                return new ValidationResult("شناسه باید یک عدد صحیح باشد.");
            }

            // بررسی مثبت بودن و غیرصفر بودن
            if (id <= 0)
            {
                return new ValidationResult(ErrorMessage ?? DefaultErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}