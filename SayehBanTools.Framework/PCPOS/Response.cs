namespace SayehBanTools.Framework.PCPOS
{
    /// <summary>زبان چاپ روی کارت‌خوان</summary>
    public enum Language
    {
        en = 1,
        fa
    }

    /// <summary>اطلاعات دستگاه کارت‌خوان</summary>
    public class PosInfo
    {
        public string TerminalId = "";
        public string MerchantId = "";
        public string MerchantName = "";
        public string SerialNumber = "";
    }

    /// <summary>جزئیات تراکنش موفق</summary>
    public class Transaction
    {
        public string ResponseCode = "";
        public string SVC = "";
        public string PAN = "";
        public string Amount = "";
        public string DateTime = "";
        public string Stan = "";
        public string RRN = "";
        public string EntryType = "";
    }

    /// <summary>پاسخ کارت‌خوان. کدهای خطا: 201 عدم اتصال، 202 خطای ارسال، 203 پاسخ خالی، 204 پاسخ نامعتبر.</summary>
    public class Response
    {
        public string AdditionalData = "";
        public bool Success;
        public int ErrorCode;
        public int WarningCode;
        public PosInfo PosInformation;
        public Transaction TransactionInfo;
    }
}