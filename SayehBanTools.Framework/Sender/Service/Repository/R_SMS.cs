using RestSharp;
using SayehBanTools.Framework.Sender.Service.Interface;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static SayehBanTools.Framework.Model.Entities.VM_SMS;

namespace SayehBanTools.Framework.Sender.Service.Repository
{
    /// <summary>
    /// ریپازیتوری پیامک - سورس کمکی
    /// </summary>
    public class R_SMS : I_SMS
    {
        /// <summary>
        /// محاسبه هزینه پیامک
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<object> Calculate_SMS_PriceAsync(Calculate_SMS_Price_Parameters model)
        {
            var options = new RestClientOptions(model.BaseUrl);
            var client = new RestClient(options);
            var request = new RestRequest($"{model.Api}?number={model.number}&message={model.message}", Method.Post);
            request.AddHeader("Authorization", model.API_Key);

            RestResponse response = await client.ExecuteAsync(request);

            if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
            {
                var jsonDocument = JsonDocument.Parse(response.Content);
                var result = JsonSerializer.Deserialize<object>(jsonDocument.RootElement.GetRawText());

                // Null check اضافه شده
                return result ?? new { error = "خطا در deserialize اطلاعات" };
            }

            return new { error = "خطا در دریافت اطلاعات" };
        }

        /// <summary>
        /// دریافت موجودی پیامک - فقط JSON اصلی
        /// </summary>
        /// <param name="model">تنظیمات API کامل از درخواست</param>
        /// <returns>لیست اطلاعات اعتبارات</returns>
        public async Task<object> GetCreditAsync(DefaultValueParameters model)
        {
            var options = new RestClientOptions(model.BaseUrl);
            var client = new RestClient(options);
            var request = new RestRequest(model.Api, Method.Get);
            request.AddHeader("Authorization", model.API_Key);

            RestResponse response = await client.ExecuteAsync(request);

            if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
            {
                var jsonDocument = JsonDocument.Parse(response.Content);
                var result = JsonSerializer.Deserialize<object>(jsonDocument.RootElement.GetRawText());

                // Null check اضافه شده
                return result ?? new { error = "خطا در deserialize اطلاعات" };
            }

            return new { error = "خطا در دریافت اطلاعات" };
        }
        /// <summary>
        /// ارسال پیامک توسط فایل
        /// </summary>
        /// <param name="model">پارامترهای ارسال فایل</param>
        /// <returns>نتیجه ارسال</returns>
        public async Task<object> SendFileSMSAsync(SendFileSMSParameters model)
        {
            try
            {
                if (model.files == null || model.files.Length == 0)
                    return new { error = "هیچ فایلی انتخاب نشده است" };

                var options = new RestClientOptions(model.BaseUrl);
                var client = new RestClient(options);
                var request = new RestRequest(model.Api, Method.Post);

                request.AddHeader("Authorization", model.API_Key);
                request.AlwaysMultipartFormData = true;

                // اضافه کردن parameters
                request.AddParameter("sending_type", model.sending_type ?? "file");
                request.AddParameter("from_number", model.from_number ?? "+983000505");
                request.AddParameter("message", model.message ?? string.Empty);
                request.AddParameter("send_time", model.send_time.ToString("yyyy-MM-dd HH:mm:ss"));

                // اضافه کردن فایل‌ها
                if (model.files != null && model.files.Length > 0)
                {
                    foreach (var file in model.files)
                    {
                        if (file != null && file.Length > 0)
                        {
                            // خواندن کامل فایل (حل CA2022)
                             var stream = file.OpenReadStream();
                             var memoryStream = new MemoryStream();

                            // خواندن کامل با loop (حل CA2022)
                            var buffer = new byte[8192]; // 8KB buffer
                            int bytesRead;
                            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await memoryStream.WriteAsync(buffer, 0, bytesRead);
                            }

                            var fileBytes = memoryStream.ToArray();

                            // اضافه کردن فایل با RestSharp (حل CS1061)
                            request.AddFile("files[]", fileBytes, file.FileName ?? "numbers.txt", file.ContentType ?? "text/plain");
                        }
                    }
                }

                RestResponse response = await client.ExecuteAsync(request);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var jsonDocument = JsonDocument.Parse(response.Content);
                    var result = JsonSerializer.Deserialize<object>(jsonDocument.RootElement.GetRawText());

                    return result ?? new { error = "خطا در deserialize اطلاعات" };
                }

