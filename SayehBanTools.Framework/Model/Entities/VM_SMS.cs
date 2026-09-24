using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SayehBanTools.Framework.Model.Entities
{
    /// <summary>
    /// مدل پیامک
    /// </summary>
    public class VM_SMS
    {
        /// <summary>
        /// مقادیر پیش فرض
        /// </summary>
        public class DefaultValueParameters
        {
            /// <summary>
            /// آدرس پایه https://edge.ippanel.com/v1 API
            /// </summary>
            public string BaseUrl { get; set; } = "https://edge.ippanel.com/v1";
            /// <summary>
            /// لینک API
            /// </summary>
            public string Api { get; set; } = string.Empty;
            /// <summary>
            /// کلید دسترسی
            /// </summary>
            public string API_Key { get; set; } = string.Empty;
        }
        /// <summary>
        /// محاسبه هزینه پیامک
        /// </summary>
        public class Calculate_SMS_Price_Parameters : DefaultValueParameters
        {
            /// <summary>
            /// شماره پیش فرض ارسال کننده پیام 983000505
            /// </summary>
            public string number { get; set; } = "983000505";
            /// <summary>
            /// متن پیام که عملیات محاسبه روش انجام میشه
            /// </summary>
            public string message { get; set; } = string.Empty;
        }
        /// <summary>
        /// وب سرویس پارامتر های ارسال
        /// </summary>
        public class SendWebserviceSMSParameters : DefaultValueParameters
        {
            /// <summary>
            /// نوع ارسال پیامک webservice
            /// </summary>
            [JsonPropertyName("sending_type")]
            public string SendingType { get; set; } = "webservice";

            /// <summary>
            /// ارسال از شماره ارسال کننده +983000505
            /// </summary>
            [JsonPropertyName("from_number")]
            public string FromNumber { get; set; } = "+983000505";

            /// <summary>
            /// متن پیام ارسال
            /// </summary>
            [JsonPropertyName("message")]
            public string Message { get; set; } = string.Empty;

            /// <summary>
            /// زمان ارسال (فرمت: yyyy-MM-dd HH:mm:ss)
            /// </summary>
            [JsonPropertyName("send_time")]
            public DateTime SendTime { get; set; } = DateTime.Now.AddSeconds(30);

            /// <summary>
            /// پارامترهای JSON (داخلی - برای API)
            /// </summary>
            [JsonPropertyName("params")]
            public SMSParams Params { get; set; }

            /// <summary>
            /// Constructor برای راحتی
            /// </summary>
            public SendWebserviceSMSParameters()
            {
                Params = new SMSParams();
            }

            /// <summary>
            ///  Helper method برای تنظیم Recipients
            /// </summary>
            /// <param name="recipients"></param>
            public void SetRecipients(List<string> recipients)
            {
                Params.Recipients = recipients ?? new List<string>();
            }
        }
        /// <summary>
        /// پارامتر گیرنده ها
        /// </summary>
        public class SMSParams
        {
            /// <summary>
            /// گیرندگان
            /// </summary>
            [JsonPropertyName("recipients")]
            public List<string> Recipients { get; set; } 
        }
        /// <summary>
        /// ارسال پیامک به صورت فایل
        /// </summary>
        public class SendFileSMSParameters : DefaultValueParameters
        {
            /// <summary>
            /// نوع ارسال پیامک file
            /// </summary>
            public string sending_type { get; set; } = "file";

            /// <summary>
            /// ارسال از شماره ارسال کننده +983000505
            /// </summary>
            [JsonPropertyName("from_number")]
            public string from_number { get; set; } = "+983000505";

            /// <summary>
            /// متن پیام ارسال
            /// </summary>
            public string message { get; set; } = string.Empty;

            /// <summary>
            /// فایل شماره ها
            /// </summary>
            public IFormFile[] files { get; set; } = null;

            /// <summary>
            /// زمان ارسال (فرمت: yyyy-MM-dd HH:mm:ss)
            /// </summary>
            public DateTime send_time { get; set; } = DateTime.Now.AddSeconds(30);
            /// <summary>
            /// ارسال پارامترهای پیامک
            /// </summary>
            public SendFileSMSParameters()
            {
                files = Array.Empty<IFormFile>();
            }
        }
        /// <summary>
        /// ارسال پیامک به صورت کلمات کلیدی
        /// </summary>
        public class SendKeywordSMSParameters : DefaultValueParameters
        {
            /// <summary>
            /// نوع ارسال پیامک keyword
            /// </summary>
            public string sending_type { get; set; } = "keyword";
            /// <summary>
            /// ارسال از شماره ارسال کننده +983000505
            /// </summary>
            [JsonPropertyName("from_number")]
            public string from_number { get; set; } = "+983000505";
            /// <summary>
            /// متن پیام ارسال درود {ex_B} م۱ {ex_C}
            /// </summary>
            public string message { get; set; } = string.Empty;
            /// <summary>
            /// فایل شماره ها
            /// </summary>
            public IFormFile[] files { get; set; } = null;
            /// <summary>
            /// زمان ارسال (فرمت: yyyy-MM-dd HH:mm:ss)
            /// </summary>
            public DateTime send_time { get; set; } = DateTime.Now.AddSeconds(30);
            /// <summary>
            /// ارسال پارامترهای پیامک
            /// </summary>
            public SendKeywordSMSParameters()
            {
                files = Array.Empty<IFormFile>();
            }
        }
        /// <summary>
        /// ارسال پیامک به صورت پترن
        /// </summary>
        public class SendPatternSMSParameters : DefaultValueParameters
        {
            /// <summary>
            /// نوع ارسال پیامک pattern
            /// </summary>
            [JsonPropertyName("sending_type")]
            public string SendingType { get; set; } = "pattern";

            /// <summary>
            /// ارسال از شماره ارسال کننده +983000505
            /// </summary>
            [JsonPropertyName("from_number")]
            public string FromNumber { get; set; } = "+983000505";

            /// <summary>
            /// کد پترن
            /// </summary>
            [JsonPropertyName("code")]
            public string Code { get; set; } = string.Empty;

            /// <summary>
            /// لیست گیرندگان (آرایه string)
            /// </summary>
            [JsonPropertyName("recipients")]
            public List<string> Recipients { get; set; } = new List<string>();

            /// <summary>
            /// پارامترهای پترن (Dictionary اختیاری)
            /// </summary>
            [JsonPropertyName("params")]
            public Dictionary<string, string> Params { get; set; }

            /// <summary>
            /// زمان ارسال (فرمت: yyyy-MM-dd HH:mm:ss)
            /// </summary>
            [JsonPropertyName("send_time")]
            public DateTime send_time { get; set; } = DateTime.Now.AddSeconds(30);
            /// <summary>
            /// Helper method برای تنظیم Params
            /// </summary>
            /// <param name="parameters"></param>
            public void SetParams(Dictionary<string, string> parameters)
            {
                Params = parameters ?? new Dictionary<string, string>();
            }

            /// <summary>
            /// Helper method برای اضافه کردن پارامتر
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public void AddParam(string key, string value)
            {
                Params = new Dictionary<string, string>();
                Params[key] = value;
            }
        }
        /// <summary>
        /// ارسال پیامک Peer to Peer
        /// </summary>
        public class SendPeerToPeerSMSParameters : DefaultValueParameters
        {
            /// <summary>
            /// نوع ارسال پیامک peer_to_peer
            /// </summary>
            [JsonPropertyName("sending_type")]
            public string SendingType { get; set; } = "peer_to_peer";

            /// <summary>
            /// شماره ارسال کننده +983000505
            /// </summary>
            [JsonPropertyName("from_number")]
            public string FromNumber { get; set; } = "+983000505";

            /// <summary>
            /// آرایه پیام‌ها و گیرندگان
            /// </summary>
            [JsonPropertyName("params")]
            public List<PeerToPeerMessage> Params { get; set; } 
            /// <summary>
            /// ارسال به صورت نظیر به نظیر
            /// </summary>
            public SendPeerToPeerSMSParameters()
            {
                Params = new List<PeerToPeerMessage>();
            }

            /// Helper method برای اضافه کردن پیام
            public void AddMessage(List<string> recipients, string message)
            {
                Params.Add(new PeerToPeerMessage
                {
                    Recipients = recipients ?? new List<string>(),
                    Message = message ?? string.Empty
                });
            }
        }
        /// <summary>
        /// یک پیام Peer to Peer
        /// </summary>
        public class PeerToPeerMessage
        {
            /// <summary>
            /// لیست گیرندگان
            /// </summary>
            [JsonPropertyName("recipients")]
            public List<string> Recipients { get; set; } = new List<string>();

            /// <summary>
            /// متن پیام
            /// </summary>
            [JsonPropertyName("message")]
            public string Message { get; set; } = string.Empty;
        }
        /// <summary>
        /// ارسال نظیر به نظیر توسط فایل
        /// </summary>
        public class SendPeertoPeerbyFileParameters : DefaultValueParameters
        {
            /// <summary>
            /// نوع ارسال پیامک peer_to_peer_file
            /// </summary>
            [JsonPropertyName("sending_type")]
            public string SendingType { get; set; } = "peer_to_peer_file";
            /// <summary>
            /// شماره ارسال کننده +983000505
            /// </summary>
            [JsonPropertyName("from_number")]
            public string FromNumber { get; set; } = "+983000505";
            /// <summary>
            /// زمان ارسال (فرمت: yyyy-MM-dd HH:mm:ss)
            /// </summary>
            public DateTime send_time { get; set; } = DateTime.Now.AddSeconds(30);
            /// <summary>
            /// فایل شماره ها
            /// </summary>
            public IFormFile[] files { get; set; } = null;
        }
    }
}
