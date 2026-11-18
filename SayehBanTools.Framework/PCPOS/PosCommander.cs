using System;

namespace SayehBanTools.Framework.PCPOS
{
    /// <summary>
    /// نقطه راحتی Singleton (ترد-سیف) برای کار با کارت‌خوان، هم‌شکل نسخه اصلی Aramestan.
    /// استفاده از طریق PosProtocolFactory هم بدون Singleton ممکن است.
    /// </summary>
    public class PosCommander
    {
        private static readonly object Lock = new object();
        private static PosCommander instance;

        private IPosProtocol protocol;

        private PosCommander() { }

        public static PosCommander Instance
        {
            get
            {
                lock (Lock)
                {
                    if (instance == null)
                        instance = new PosCommander();
                    return instance;
                }
            }
        }

        /// <summary>آماده‌سازی پروتکل و تست اتصال، بر اساس تنظیمات.</summary>
        public bool Init(PosConnectionSettings settings)
        {
            try
            {
                protocol = PosProtocolFactory.Create(settings);
                return true;
            }
            catch (Exception)
            {
                protocol = null;
                return false;
            }
        }

        public Response Purchase(string additionalData, string amount, bool print, string carId = "", string carOwner = "", string subject = "")
        {
            return RequireProtocol().Purchase(additionalData, amount, print, carId, carOwner, subject);
        }

        public Response BalanceInquiry(string additionalData, bool print)
        {
            return RequireProtocol().BalanceInquiry(additionalData, print);
        }

        public Response BillPayment(string additionalData, string billId, string paymentId, bool print)
        {
            return RequireProtocol().BillPayment(additionalData, billId, paymentId, print);
        }

        public Response GetPosInfo()
        {
            return RequireProtocol().GetPosInfo();
        }

        public Response LastTransaction()
        {
            return RequireProtocol().LastTransaction();
        }

        public Response LastPcPosTransaction()
        {
            return RequireProtocol().LastPcPosTransaction();
        }

        public Response PrintText(string text, Language lang)
        {
            return RequireProtocol().PrintText(text, lang);
        }

        public Response PrintBarcode(string barcode, Language lang)
        {
            return RequireProtocol().PrintBarcode(barcode, lang);
        }

        private IPosProtocol RequireProtocol()
        {
            if (protocol == null)
                throw new InvalidOperationException("کارت‌خوان مقداردهی نشده است. اول PosCommander.Instance.Init(settings) را صدا بزنید.");
            return protocol;
        }
    }
}