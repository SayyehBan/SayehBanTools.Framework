using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.Helper
{
    public static class SystemHelper
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);

        /// <summary>
        /// تنظیم لوگوی برنامه روی فرم جاری به‌صورت پویا از طریق آرایه بایتی
        /// </summary>
        public static void SetAppIcon(this Form form, byte[] imageBytes)
        {
            if (form == null || imageBytes == null || imageBytes.Length == 0) return;

            try
            {
                using (MemoryStream msLogo = new MemoryStream(imageBytes))
                {
                    using (Bitmap bmp = new Bitmap(Image.FromStream(msLogo)))
                    {
                        IntPtr hIcon = IntPtr.Zero;
                        using (Bitmap thumb = (Bitmap)bmp.GetThumbnailImage(32, 32, null, IntPtr.Zero))
                        {
                            thumb.MakeTransparent();
                            hIcon = thumb.GetHicon();
                            using (Icon tempIcon = Icon.FromHandle(hIcon))
                            {
                                form.Icon = (Icon)tempIcon.Clone();
                            }
                        }

                        if (hIcon != IntPtr.Zero)
                        {
                            DestroyIcon(hIcon);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("خطا در تبدیل آیکون: " + ex.Message);
            }
        }

        /// <summary>
        /// تنظیم عنوان فرم به‌صورت پویا و ترکیب با عنوان پیش‌فرض
        /// </summary>
        public static void SetAppTitle(this Form form, string dynamicTitle, string defaultTitle = "سیستم مدیریت فروش")
        {
            if (form == null) return;

            if (!string.IsNullOrWhiteSpace(dynamicTitle))
            {
                form.Text = $"{defaultTitle} - {dynamicTitle}";
            }
            else
            {
                form.Text = defaultTitle;
            }
        }

        /// <summary>
        /// دکمه خروج کامل از نرم‌افزار (بستن تمام کل برنامه)
        /// </summary>
        public static void ExitApp()
        {
            DialogResult dr = MessageBox.Show(
                "آیا مایل به خروج از برنامه هستید؟",
                "خروج",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.RtlReading);

            if (dr == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// تأیید خروج کامل از نرم‌افزار (مناسب جهت استفاده در FormClosing فرم اصلی)
        /// </summary>
        public static bool ConfirmExit()
        {
            DialogResult dr = MessageBox.Show(
                "آیا مایل به خروج از برنامه هستید؟",
                "خروج",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.RtlReading);

            return dr == DialogResult.Yes;
        }

        /// <summary>
        /// بستن فرم جاری به صورت Extension Method
        /// </summary>
        /// <param name="form">فرمی که قرار است بسته شود</param>
        public static void CloseApp(this Form form)
        {
            if (form == null) return;

            DialogResult dr = MessageBox.Show(
                "آیا مایل به بستن این فرم هستید؟",
                "بستن فرم",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.RtlReading);

            if (dr == DialogResult.Yes)
            {
                form.Close(); // استفاده از پارامتر form به جای this
            }
        }

        /// <summary>
        /// تأیید بستن فرم جاری (مناسب جهت استفاده در FormClosing فرم‌های زیرمجموعه)
        /// </summary>
        public static bool ConfirmClose()
        {
            DialogResult dr = MessageBox.Show(
                "آیا مایل به بستن این فرم هستید؟",
                "بستن فرم",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.RtlReading);

            return dr == DialogResult.Yes;
        }
        // نگهداری یک نمونه استاتیک برای جلوگیری از پاک شدن سریع از حافظه
        public static class WindowsNotifier
        {
            // نگهداری یک نمونه استاتیک برای جلوگیری از پاک شدن سریع از حافظه
            private static NotifyIcon _notifyIcon;

            public static void Show(string title, string message, ToolTipIcon iconType = ToolTipIcon.Info)
            {
                // اگر هنوز ساخته نشده بود، آن را ایجاد می‌کنیم
                if (_notifyIcon == null)
                {
                    _notifyIcon = new NotifyIcon
                    {
                        Icon = SystemIcons.Information,
                        Visible = true
                    };
                }

                // اطمینان از دیده شدن و ارسال پیام به ویندوز
                _notifyIcon.Visible = true;
                _notifyIcon.ShowBalloonTip(3000, title, message, iconType);
            }
        }
    }
}