using System;
using System.ComponentModel.DataAnnotations;

namespace SayehBanTools.Framework.Validations.Attributes
{
    /// <summary>
    /// Attribute برای اعتبار‌سنجی GUID با کنترل اجباری بودن (بدون تداخل با [Required])
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class GuidAttribute : ValidationAttribute
    {
        /// <summary>
        ///بررسی اجباری بودن یا نبودن اگر مقدار true  باید guid داده بشه اگر false نیاز نیست ولی داده شد بررسی میشه اشتباه نباشه
        /// </summary>
        public bool Required { get; }

        private const string DefaultRequiredMessage = "فیلد {0} الزامی است.";
        private const string DefaultInvalidMessage = "مقدار GUID برای فیلد {0} نامعتبر است.";
        /// <summary>
        /// کلاس سازنده
        /// </summary>
        /// <param name="required"></param>
        public GuidAttribute(bool required = true)
        {
            Required = required;
            // مهم: این خط باعث می‌شود ASP.NET Core [Required] داخلی رو فعال نکنه
            ErrorMessage = required ? DefaultRequiredMessage : DefaultInvalidMessage;
        }
        /// <summary>
        /// کلاس سازنده
        /// </summary>
        /// <param name="required"></param>
        /// <param name="errorMessage"></param>
        public GuidAttribute(bool required, string errorMessage) : base(errorMessage)
        {
            Required = required;
        }

        /// <summary>
        /// مهم: این پراپرتی باعث می‌شه ASP.NET Core فکر نکنه این Attribute خودش [Required] هست
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            // این متد فقط برای جلوگیری از فعال شدن [Required] داخلی استفاده می‌شه
            return true;
        }
        /// <summary>
        /// بررسی اعتبار ستجی GUID
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // اگر Required = false و مقدار null یا Guid.Empty یا string خالی → معتبر
            if (!Required)
            {
                if (value == null ||
                    value is Guid guid && guid == Guid.Empty ||
                    value is string str && string.IsNullOrWhiteSpace(str))
                {
                    return ValidationResult.Success;
                }
            }

            // اگر Required = true و مقدار null یا خالی → خطا
            if (value == null ||
                value is Guid g && g == Guid.Empty ||
                value is string s && string.IsNullOrWhiteSpace(s))
            {
                var memberName = validationContext.DisplayName ?? validationContext.MemberName;
                var message = string.Format(ErrorMessage ?? DefaultRequiredMessage, memberName);
                return new ValidationResult(message, new[] { validationContext.MemberName });
            }

            // اگر مقدار string هست → بررسی فرمت GUID
            if (value is string stringValue)
            {
                if (!Guid.TryParse(stringValue, out _))
                {
                    var memberName = validationContext.DisplayName ?? validationContext.MemberName;
                    var message = string.Format(ErrorMessage ?? DefaultInvalidMessage, memberName);
                    return new ValidationResult(message, new[] { validationContext.MemberName });
                }
                return ValidationResult.Success;
            }

            // اگر Guid هست → همیشه معتبر (چون null و Empty قبلاً چک شدن)
            if (value is Guid)
            {
                return ValidationResult.Success;
            }

            // نوع نامعتبر
            var name = validationContext.DisplayName ?? validationContext.MemberName;
            return new ValidationResult($"نوع {name} باید Guid یا string معتبر باشد.", new[] { validationContext.MemberName });
        }
    }

    /*
     نحوه استفاده از دستور
     /// <summary>
    /// شناسه کاربر ثبت کننده
    /// </summary>
    [Guid(required: true, ErrorMessage = "شناسه GUID کاربر ثبت کننده نامعتبر است.")]
    public Guid? GlobalCitizenId { get; set; } 
     */
}