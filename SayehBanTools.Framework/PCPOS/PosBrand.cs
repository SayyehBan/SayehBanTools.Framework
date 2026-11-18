using System.Collections.Generic;

namespace SayehBanTools.Framework.PCPOS
{
    /// <summary>برندهای کارت‌خوان بانکی (PC-POS). برای بانک‌های جدید این enum گسترش داده می‌شود.</summary>
    public enum PosBrand
    {
        SayanKart,
        BankShahr,
        Sadad,
        SamanKish,
        Fanava,
        Parsian,
        Pasargad,
        Melli,
        IranKish,
        NovinArian,
        AsanPardaz,
        MabnaAria
    }

    /// <summary>اطلاعات هر برند کارت‌خوان: نام سیستمی، نام نمایشی فارسی و اینکه آیا پروتکل عمومی CPML را پشتیبانی می‌کند.</summary>
    public class PosBrandInfo
    {
        public PosBrand Brand { get; set; }
        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
        /// <summary>آیا این برند با پروتکل CPML کار می‌کند (در PosProtocolFactory قابل ساخت است).</summary>
        public bool Supported { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }

        /// <summary>ثبت‌نام برندها. هر برندی که با CPML کار کند Supported=true می‌گیرد؛ بقیه برای پروتکل‌های اختصاصی (مثل Sunyard بانک شهر) رزرو شده‌اند.</summary>
        public static IReadOnlyList<PosBrandInfo> GetAll()
        {
            return Registry;
        }

        public static PosBrandInfo Get(PosBrand brand)
        {
            return Registry[(int)brand];
        }

        private static readonly List<PosBrandInfo> Registry = new List<PosBrandInfo>
        {
            new PosBrandInfo { Brand = PosBrand.SayanKart,   Name = "SayanKart",   DisplayName = "سایان‌کارت",   Supported = true  },
            new PosBrandInfo { Brand = PosBrand.BankShahr,   Name = "BankShahr",   DisplayName = "بانک شهر",    Supported = false },
            new PosBrandInfo { Brand = PosBrand.Sadad,       Name = "Sadad",       DisplayName = "سداد",        Supported = false },
            new PosBrandInfo { Brand = PosBrand.SamanKish,   Name = "SamanKish",   DisplayName = "سامان کیش",   Supported = false },
            new PosBrandInfo { Brand = PosBrand.Fanava,      Name = "Fanava",      DisplayName = "فناوا",       Supported = false },
            new PosBrandInfo { Brand = PosBrand.Parsian,     Name = "Parsian",     DisplayName = "پارسیان",     Supported = false },
            new PosBrandInfo { Brand = PosBrand.Pasargad,    Name = "Pasargad",    DisplayName = "پاسارگاد",    Supported = false },
            new PosBrandInfo { Brand = PosBrand.Melli,       Name = "Melli",       DisplayName = "بانک ملی",    Supported = false },
            new PosBrandInfo { Brand = PosBrand.IranKish,    Name = "IranKish",    DisplayName = "ایران کیش",   Supported = false },
            new PosBrandInfo { Brand = PosBrand.NovinArian,  Name = "NovinArian",  DisplayName = "نوین آریان",  Supported = false },
            new PosBrandInfo { Brand = PosBrand.AsanPardaz,  Name = "AsanPardaz",  DisplayName = "آسان پرداخت", Supported = false },
            new PosBrandInfo { Brand = PosBrand.MabnaAria,   Name = "MabnaAria",   DisplayName = "مبنا آریا",   Supported = false }
        };
    }
}
