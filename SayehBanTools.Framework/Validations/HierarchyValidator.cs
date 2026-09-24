using System;
using System.Linq;

namespace SayehBanTools.Framework.Validations
{

    /// <summary>
    /// کلاس بررسی ساختار درختی
    /// </summary>
    public static class HierarchyValidator
    {
        /// <summary>
        /// بررسی اعتبار سنجی ساختار درختی
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsValidHierarchyPath(string path)
        {
            // مسیر null یا خالی نامعتبر است
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            // بررسی وجود چند اسلش متوالی
            if (path.Contains("//"))
            {
                return false;
            }

            // مسیر فقط "/" معتبر است
            if (path == "/")
            {
                return true;
            }

            // باید با / شروع و پایان یابد
            if (!path.StartsWith("/") || !path.EndsWith("/"))
            {
                return false;
            }

            // بین /ها باید اعداد باشند و حداقل یک عدد وجود داشته باشد
            var parts = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                return false;
            }

            return parts.All(part => int.TryParse(part, out _));
        }
        /// <summary>
        /// بررسی صحت رشته مسیر سلسله مراتب والد.
        /// </summary>
        /// <param name="parentHierarchyPath">رشته مسیر سلسله مراتب (مثال: "/" یا "/1/")</param>
        /// <param name="message">پیام خطا اختیاری برای استثنا</param>
        /// <exception cref="ArgumentException">در صورتی که فرمت مسیر سلسله مراتب نامعتبر باشد.</exception>
        public static void ValidateHierarchyPath(string parentHierarchyPath, string message = null)
        {
            if (!IsValidHierarchyPath(parentHierarchyPath))
            {
                throw new ArgumentException(message ?? "فرمت سلسله مراتب نامعتبر است. باید به صورت / یا /1/ باشد.",
                    nameof(parentHierarchyPath));
            }
        }
    }
}
