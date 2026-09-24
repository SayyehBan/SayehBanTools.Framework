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
                using (var ping = new System.Net.NetworkInformation.Ping())
                {
                    var result = ping.Send(hostToPing);
                    return result.Status == System.Net.NetworkInformation.IPStatus.Success;
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
                using (var ping = new System.Net.NetworkInformation.Ping())
                {
                    var result = ping.Send("www.google.com");
                    return result.Status == System.Net.NetworkInformation.IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
