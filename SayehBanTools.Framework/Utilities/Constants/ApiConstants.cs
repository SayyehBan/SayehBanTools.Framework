namespace SayehBanTools.Framework.Utilities.Constants
{
    /// <summary>
    /// ثابت های API
    /// </summary>
    public class ApiConstants
    {
        /// <summary>
        /// مقدار پیش‌فرض برای آدرس API.
        /// </summary>
        public const string DefaultApiLink = "http://localhost:90";

        /// <summary>
        /// مقدار پیش‌فرض برای مسیر API دریافت تمام زبانها
        /// </summary>
        public const string ApiAddressLanguageCodeList = "api/LanguagesCode/LanguageCodeList";
        /// <summary>
        /// آدرس API دریافت تمام تنظیمات زبان
        /// </summary>
        public const string ApiAddressLanguageSettingsGetAll = "api/LanguageSettings/LanguageSettingsGetAll";
        /// <summary>
        /// طول تعداد شماره تلفن
        /// </summary>
        public const int LimitTel = 10;
        /// <summary>
        /// Expression بررسی کننده ایمیل
        /// </summary>
        public const string ExpressionEmail = "\\w+([-+.\']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
        /// <summary>
        /// Expression بررسی کننده سایت
        /// </summary>
        public const string ExpressionSite = @"^[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(/\S*)?$";
    }
}
