using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.RegularExpressions;

namespace SayehBanTools.Framework.Validations.Attributes
{
    /// <summary>
    /// Attribute برای اعتبار‌سنجی آدرس IPv6 با قابلیت اختیاری/اجباری
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class IpAddressV6Attribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "آدرس IP نامعتبر است. باید یک آدرس IPv6 معتبر (مانند 2001:0db8:85a3:0000:0000:8a2e:0370:7334) باشد.";

        /// <summary>
        /// آیا فیلد اجباری است؟
        /// </summary>
        public bool Required { get; }

        /// <summary>
        /// سازنده پیش‌فرض: اجباری (Required = true)
        /// </summary>
        public IpAddressV6Attribute() : this(true)
        {
        }

        /// <summary>
        /// سازنده با کنترل اجباری بودن
        /// </summary>
        /// <param name="required">اگر true باشد، IPv6 اجباری است</param>
        public IpAddressV6Attribute(bool required) : base(DefaultErrorMessage)
        {
            Required = required;
        }

        /// <summary>
        /// سازنده با پیام خطای سفارشی (اجباری)
        /// </summary>
        public IpAddressV6Attribute(string errorMessage) : base(errorMessage)
        {
            Required = true;
        }

        /// <summary>
        /// سازنده با کنترل اجباری بودن + پیام خطای سفارشی
        /// </summary>
        public IpAddressV6Attribute(bool required, string errorMessage) : base(errorMessage)
        {
            Required = required;
        }
        /// <summary>
        /// اعتبار سنجی آدرس IPv6
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // اگر مقدار null باشد
            if (value == null)
            {
                return Required
                    ? new ValidationResult(ErrorMessage ?? DefaultErrorMessage)
                    : ValidationResult.Success;
            }

            // اگر string نباشد
            if (!(value is string ipAddress))
            {
                return new ValidationResult("آدرس IP باید یک رشته باشد.");
            }

            // اگر خالی یا فقط فاصله باشد
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                return Required
                    ? new ValidationResult(ErrorMessage ?? DefaultErrorMessage)
                    : ValidationResult.Success;
            }

            // حالا که مقدار داریم، حتماً باید معتبر باشد (حتی اگر Required=false)

            // بررسی فرمت IPv6 با regex (پشتیبانی از :: و فرمت کامل)
            if (!Regex.IsMatch(ipAddress, @"^(([0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:(:[0-9a-fA-F]{1,4}){1,6}|:((:[0-9a-fA-F]{1,4}){1,7}|:))$", RegexOptions.IgnoreCase))
            {
                return new ValidationResult($"{ErrorMessage ?? DefaultErrorMessage} مقدار واردشده: '{ipAddress}'");
            }

            // بررسی با IPAddress.TryParse
            if (!IPAddress.TryParse(ipAddress, out IPAddress parsedIp) ||
                parsedIp.AddressFamily != System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                return new ValidationResult($"{ErrorMessage ?? DefaultErrorMessage} مقدار واردشده: '{ipAddress}'");
            }

            return ValidationResult.Success;
        }
    }
}