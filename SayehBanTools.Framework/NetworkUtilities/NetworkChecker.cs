using Newtonsoft.Json;
using System.Net;
using System.Net.NetworkInformation;
using static SayehBanTools.Framework.Model.Entities.PublicModel;

namespace SayehBanTools.Framework.NetworkUtilities
{
    /// <summary>
    /// بررسی شبکه
    /// </summary>
    public class NetworkChecker
    {
        /// <summary>
        /// بررسی اتصال اینترنت با امکان تغییر آدرس تست
        /// </summary>
        /// <returns></returns>
        public bool CheckInternetConnection(string hostToPing)
        {
            try
            {
                using (var ping = new Ping())
                {
                    var result = ping.Send(hostToPing);
                    return result.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// بررسی اتصال اینترنت با امکان تغییر آدرس تست
        /// </summary>
        /// <returns></returns>
        public bool CheckInternetConnection()
        {
            try
            {
                using (var ping = new Ping())
                {
                    var result = ping.Send("www.google.com");
                    return result.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// دریافت آدرس IP محلی سیستم
        /// </summary>
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";
        }
        /// <summary>
        /// دریافت IP عمومی (با متد جایگزین، چون وای‌فای ممکن است مشکل‌ساز باشد)
        /// </summary>
        public static string GetPublicIPAddress()
        {
            try
            {
                using (var client = new WebClient())
                {
                    string json = client.DownloadString("https://ipwho.is/");

                    // Deserialize کامل JSON
                    var ipData = JsonConvert.DeserializeObject<IpResponse>(json);

                    return ipData?.ip ?? "127.0.0.1";
                }
            }
            catch
            {
                return "127.0.0.1";
            }
        }
    }
}
