using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
namespace SayehBanTools.Framework.Sender
{
    /// <summary>
    /// این کلاس برای ارسال پیامک به صورت پترن استفاده میشود
    /// </summary>
    public class SMS_System
    {
        /// <summary>
        /// ارسال به صورت پترن
        /// </summary>
        public static async Task<(HttpResponseMessage Response, string ResponseContent)> SendPatternAsync(string APILink, string APIKey, Dictionary<string, object> data, string patternCode, string from, string to, DateTime DateTimeSender)
        {
            string formattedDateTime = GetDateTimeUTCWithOffset(DateTimeSender, TimeSpan.FromSeconds(30));
            // Create JSON payload
            string jsonPayload = JsonConvert.SerializeObject(new
            {
                code = patternCode,
                sender = from,
                time = formattedDateTime,
                recipient = to,
                variable = data,
            });

            try
            {
                using (var client = new HttpClient())
                {
                    // Set up the request
                    var request = new HttpRequestMessage(HttpMethod.Post, APILink ?? "https://api2.ippanel.com/api/v1/sms/pattern/normal/send")
                    {
                        Content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json")
                    };
                    request.Headers.Add("apikey", APIKey);

                    // Send the request
                    HttpResponseMessage response = await client.SendAsync(request);

                    // Read the response content
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return (response, responseContent);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Error sending SMS: " + ex.Message);
                return (new HttpResponseMessage(HttpStatusCode.InternalServerError), ex.Message); // Return a default HttpResponseMessage with error message
            }
        }
        /// <summary>
        /// ارسال به صورت معمولی
        /// </summary>
        /// <param name="APILink"></param>
        /// <param name="APIKey"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="Message"></param>
        /// <param name="DateTimeSender"></param>
        /// <returns></returns>
        public static async Task<(HttpResponseMessage Response, string ResponseContent)> SendNormalSingleAsync(string APILink, string APIKey, string from, string[] to, string Message, DateTime? DateTimeSender)
        {
            string formattedDateTime = GetDateTimeUTCWithOffset(DateTimeSender, TimeSpan.FromSeconds(30));
            // Create JSON payload
            string jsonPayload = JsonConvert.SerializeObject(new
            {
                sender = from,
                recipient = to,
                time = formattedDateTime,
                message = Message,
            });

            try
            {
                using (var client = new HttpClient())
                {
                    // Set up the request
                    var request = new HttpRequestMessage(HttpMethod.Post, APILink ?? "https://api2.ippanel.com/api/v1/sms/send/webservice/single")
                    {
                        Content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json")
                    };
                    request.Headers.Add("apikey", APIKey);

                    // Send the request
                    HttpResponseMessage response = await client.SendAsync(request);

                    // Read the response content
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return (response, responseContent);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Error sending SMS: " + ex.Message);
                return (new HttpResponseMessage(HttpStatusCode.InternalServerError), ex.Message); // Return a default HttpResponseMessage with error message
            }
        }
        /// <summary>
        /// ارسال به صورت فایل
        /// </summary>
        /// <param name="APILink"></param>
        /// <param name="APIKey"></param>
        /// <param name="FromNumber"></param>
        /// <param name="To"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static async Task<string> SendNormalFileAsync(string APILink, string APIKey, string FromNumber, IFormFile To,string Message)
        {
            try
            {
                if (To == null || To.Length == 0)
                {
                    throw new ArgumentException("File parameter cannot be null or empty.");
                }

                using (var client = new HttpClient())
                {
                    // Create the HttpContent for the form to be posted.
                    var requestContent = new MultipartFormDataContent();
                    // Updated line to handle potential null reference for 'FromNumber'
                    requestContent.Add(new StringContent(FromNumber ?? string.Empty), "sender");
                    requestContent.Add(new StringContent(Message?? string.Empty), "message");
                    requestContent.Add(new StreamContent(To.OpenReadStream()), "file", To.FileName);
                    requestContent.Add(new StringContent("ارسال به فایل"), "description[summary]");
                    requestContent.Add(new StringContent("1"), "description[count_recipient]");

                    // Set additional headers.

                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("apikey", APIKey);

                    // Send the POST request.
                    HttpResponseMessage response = await client.PostAsync(APILink ?? "https://api2.ippanel.com/api/v1/sms/send/panel/file", requestContent);

                    // Get the response content.
                    HttpContent responseContent = response.Content;

                    // Read the response.
                    string result = await responseContent.ReadAsStringAsync();
                    return result;
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine("Error sending SMS: " + ex.Message);
                return (ex.Message); // Return empty content and error status code}
            }
        }
        /// <summary>
        /// ارسال به صورت معمولی
        /// </summary>
        /// <param name="DateTimeSender"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private static string GetDateTimeUTCWithOffset(DateTime? DateTimeSender, TimeSpan offset)
        {
            DateTime dateTime = DateTimeSender ?? DateTime.UtcNow + offset;
            var formattedDateTime = dateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture);
            return formattedDateTime;
        }

        /// <summary>
        /// این کد یک تابع C# به نام SendPeerToPeerAsync است که به صورت همزمان برای ارسال پیامک‌های چند گیرنده به یک API با روش POST عمل می‌کند.
        /// </summary>
        /// <param name="APILink"></param>
        /// <param name="APIKey"></param>
        /// <param name="Recipients"></param>
        /// <param name="FromNumber"></param>
        /// <param name="Messages"></param>
        /// <returns></returns>
        public static async Task<(string ResponseContent, int StatusCode)> SendPeerToPeerAsync(string APILink, string APIKey, List<List<string>> Recipients, string FromNumber, string[] Messages)
        {
            try
            {
                // Create JSON payload
                var jsonPayload = new Dictionary<string, object>()
                {
                    ["recipient"] = Recipients,
                    ["sending_type"] = "webservice",
                    ["sender"] = FromNumber,
                    ["message"] = Messages,
                };

                string jsonString = JsonConvert.SerializeObject(jsonPayload);

                using (var client = new HttpClient())
                {
                    // Set up the request
                    var request = new HttpRequestMessage(HttpMethod.Post, APILink ?? "https://api2.ippanel.com/api/v1/sms/send/webservice/peer-to-peer")
                    {
                        Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
                    };
                    request.Headers.Add("Accept", "application/json");
                    request.Headers.Add("apikey", APIKey);

                    // Send the request
                    HttpResponseMessage response = await client.SendAsync(request);

                    // Read the response content
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return (responseContent, (int)response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Error sending SMS: " + ex.Message);
                return ("", -1); // Return empty content and error status code
            }
        }
        /// <summary>
        /// ارسال پیامک به صورت Peer To Peer توسط فایل
        /// </summary>
        /// <param name="APILink"></param>
        /// <param name="APIKey"></param>
        /// <param name="FromNumber"></param>
        /// <param name="File"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static async Task<string> SendPeerToPeerByFileAsync(string APILink, string APIKey, string FromNumber, IFormFile File)
        {
            try
            {
                if (File == null || File.Length == 0)
                {
                    throw new ArgumentException("File parameter cannot be null or empty.");
                }

                using (var client = new HttpClient())
                {
                    // Create the HttpContent for the form to be posted.
                    var requestContent = new MultipartFormDataContent();
                    requestContent.Add(new StringContent(FromNumber), "sender");
                    requestContent.Add(new StreamContent(File.OpenReadStream()), "file", File.FileName);
                    requestContent.Add(new StringContent("ارسال به فایل"), "description[summary]");
                    requestContent.Add(new StringContent("1"), "description[count_recipient]");

                    // Set additional headers.

                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("apikey", APIKey);

                    // Send the POST request.
                    HttpResponseMessage response = await client.PostAsync(APILink ?? "https://api2.ippanel.com/api/v1/sms/send/panel/peer-to-peer-by-file", requestContent);

                    // Get the response content.
                    HttpContent responseContent = response.Content;

                    // Read the response.
                    string result = await responseContent.ReadAsStringAsync();
                    return result;
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine("Error sending SMS: " + ex.Message);
                return (ex.Message); // Return empty content and error status code}
            }
        }
        /// <summary>
        /// نمایش هزینه پانل
        /// </summary>
        /// <param name="APILink"></param>
        /// <param name="APIKey"></param>
        /// <returns></returns>
        public static async Task<(string ResponseContent, int StatusCode)> GetCreditAsync(string APILink, string APIKey)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Set up the request
                    var request = new HttpRequestMessage(HttpMethod.Get, APILink ?? "https://api2.ippanel.com/api/v1/sms/accounting/credit/show");
                    request.Headers.Add("Accept", "application/json");
                    request.Headers.Add("apikey", APIKey);

                    // Send the request
                    HttpResponseMessage response = await client.SendAsync(request);

                    // Read the response content
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return (responseContent, (int)response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Error getting credit: " + ex.Message);
                return ("", -1); // Return empty content and error status code
            }
        }
        /// <summary>
        /// نمایش پیام های ارسال شده   
        /// /// </summary>
        /// <param name="APILink"></param>
        /// <param name="APIKey"></param>
        /// <param name="page"></param>
        /// <param name="per_page"></param>
        /// <returns></returns>
        public static async Task<(string ResponseContent, int StatusCode)> GetSendListAsync(string APILink, string APIKey, int page, int per_page)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Construct the URL with query parameters
                    string url = APILink ?? $"https://api2.ippanel.com/api/v1/sms/message/all?page={page}&per_page={per_page}";

                    // Set up the request
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Headers.Add("Accept", "application/json");
                    request.Headers.Add("apikey", APIKey);

                    // Send the request
                    HttpResponseMessage response = await client.SendAsync(request);

                    // Read the response content
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return (responseContent, (int)response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Error getting send list: " + ex.Message);
                return ("", -1); // Return empty content and error status code
            }
        }
    }
    /*
    How Use Source
    https://github.com/SayyehBan/SendSMSSayyehBan/blob/master/SendSMSSayyehBan/Controllers/SMS_SayyehBanController.cs
     */
}
