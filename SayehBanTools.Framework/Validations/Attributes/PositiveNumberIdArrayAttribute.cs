using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SayehBanTools.Framework.Validations.Attributes
{
    /// <summary>
    /// Attribute برای اعتبار‌سنجی آرایه‌ای از شناسه‌های عددی مثبت (غیرصفر و غیرمنفی)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class PositiveNumberIdArrayAttribute : ValidationAttribute
    {
        /// <summary>
        /// پیام خطای پیش‌فرض برای زمانی که آرایه شناسه‌های عددی نامعتبر است
        /// </summary>
        private const string DefaultErrorMessage = "شناسه‌ها نمی‌توانند خالی، صفر یا منفی باشند.";

        /// <summary>
        /// سازنده پیش‌فرض
        /// </summary>
        public PositiveNumberIdArrayAttribute() : base(DefaultErrorMessage)
        {
        }

        /// <summary>
        /// سازنده با پیام خطای سفارشی
        /// </summary>
        /// <param name="errorMessage">پیام خطای سفارشی</param>
        public PositiveNumberIdArrayAttribute(string errorMessage) : base(errorMessage)
        {
        }

        /// <summary>
        /// بررسی صحت آرایه شناسه‌های عددی
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

            // بررسی نوع مقدار (باید آرایه اعداد صحیح باشد)
            if (!(value is int[] ids))
            {
                return new ValidationResult("مقدار باید آرایه‌ای از اعداد صحیح باشد.");
            }

            // بررسی خالی نبودن آرایه
            if (ids.Length == 0)
            {
                return new ValidationResult(ErrorMessage ?? DefaultErrorMessage);
            }

            // بررسی مثبت بودن و غیرصفر بودن تمام عناصر
            var invalidIds = ids.Where(id => id <= 0).ToList();
            if (invalidIds.Any())
            {
                var invalidIdsString = string.Join(", ", invalidIds);
                var errorMessage = $"{ErrorMessage ?? DefaultErrorMessage} شناسه‌های نامعتبر: [{invalidIdsString}]";
                return new ValidationResult(errorMessage);
            }

            return ValidationResult.Success;
        }
    }
}