                return new { error = $"خطا در ارسال پیامک: {response.StatusCode} - {response.StatusDescription}" };
            }
            catch (Exception ex)
            {
                return new { error = $"خطای عمومی: {ex.Message}" };
            }
        }
        /// <summary>
        /// ارسال پیامک توسط Keyword
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<object> SendKeywordSMSAsync(SendKeywordSMSParameters model)
        {
            try
            {
                if (model.files == null || model.files.Length == 0)
                    return new { error = "هیچ فایلی انتخاب نشده است" };

                var options = new RestClientOptions(model.BaseUrl);
                var client = new RestClient(options);
                var request = new RestRequest(model.Api, Method.Post);

                request.AddHeader("Authorization", model.API_Key);
                request.AlwaysMultipartFormData = true;

                // اضافه کردن parameters
                request.AddParameter("sending_type", model.sending_type ?? "keyword");
                request.AddParameter("from_number", model.from_number ?? "+983000505");
                request.AddParameter("message", model.message ?? string.Empty);
                request.AddParameter("send_time", model.send_time.ToString("yyyy-MM-dd HH:mm:ss"));

                // اضافه کردن فایل‌ها
                if (model.files != null && model.files.Length > 0)
                {
                    foreach (var file in model.files)
                    {
                        if (file != null && file.Length > 0)
                        {
                            // خواندن کامل فایل (حل CA2022)
                             var stream = file.OpenReadStream();
                             var memoryStream = new MemoryStream();

                            // خواندن کامل با loop (حل CA2022)
                            var buffer = new byte[8192]; // 8KB buffer
                            int bytesRead;
                            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await memoryStream.WriteAsync(buffer, 0, bytesRead);
                            }

                            var fileBytes = memoryStream.ToArray();

                            // اضافه کردن فایل با RestSharp (حل CS1061)
                            request.AddFile("files[]", fileBytes, file.FileName ?? "numbers.txt", file.ContentType ?? "text/plain");
                        }
                    }
                }

                RestResponse response = await client.ExecuteAsync(request);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var jsonDocument = JsonDocument.Parse(response.Content);
                    var result = JsonSerializer.Deserialize<object>(jsonDocument.RootElement.GetRawText());

                    return result ?? new { error = "خطا در deserialize اطلاعات" };
                }

                return new { error = $"خطا در ارسال پیامک: {response.StatusCode} - {response.StatusDescription}" };
            }
            catch (Exception ex)
            {
                return new { error = $"خطای عمومی: {ex.Message}" };
            }
        }

        /// <summary>
        /// ارسال پیامک از طریق WebService
        /// </summary>
        /// <param name="model">پارامترهای کامل ارسال</param>
        /// <returns>نتیجه ارسال</returns>
        public async Task<object> SendWebserviceSMSAsync(SendWebserviceSMSParameters model)
        {
            try
            {

                var options = new RestClientOptions(model.BaseUrl);
                var client = new RestClient(options);
                var request = new RestRequest(model.Api, Method.Post);
                request.AddHeader("Authorization", model.API_Key);

                // تنظیم JSON options برای فارسی و فرمت صحیح
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = false,
                    // تنظیم فرمت DateTime
                    Converters = { new CustomDateTimeConverter() }
                };

                // Serialize کردن مدل کامل
                var jsonContent = JsonSerializer.Serialize(model, jsonOptions);
                request.AddStringBody(jsonContent, ContentType.Json);

                RestResponse response = await client.ExecuteAsync(request);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var jsonDocument = JsonDocument.Parse(response.Content);
                    var result = JsonSerializer.Deserialize<object>(jsonDocument.RootElement.GetRawText());

                    return result ?? new { error = "خطا در deserialize اطلاعات" };
                }

                return new { error = $"خطا در ارسال پیامک: {response.StatusCode} - {response.StatusDescription}" };
            }
            catch (JsonException ex)
            {
                return new { error = $"خطا در پردازش JSON: {ex.Message}" };
            }
            catch (Exception ex)
            {
                return new { error = $"خطای عمومی: {ex.Message}" };
            }
        }
        /// <summary>
        /// ارسال پیامک پترن
        /// </summary>
        /// <param name="model">پارامترهای ارسال پترن</param>
        /// <returns>نتیجه ارسال</returns>
        public async Task<object> SendPatternSMSAsync(SendPatternSMSParameters model)
        {
            try
            {
                // اعتبارسنجی
                if (string.IsNullOrEmpty(model.Code))
                    return new { error = "کد پترن الزامی است" };

                if (!model.Recipients.Any())
                    return new { error = "حداقل یک گیرنده الزامی است" };

                var options = new RestClientOptions(model.BaseUrl);
                var client = new RestClient(options);
                var request = new RestRequest(model.Api, Method.Post);
                request.AddHeader("Authorization", model.API_Key);

                // تنظیم JSON options برای فارسی
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = false,
                    // تنظیم فرمت DateTime
                    Converters = { new CustomDateTimeConverter() }
                };

                // Serialize کردن مدل
                var jsonContent = JsonSerializer.Serialize(model, jsonOptions);
                request.AddStringBody(jsonContent, ContentType.Json);

                RestResponse response = await client.ExecuteAsync(request);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var jsonDocument = JsonDocument.Parse(response.Content);
                    var result = JsonSerializer.Deserialize<object>(jsonDocument.RootElement.GetRawText());

                    return result ?? new { error = "خطا در deserialize اطلاعات" };
                }

                return new { error = $"خطا در ارسال پیامک پترن: {response.StatusCode} - {response.StatusDescription}" };
            }
            catch (JsonException ex)
            {
                return new { error = $"خطا در پردازش JSON: {ex.Message}" };
            }
            catch (Exception ex)
            {
                return new { error = $"خطای عمومی: {ex.Message}" };
            }
        }
        /// <summary>
        /// ارسال پیامک Peer to Peer
        /// </summary>
        /// <param name="model">پارامترهای ارسال Peer to Peer</param>
        /// <returns>نتیجه ارسال</returns>
        public async Task<object> SendPeerToPeerSMSAsync(SendPeerToPeerSMSParameters model)
        {
            try
            {
                // اعتبارسنجی
                if (!model.Params.Any())
                    return new { error = "حداقل یک پیام الزامی است" };

                foreach (var param in model.Params)
                {
                    if (!param.Recipients.Any())
                        return new { error = "هر پیام باید حداقل یک گیرنده داشته باشد" };

                    if (string.IsNullOrEmpty(param.Message))
                        return new { error = "متن پیام الزامی است" };
                }

                var options = new RestClientOptions(model.BaseUrl);
                var client = new RestClient(options);
                var request = new RestRequest(model.Api, Method.Post);
                request.AddHeader("Authorization", model.API_Key);

                // تنظیم JSON options برای فارسی
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = false
                };

                // Serialize کردن مدل
                var jsonContent = JsonSerializer.Serialize(model, jsonOptions);
                request.AddStringBody(jsonContent, ContentType.Json);

                RestResponse response = await client.ExecuteAsync(request);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var jsonDocument = JsonDocument.Parse(response.Content);
                    var result = JsonSerializer.Deserialize<object>(jsonDocument.RootElement.GetRawText());

                    return result ?? new { error = "خطا در deserialize اطلاعات" };
                }

                return new { error = $"خطا در ارسال پیامک Peer to Peer: {response.StatusCode} - {response.StatusDescription}" };
            }
            catch (JsonException ex)
            {
                return new { error = $"خطا در پردازش JSON: {ex.Message}" };
            }
            catch (Exception ex)
            {
                return new { error = $"خطای عمومی: {ex.Message}" };
            }
        }
        /// <summary>
        /// ارسال نظیر به نظیر توسط فایل
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<object> SendPeertoPeerbyFile(SendPeertoPeerbyFileParameters model)
        {
            try
            {
                if (model.files == null || model.files.Length == 0)
                    return new { error = "هیچ فایلی انتخاب نشده است" };

                var options = new RestClientOptions(model.BaseUrl);
                var client = new RestClient(options);
                var request = new RestRequest(model.Api, Method.Post);

                request.AddHeader("Authorization", model.API_Key);
                request.AlwaysMultipartFormData = true;

                // اضافه کردن parameters
                request.AddParameter("sending_type", model.SendingType ?? "peer_to_peer_file");
                request.AddParameter("from_number", model.FromNumber ?? "+983000505");
                request.AddParameter("send_time", model.send_time.ToString("yyyy-MM-dd HH:mm:ss"));

                // اضافه کردن فایل‌ها
                if (model.files != null && model.files.Length > 0)
                {
                    foreach (var file in model.files)
                    {
                        if (file != null && file.Length > 0)
                        {
                            // خواندن کامل فایل (حل CA2022)
                             var stream = file.OpenReadStream();
                             var memoryStream = new MemoryStream();

                            // خواندن کامل با loop (حل CA2022)
                            var buffer = new byte[8192]; // 8KB buffer
                            int bytesRead;
                            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await memoryStream.WriteAsync(buffer, 0, bytesRead);
                            }

                            var fileBytes = memoryStream.ToArray();

                            // اضافه کردن فایل با RestSharp (حل CS1061)
                            request.AddFile("files[]", fileBytes, file.FileName ?? "numbers.txt", file.ContentType ?? "text/plain");
                        }
                    }
                }

                RestResponse response = await client.ExecuteAsync(request);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var jsonDocument = JsonDocument.Parse(response.Content);
                    var result = JsonSerializer.Deserialize<object>(jsonDocument.RootElement.GetRawText());

                    return result ?? new { error = "خطا در deserialize اطلاعات" };
                }

                return new { error = $"خطا در ارسال پیامک: {response.StatusCode} - {response.StatusDescription}" };
            }
            catch (Exception ex)
            {
                return new { error = $"خطای عمومی: {ex.Message}" };
            }
        }
    }
    /// <summary>
    /// سفارشی تبدیل تاریخ
    /// </summary>
    public class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        /// <summary>
        /// خواندن
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="JsonException"></exception>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (DateTime.TryParse(value, out var dt))
            {
                return dt;
            }
            throw new JsonException($"Unable to convert \"{value}\" to DateTime.");
        }
        /// <summary>
        /// نوشتن
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // فرمت مورد نظر API: yyyy-MM-dd HH:mm:ss
            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}