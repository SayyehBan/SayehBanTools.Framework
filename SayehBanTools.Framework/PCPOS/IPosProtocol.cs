namespace SayehBanTools.Framework.PCPOS
{
    /// <summary>
    /// قرارداد پروتکل کارت‌خوان. پیاده‌سازی فعلی CpmlPosProtocol است؛ پروتکل‌های دیگر
    /// (مثل Sunyard بانک شهر) بعداً با همین interface اضافه می‌شوند.
    /// </summary>
    public interface IPosProtocol
    {
        void Init(PosConnectionSettings settings);

        Response Purchase(string additionalData, string amount, bool print, string carId, string carOwner, string subject);

        Response BalanceInquiry(string additionalData, bool print);

        Response BillPayment(string additionalData, string billId, string paymentId, bool print);

        Response GetPosInfo();

        Response LastTransaction();

        Response LastPcPosTransaction();

        Response PrintText(string text, Language lang);

        Response PrintBarcode(string barcode, Language lang);
    }
}