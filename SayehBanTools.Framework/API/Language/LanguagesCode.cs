using RestSharp;
using SayehBanTools.Framework.Utilities.Constants;
using System;

namespace SayehBanTools.Framework.API.Language
{
    /// <summary>
    /// کلاس برای دریافت کدهای زبان از API.
    /// </summary>
    public class LanguagesCode
    {
        /// <summary>
        /// ایجاد RestClient و RestRequest برای درخواست به API.
        /// </summary>
        /// <param name="apiLink">آدرس API (اختیاری، پیش‌فرض: http://localhost:90).</param>
        /// <param name="apiAddress">مسیر API (اختیاری، پیش‌فرض: api/LanguagesCode/LanguagesCodeGetAll).</param>
        /// <param name="client">یک نمونه RestClient اختیاری برای اهداف تستی.</param>
        /// <returns>یک تاپل شامل RestClient و RestRequest.</returns>
        public static (IRestClient Client, RestRequest Request) CreateApiRequest(
            string apiLink = null,
            string apiAddress = null,
            IRestClient client = null)
        {
            // استفاده از مقدار پیش‌فرض اگر apiLink null باشد
            client = new RestClient(apiLink ?? ApiConstants.DefaultApiLink, configureRestClient: options =>
            {
                options.Timeout = TimeSpan.FromSeconds(10);
            });

            // استفاده از مقدار پیش‌فرض اگر apiAddress null باشد
            var request = new RestRequest(apiAddress ?? ApiConstants.ApiAddressLanguageCodeList, Method.Get);

            return (client, request);
        }
    }
}