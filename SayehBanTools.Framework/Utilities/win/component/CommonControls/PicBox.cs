using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.component.CommonControls
{
    public class PicBox : PictureBox
    {
        /// <summary>
        /// نگهداشت بایت‌های تصویر (برای تصاویری که از دیتابیس دریافت می‌شوند)
        /// </summary>
        public byte[] RawImageBytes { get; private set; }

        /// <summary>
        /// نگهداشت مسیر فایل (برای تصاویری که از سیستم انتخاب می‌شوند)
        /// </summary>
        public string ImagePath { get; private set; }

        public PicBox()
        {
            this.SizeMode = PictureBoxSizeMode.Zoom;
            this.BorderStyle = BorderStyle.Fixed3D;
            this.Cursor = Cursors.Hand;
        }

        /// <summary>
        /// بارگذاری و نمایش تصویر مستقیم از آرایه بایتی دیتابیس
        /// </summary>
        /// <param name="imageBytes">بایت‌های تصویر دریافتی از SQL Server</param>
        public void SetImageFromBytes(byte[] imageBytes)
        {
            this.RawImageBytes = imageBytes;
            this.ImagePath = null; // پاکسازی مسیر قبلی در صورت وجود

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

        /// <summary>
        /// دریافت بایت‌های تصویر فعلی برای ذخیره‌سازی یا بروزرسانی در دیتابیس
        /// </summary>
        /// <returns>آرایه بایتی تصویر جهت ارسال به Stored Procedure</returns>
        public byte[] GetImageBytes()
        {
            // اگر تصویر جدیدی از دیسک انتخاب شده باشد
            if (!string.IsNullOrEmpty(ImagePath) && File.Exists(ImagePath))
            {
                return File.ReadAllBytes(ImagePath);
            }

            // اگر تصویر تغییر نکرده و همان بایت‌های قبلی دیتابیس است
            return RawImageBytes;
        }

        /// <summary>
        /// با دوبار کلیک، پنجره انتخاب فایل تصویر جدید باز می‌شود
        /// </summary>
        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);

            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "تصاویر پشتیبانی شده|*.JPG;*.JPEG;*.JPE;*.JFIF;*.PNG;*.BMP;*.DIB;*.RLE;*.TIF;*.TIFF" +
                 "|فایل‌های JPEG|*.JPG;*.JPEG;*.JPE;*.JFIF" +
                 "|فایل‌های PNG|*.PNG" +
                 "|فایل‌های BMP|*.BMP;*.DIB;*.RLE" +
                 "|فایل‌های TIFF|*.TIF;*.TIFF";
                dialog.Title = "انتخاب تصویر جدید";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        byte[] fileBytes = File.ReadAllBytes(dialog.FileName);

                        using (MemoryStream ms = new MemoryStream(fileBytes))
                        {
                            if (this.Image != null)
                            {
                                this.Image.Dispose();
                            }

                            this.Image = Image.FromStream(ms);
                            this.ImagePath = dialog.FileName;
                            this.RawImageBytes = fileBytes; // بروزرسانی بایت‌ها با فایل جدید
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("خطا در بارگذاری تصویر:\n" + ex.Message, "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RtlReading);
                    }
                }
            }
        }

        /// <summary>
        /// با تک‌کلیک، تصویر فعلی (چه فایل محلی و چه بایت‌های دیتابیس) بزرگ نمایش داده می‌شود
        /// </summary>
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (this.Image == null) return;

            try
            {
                string targetPath = this.ImagePath;

                // اگر تصویر از دیتابیس آمده و فایل روی دیسک ندارد، یک فایل موقت (Temp) ایجاد می‌شود
                if (string.IsNullOrEmpty(targetPath) || !File.Exists(targetPath))
                {
                    if (this.RawImageBytes != null && this.RawImageBytes.Length > 0)
                    {
                        string tempFolder = Path.Combine(Path.GetTempPath(), "SayehBanTools_TempImages");
                        if (!Directory.Exists(tempFolder))
                        {
                            Directory.CreateDirectory(tempFolder);
                        }

                        targetPath = Path.Combine(tempFolder, $"Img_{Guid.NewGuid():N}.png");
                        File.WriteAllBytes(targetPath, this.RawImageBytes);
                    }
                }

                // باز کردن تصویر در نمایشگر پیش‌فرض ویندوز
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
                MessageBox.Show("خطا در نمایش بزرگتر تصویر:\n" + ex.Message, "خطا",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error, 
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RtlReading);
            }
        }
    }
}
/*
 نحوه استفاده از دستور
// نمایش تصویر رکورد انتخابی در کامپوننت
picProductImage.SetImageFromBytes(selectedProduct.ImageBytes);

 // دریافت آرایه بایتی تصویر (چه عکس قبلی باشد و چه عکس جدیدی انتخاب شده باشد)
byte[] imageToSendToDb = picProductImage.GetImageBytes();

var model = new ProductUpdateModel
{
    ProductID = id,
    ProductImage = imageToSendToDb
};

await _productService.UpdateAsync(model);
 */