using System;
using System.Text.RegularExpressions;

namespace SayehBanTools.Framework.Converter
{
    /// <summary>
    /// افزونه‌های مدیریت آدرس‌های IP
    /// </summary>
    public static class IPExtensions
    {
        /// <summary>
        /// تبدیل آدرس IP به عدد صحیح
        /// </summary>
        /// <param name="ipAddress">آدرس IP به‌صورت رشته (مثال: "192.168.0.1")</param>
        /// <returns>عدد صحیح معادل آدرس IP</returns>
        /// <exception cref="ArgumentException">در صورت نامعتبر بودن آدرس IP</exception>
        public static long IPToNumber(this string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                throw new ArgumentException("آدرس IP نامعتبر است");

            // بررسی فرمت IP با Regex
            if (!Regex.IsMatch(ipAddress, @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$"))
                throw new ArgumentException("فرمت آدرس IP نامعتبر است");

            // تقسیم آدرس IP به بخش‌ها
            var parts = ipAddress.Split('.');
            if (parts.Length != 4)
                throw new ArgumentException("فرمت آدرس IP نامعتبر است");

            // اعتبارسنجی مقادیر بخش‌ها (0 تا 255)
            for (int i = 0; i < parts.Length; i++)
            {
                if (!long.TryParse(parts[i], out long value) || value < 0 || value > 255)
                    throw new ArgumentException("مقدار بخش‌های IP باید بین 0 و 255 باشد");
            }

            // تبدیل به عدد صحیح
            return (long.Parse(parts[0]) * 256 * 256 * 256) +
                   (long.Parse(parts[1]) * 256 * 256) +
                   (long.Parse(parts[2]) * 256) +
                   long.Parse(parts[3]);
        }

        /// <summary>
        /// تبدیل عدد صحیح به آدرس IP
        /// </summary>
        /// <param name="ipNumber">عدد صحیح معادل آدرس IP</param>
        /// <returns>آدرس IP به‌صورت رشته (مثال: "192.168.0.1")</returns>
        /// <exception cref="ArgumentException">در صورت نامعتبر بودن عدد ورودی</exception>
        public static string NumberToIPAddress(this long ipNumber)
        {
            if (ipNumber < 0 || ipNumber > 4294967295) // 2^32 - 1
                throw new ArgumentException("عدد IP نامعتبر است");

            long octet1 = ipNumber / (256 * 256 * 256);
            long octet2 = (ipNumber % (256 * 256 * 256)) / (256 * 256);
            long octet3 = (ipNumber % (256 * 256)) / 256;
            long octet4 = ipNumber % 256;

            return $"{octet1}.{octet2}.{octet3}.{octet4}";
        }

        /// <summary>
        /// اعتبارسنجی فرمت آدرس IP
        /// </summary>
        /// <param name="ipAddress">آدرس IP به‌صورت رشته</param>
        /// <returns>در صورت معتبر بودن آدرس IP، مقدار true برمی‌گرداند</returns>
        public static bool IsValidIPAddress(this string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                return false;

            if (!Regex.IsMatch(ipAddress, @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$"))
                return false;

            var parts = ipAddress.Split('.');
            if (parts.Length != 4)
                return false;

            foreach (var part in parts)
            {
                if (!long.TryParse(part, out long value) || value < 0 || value > 255)
                    return false;
            }

            return true;
        }
    }
}