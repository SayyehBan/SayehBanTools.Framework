using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.component.CommonControls
{
    public class PicBox : PictureBox
    {
        // یک پراپرتی برای نگهداری مسیر فایل تصویر اضافه کردیم
        public string ImagePath { get; private set; }

        public PicBox()
        {
            // تنظیمات پیش‌فرض کنترل
            this.SizeMode = PictureBoxSizeMode.StretchImage;
            this.BorderStyle = BorderStyle.Fixed3D;
        }

        // با دو بار کلیک، پنجره انتخاب تصویر باز می‌شود
        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);

            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "تمام تصاویر|*.BMP;*.DIB;*.RLE;*.JPG;*.JPEG;*.JPE;*.JFIF;*.GIF;*.TIF;*.TIFF;*.PNG|BMP فایل: (*.BMP;*.DIB;*.RLE)|*.BMP;*.DIB;*.RLE|JPEG فایل: (*.JPG;*.JPEG;*.JPE;*.JFIF)|*.JPG;*.JPEG;*.JPE;*.JFIF|GIF فایل: (*.GIF)|*.GIF|TIFF فایل: (*.TIF;*.TIFF)|*.TIF;*.TIFF|PNG فایل: (*.PNG)|*.PNG|تمام فایل ها|*.*";
                dialog.Title = "انتخاب تصویر";

                // بررسی می‌کنیم که کاربر حتماً عکسی انتخاب کرده باشد (روی OK کلیک کرده باشد)
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // استفاده از FileStream برای جلوگیری از قفل شدن فایل روی هارد
                        using (FileStream fs = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read))
                        {
                            // اگر عکسی از قبل وجود دارد، آن را از حافظه پاک کن تا رم پر نشود
                            if (this.Image != null)
                            {
                                this.Image.Dispose();
                            }

                            // خواندن تصویر از استریم
                            this.Image = Image.FromStream(fs);
                            this.ImagePath = dialog.FileName; // ذخیره مسیر تصویر برای نمایش با تک‌کلیک
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("خطا در بارگذاری تصویر:\n" + ex.Message, "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
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