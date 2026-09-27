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
        /// <param name="form">فرمی که متد روی آن فراخوانی می‌شود</param>
        /// <param name="imageBytes">آرایه بایتی تصویر آیکون/لوگو</param>
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

                        // آزادسازی Handle از حافظه ویندوز
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
        /// <param name="form">فرمی که متد روی آن فراخوانی می‌شود</param>
        /// <param name="dynamicTitle">عنوان پویا (مثلاً نام فروشگاه یا اسم کاربر)</param>
        /// <param name="defaultTitle">عنوان اصلی فرم یا نرم‌افزار</param>
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
        ///دکمه خروج
        /// </summary>
        public static void ExitApp()
        {
            DialogResult dr = MessageBox.Show(
                "آیا مایل به خروج از برنامه هستید؟",
                "خروج",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
        /// <summary>
        /// دکمه خروج برای دکمه ضربدر
        /// </summary>
        /// <returns></returns>
        public static bool ConfirmExit()
        {
            DialogResult dr = MessageBox.Show(
                "آیا مایل به خروج از برنامه هستید؟",
                "خروج",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2, // فوکوس پیش‌فرض روی No برای جلوگیری از خروج اشتباهی
                MessageBoxOptions.RtlReading);  // جهت راست‌به‌چپ برای متن فارسی

            return dr == DialogResult.Yes;
        }
    }
}
