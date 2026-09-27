using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.component.CommonControls
{
    public class PicBoxShow:PictureBox
    {      // یک پراپرتی برای نگهداری مسیر فایل تصویر اضافه کردیم
        public string ImagePath { get; private set; }

        public PicBoxShow()
        {
            // تنظیمات پیش‌فرض کنترل
            this.SizeMode = PictureBoxSizeMode.StretchImage;
            this.BorderStyle = BorderStyle.Fixed3D;
        }

        // با یک بار کلیک، اگر تصویری وجود داشته باشد باز می‌شود
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            // بررسی می‌کنیم که هم تصویر وجود داشته باشد و هم مسیر فایل معتبر باشد
            if (this.Image != null && !string.IsNullOrEmpty(ImagePath) && File.Exists(ImagePath))
            {
                try
                {
                    // باز کردن تصویر در نمایشگر پیش‌فرض ویندوز
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = ImagePath,
                        UseShellExecute = true // این گزینه برای .NET Core و نسخه‌های جدیدتر الزامی است
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطا در نمایش تصویر:\n" + ex.Message, "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
