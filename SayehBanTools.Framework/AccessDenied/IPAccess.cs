using System;

namespace SayehBanTools.Framework.AccessDenied
{
    /// <summary>
    /// دسترسی به سیستم
    /// </summary>
    public static class IPAccess
    {
        /// <summary>
        /// بررسی معتبر بودن Range IP
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="rangeStart"></param>
        /// <param name="rangeEnd"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static bool IsInRange(string ipAddress, string rangeStart, string rangeEnd)
        {
            // بررسی null
            if (ipAddress == null || rangeStart == null || rangeEnd == null)
            {
                throw new ArgumentNullException("IP address or range cannot be null.");
            }

            // بررسی فرمت IP
            string[] ipParts = ipAddress.Split('.');
            string[] rangeStartParts = rangeStart.Split('.');
            string[] rangeEndParts = rangeEnd.Split('.');

            if (ipParts.Length != 4 || rangeStartParts.Length != 4 || rangeEndParts.Length != 4)
            {
                throw new ArgumentException("Invalid IP address format. Must have exactly 4 parts.");
            }

            // بررسی عددی بودن و محدوده 0-255
            for (int i = 0; i < 4; i++)
            {
                if (!int.TryParse(ipParts[i], out int ipValue) ||
                    !int.TryParse(rangeStartParts[i], out int startValue) ||
                    !int.TryParse(rangeEndParts[i], out int endValue))
                {
                    throw new ArgumentException("IP parts must be numeric.");
                }

                if (ipValue < 0 || ipValue > 255 || startValue < 0 || startValue > 255 || endValue < 0 || endValue > 255)
                {
                    throw new ArgumentException("IP parts must be between 0 and 255.");
                }

                if (ipValue < startValue || ipValue > endValue)
                {
                    return false;
                }
            }

            return true;
        }
    }

    /*
     طریقه استفاده از دستور
    using SayehBanTools.AccessDenied;
    using SayehBanTools.Converter;

    namespace SafeIPList.Models
    {
        public class IpRestrictionMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly string[] _allowedIps;

            public IpRestrictionMiddleware(RequestDelegate next)
            {
                _next = next;

                string filePath = "wwwroot/file/IpIran.txt";
                _allowedIps = File.ReadAllLines(filePath);
            }

            public async Task Invoke(HttpContext context)
            {
                var ipAddress = context.Connection.RemoteIpAddress.ToString();
                if (_allowedIps.Any(allowedIp => IPAccess.IsInRange(StringExtensions.AnonymousIP(ipAddress), allowedIp.Split(',')[0], allowedIp.Split(',')[1])))
                {
                    await _next(context);
                }
                else
                {
                    // The IP address is not allowed.
                    context.Response.StatusCode = 403; // Forbidden
                    await context.Response.WriteAsync("Access Denied " + ipAddress);
                    return;
                }
            }

        }


        public static class IpRestrictionMiddlewareExtensions
        {
            public static IApplicationBuilder UseIpRestrictionMiddleware(this IApplicationBuilder builder)
            {
                return builder.UseMiddleware<IpRestrictionMiddleware>();
            }
        }
    }

     */
}