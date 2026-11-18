using System;

namespace SayehBanTools.Framework.PCPOS
{
    /// <summary>
    /// کارخانه ساخت پروتکل کارت‌خوان بر اساس برند. برندهای خانواده CPML نمونه CpmlPosProtocol می‌گیرند؛
    /// بقیه با NotSupportedException رد می‌شوند تا پروتکل‌های اختصاصی بعداً جایگزین شوند.
    /// </summary>
    public static class PosProtocolFactory
    {
        public static IPosProtocol Create(PosBrand brand, PosConnectionSettings settings)
        {
            if (!PosBrandInfo.Get(brand).Supported)
                throw new NotSupportedException("این برند هنوز پیاده‌سازی نشده");

            CpmlPosProtocol protocol = new CpmlPosProtocol();
            protocol.Init(settings);
            return protocol;
        }

        /// <summary>ساخت پروتکل مستقیماً از تنظیمات (برند از داخل تنظیمات خوانده می‌شود).</summary>
        public static IPosProtocol Create(PosConnectionSettings settings)
        {
            return Create(settings.Brand, settings);
        }
    }
}