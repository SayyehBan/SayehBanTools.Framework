namespace SayehBanTools.Framework.PCPOS
{
    /// <summary>نوع ارتباط با کارت‌خوان.</summary>
    public enum PosConnectionType
    {
        /// <summary>پورت سریال (COM)</summary>
        SerialPort = 1,
        /// <summary>شبکه محلی (TCP)</summary>
        Lan
    }

    /// <summary>
    /// تنظیمات اتصال به کارت‌خوان. کلاس ساده و قابل سریال‌شدن با JSON (بدون وابستگی به Properties.Settings).
    /// </summary>
    public class PosConnectionSettings
    {
        public PosBrand Brand { get; set; } = PosBrand.SayanKart;

        public PosConnectionType ConnectionType { get; set; } = PosConnectionType.SerialPort;

        /// <summary>نام پورت سریال مثل COM3</summary>
        public string SerialPort { get; set; } = "COM3";

        /// <summary>نرخ باود پیش‌فرض اکثر PC-POSها</summary>
        public int BaudRate { get; set; } = 38400;

        /// <summary>تایم‌اوت پورت سریال (ثانیه)</summary>
        public int SerialTimeout { get; set; } = 30;

        /// <summary>آی‌پی کارت‌خوان در حالت شبکه</summary>
        public string LanIp { get; set; } = "192.168.1.100";

        /// <summary>پورت TCP پیش‌فرض پروتکل CPML</summary>
        public int LanPort { get; set; } = 16028;

        /// <summary>تایم‌اوت شبکه (ثانیه)</summary>
        public int LanTimeout { get; set; } = 60;

        /// <summary>چاپ رسید توسط خود دستگاه کارت‌خوان</summary>
        public bool PrintReceipt { get; set; } = true;
    }
}