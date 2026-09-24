namespace SayehBanTools.Framework.Utilities.Constants
{
    public class ExpressionConstants
    {
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
