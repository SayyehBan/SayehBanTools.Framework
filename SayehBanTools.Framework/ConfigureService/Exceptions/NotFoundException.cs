using System;

namespace SayehBanTools.Framework.ConfigureService.Exceptions
{
    /// <summary>
    /// استثنای سفارشی برای زمانی که اطلاعات یافت نشود
    /// </summary>
    public class NotFoundException : Exception
    {
        /// <summary>
        /// ایجاد نمونه جدید از استثنای عدم یافتن اطلاعات
        /// </summary>
        /// <param name="message">پیام خطا</param>
        public NotFoundException(string message) : base(message) { }
    }
}
