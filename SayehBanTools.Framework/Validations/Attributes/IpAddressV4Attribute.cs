using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.RegularExpressions;

namespace SayehBanTools.Framework.Validations.Attributes
{
    /// <summary>
    /// Attribute برای اعتبار‌سنجی آدرس IPv4 با قابلیت اختیاری/اجباری
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class IpAddressV4Attribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "آدرس IP نامعتبر است. باید یک آدرس IPv4 معتبر (مانند 192.168.1.1) باشد.";

        /// <summary>
        /// آیا فیلد اجباری است؟
        /// </summary>
        public bool Required { get; }

        /// <summary>
        /// سازنده پیش‌فرض: اجباری (Required = true)
        /// </summary>
        public IpAddressV4Attribute() : this(true)
        {
        }

        /// <summary>
        /// سازنده با کنترل اجباری بودن
        /// </summary>
        /// <param name="required">اگر true باشد، IP اجباری است</param>
        public IpAddressV4Attribute(bool required) : base(DefaultErrorMessage)
        {
            Required = required;
        }

        /// <summary>
        /// سازنده با پیام خطای سفارشی (اجباری)
        /// </summary>
        public IpAddressV4Attribute(string errorMessage) : base(errorMessage)
        {
            Required = true;
        }

        /// <summary>
        /// سازنده با کنترل اجباری بودن + پیام خطای سفارشی
        /// </summary>
        public IpAddressV4Attribute(bool required, string errorMessage) : base(errorMessage)
        {
            Required = required;
        }
        /// <summary>
        /// اعتبار سنجی آدرس IPv4
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
            // بررسی فرمت با regex
            if (!Regex.IsMatch(ipAddress, @"^(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3})$"))
            {
                return new ValidationResult($"{ErrorMessage ?? DefaultErrorMessage} مقدار واردشده: '{ipAddress}'");
            }

            // بررسی با IPAddress.TryParse
            if (!IPAddress.TryParse(ipAddress, out IPAddress parsedIp) ||
                parsedIp.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return new ValidationResult($"{ErrorMessage ?? DefaultErrorMessage} مقدار واردشده: '{ipAddress}'");
            }

            // بررسی محدوده 0-255
            string[] octets = ipAddress.Split('.');
            foreach (var octet in octets)
            {
                if (!int.TryParse(octet, out int number) || number < 0 || number > 255)
                {
                    return new ValidationResult($"{ErrorMessage ?? DefaultErrorMessage} مقدار واردشده: '{ipAddress}'");
                }
            }

            return ValidationResult.Success;
        }
    }
}