using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.component.CommonControls
{
    public class PicBoxShow : PictureBox
    {
        // نگهداری بایت‌های اصلی تصویر برای باز کردن فایل موقت
        public byte[] RawImageBytes { get; private set; }

        // نگهداری مسیر فایل (در صورتی که فایل از سیستم بارگذاری شده باشد)
        public string ImagePath { get; set; }

        public PicBoxShow()
        {
            // تنظیمات ظاهری پیش‌فرض
            this.SizeMode = PictureBoxSizeMode.Zoom; // Zoom تصویر را متناسب نگه می‌دارد و دفرم نمی‌کند
            this.BorderStyle = BorderStyle.Fixed3D;
            this.Cursor = Cursors.Hand; // تغییر نشانگر ماوس برای راهنمایی کاربر
        }

        /// <summary>
        /// مقداردهی و نمایش تصویر مستقیم از آرایه بایتی دیتابیس
        /// </summary>
        public void SetImageFromBytes(byte[] imageBytes)
        {
            this.RawImageBytes = imageBytes;

            if (imageBytes != null && imageBytes.Length > 0)
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        if (this.Image != null)
                        {
                            this.Image.Dispose();
                        }
                        this.Image = Image.FromStream(ms);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("خطا در بارگذاری تصویر: " + ex.Message);
                    this.Image = null;
                }
            }
            else
            {
                if (this.Image != null)
                {
                    this.Image.Dispose();
                }
                this.Image = null;
            }
        }

        // اکشن با دبل‌کلیک روی تصویر
        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);

            if (this.Image == null) return;

            try
            {
                string targetPath = this.ImagePath;

                // اگر تصویر از دیتابیس آمده و مسیر فایل فیزیکی ندارد، یک فایل موقت ساخت می‌شود
                if (string.IsNullOrEmpty(targetPath) || !File.Exists(targetPath))
                {
                    if (this.RawImageBytes != null && this.RawImageBytes.Length > 0)
                    {
                        string tempFolder = Path.Combine(Path.GetTempPath(), "SayehBanTools_TempImages");
                        if (!Directory.Exists(tempFolder))
                        {
                            Directory.CreateDirectory(tempFolder);
                        }

                        // ساخت یک نام یکتا برای فایل موقت
                        targetPath = Path.Combine(tempFolder, $"Img_{Guid.NewGuid():N}.png");
                        File.WriteAllBytes(targetPath, this.RawImageBytes);
                    }
                }

                // اگر مسیر معتبری برای تصویر وجود دارد، آن را در نمایشگر ویندوز باز کن
                if (!string.IsNullOrEmpty(targetPath) && File.Exists(targetPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = targetPath,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در نمایش بزرگتر تصویر:\n" + ex.Message, "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RtlReading);
            }
        }
    }
}