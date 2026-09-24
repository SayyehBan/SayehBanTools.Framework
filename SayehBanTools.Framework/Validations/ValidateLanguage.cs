using RestSharp;
using SayehBanTools.Framework.API.Language;
using SayehBanTools.Framework.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SayehBanTools.Framework.Validations
{
    /// <summary>
    /// کلاس بررسی صحت فرمت کد زبان.
    /// </summary>
    public class ValidateLanguage
    {
        /// <summary>
        /// بررسی صحت فرمت کد زبان.
        /// </summary>
        /// <param name="languageCode">کد زبان برای بررسی (باید دو کاراکتر حروفی باشد).</param>
        /// <param name="message">پیام خطا اختیاری برای استثنا.</param>
        /// <exception cref="ArgumentException">در صورتی که کد زبان معتبر نباشد.</exception>
        public static void ValidateLanguageCodeFormat(string languageCode, string message = null)
        {
            if (string.IsNullOrEmpty(languageCode) || languageCode.Length != 2 ||
                !Regex.IsMatch(languageCode, @"^[a-zA-Z]{2}$"))
                throw new ArgumentException(message ?? "کد زبان باید دقیقاً دو کاراکتر انگلیسی حروفی باشد.", nameof(languageCode));
        }

        /// <summary>
        /// اعتبارسنجی کد زبان از طریق درخواست به API.
        /// </summary>
        /// <param name="apiLink">آدرس API برای دریافت کدهای زبان (اختیاری).</param>
        /// <param name="apiAddress">مسیر API برای دریافت کدهای زبان (اختیاری).</param>
        /// <param name="languageCode">کد زبان مورد نظر برای اعتبارسنجی.</param>
        /// <param name="client">یک نمونه RestClient اختیاری برای اهداف تستی.</param>
        /// <returns>یک Task که نشان‌دهنده تکمیل عملیات است.</returns>
        /// <exception cref="InvalidOperationException">در صورت خطا در دریافت داده از API.</exception>
        /// <exception cref="ArgumentException">در صورتی که کد زبان معتبر نباشد.</exception>
        public static async Task ValidateLanguageCodeViaApiAsync(
            string apiLink,
            string apiAddress,
            string languageCode,
            IRestClient client = null)
        {
            // دریافت RestClient و RestRequest از متد کمکی
            var (restClient, request) = LanguagesCode.CreateApiRequest(apiLink, apiAddress, client);

            try
            {
                var response = await restClient.ExecuteAsync<List<VM_Language.LanguageCodeList>>(request);

                // بررسی موفقیت درخواست و وجود داده
                if (!response.IsSuccessful || response.Data == null || !response.Data.Any())
                {
                    string errorMessage = $"خطا در دریافت داده از API کدهای زبان. کد وضعیت: {response.StatusCode}";
                    if (!string.IsNullOrEmpty(response.ErrorMessage))
                    {
                        errorMessage += $" - پیام خطا: {response.ErrorMessage}";
                    }
                    throw new InvalidOperationException(errorMessage);
                }

                // اعتبارسنجی کد زبان
                if (!response.Data.Any(l => l.LanguageCode != null &&
                    l.LanguageCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new ArgumentException("کد زبان معتبر نیست.", nameof(languageCode));
                }
            }
            catch (ArgumentException)
            {
                throw; // استثنای ArgumentException را مستقیماً پرتاب کن
            }
            catch (Exception ex)
            {
                // مدیریت خطاهای غیرمنتظره
                string errorMessage = $"خطا در دریافت داده از API: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $" - Inner Exception: {ex.InnerException.Message}";
                }
                throw new InvalidOperationException(errorMessage, ex);
            }
        }
    }
}