using System;
using System.ComponentModel.DataAnnotations;

namespace SayehBanTools.Framework.Validations.Attributes
{
    /// <summary>
    /// Attribute برای اعتبار‌سنجی مسیر سلسله مراتب
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class HierarchyPathAttribute : ValidationAttribute
    {
        /// <summary>
        /// پیام خطای پیش‌فرض برای زمانی که مسیر سلسله مراتب نامعتبر است
        /// </summary>
        private const string DefaultErrorMessage = "فرمت مسیر سلسله مراتب نامعتبر است. باید به صورت / یا /1/ یا /1/2/ باشد و شامل چند اسلش متوالی نباشد.";

        /// <summary>
        /// سازنده پیش‌فرض
        /// </summary>
        public HierarchyPathAttribute() : base(DefaultErrorMessage)
        {
        }

        /// <summary>
        /// سازنده با پیام خطای سفارشی
        /// </summary>
        /// <param name="errorMessage">پیام خطای سفارشی</param>
        public HierarchyPathAttribute(string errorMessage) : base(errorMessage)
        {
        }

        /// <summary>
        /// بررسی صحت مسیر سلسله مراتب
        /// </summary>
        /// <param name="value">مقداری که باید بررسی شود</param>
        /// <param name="validationContext">زمینه اعتبار‌سنجی</param>
        /// <returns>نتیجه اعتبار‌سنجی</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // اگر مقدار null باشد، نامعتبر است
            if (value == null)
            {
                return new ValidationResult("مسیر سلسله مراتب نمی‌تواند null باشد.");
            }

            // بررسی نوع مقدار (باید رشته باشد)
            if (!(value is  string path))
            {
                return new ValidationResult("مقدار باید یک رشته باشد.");
            }

            // استفاده از متد موجود برای بررسی صحت مسیر
            if (HierarchyValidator.IsValidHierarchyPath(path))
            {
                return ValidationResult.Success;
            }

            // در صورت نامعتبر بودن، پیام خطا را برمی‌گرداند
            return new ValidationResult(ErrorMessage ?? DefaultErrorMessage);
        }
    }

}
