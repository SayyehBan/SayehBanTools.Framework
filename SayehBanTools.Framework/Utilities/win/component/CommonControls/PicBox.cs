using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.component.CommonControls
{
    public class PicBox : PictureBox
    {
        public byte[] RawImageBytes { get; private set; }
        public string ImagePath { get; private set; }

        // تعریف دکمه حذف و ToolTip
        private Button btnRemove;
        private ToolTip toolTip;

        public PicBox()
        {
            this.SizeMode = PictureBoxSizeMode.Zoom;
            this.BorderStyle = BorderStyle.Fixed3D;
            this.Cursor = Cursors.Hand;

            // پیکربندی ظاهر و عملکرد دکمه حذف
            btnRemove = new Button();
            btnRemove.Text = "✖";
            btnRemove.ForeColor = Color.White;
            btnRemove.BackColor = Color.Red;
            btnRemove.FlatStyle = FlatStyle.Flat;
            btnRemove.FlatAppearance.BorderSize = 0;
            btnRemove.Size = new Size(22, 22);
            btnRemove.Cursor = Cursors.Hand;
            btnRemove.Visible = false; // در حالت پیش‌فرض مخفی است
            btnRemove.Font = new Font("Arial", 8, FontStyle.Bold);
            btnRemove.Click += BtnRemove_Click;

            toolTip = new ToolTip();
            toolTip.SetToolTip(btnRemove, "حذف");

            this.Controls.Add(btnRemove);
        }

        // رویداد کلیک دکمه ضربدر
        private void BtnRemove_Click(object sender, EventArgs e)
        {
            ClearImage();
        }

        // متد پاکسازی تصویر و ریست متغیرها
        public void ClearImage()
        {
            if (this.Image != null)
            {
                this.Image.Dispose();
            }
            this.Image = null;
            this.RawImageBytes = null;
            this.ImagePath = null;
            btnRemove.Visible = false;
        }

        // بروزرسانی موقعیت دکمه با تغییر سایز کنترل
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateBtnPosition();
        }

        // بروزرسانی موقعیت دکمه بر اساس راست‌چین یا چپ‌چین بودن فرم
        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            UpdateBtnPosition();
        }

        private void UpdateBtnPosition()
        {
            if (this.RightToLeft == RightToLeft.Yes)
            {
                btnRemove.Location = new Point(0, 0); // چپ
            }
            else
            {
                btnRemove.Location = new Point(this.Width - btnRemove.Width, 0); // راست
            }
        }

        public void SetImageFromBytes(byte[] imageBytes)
        {
            this.RawImageBytes = imageBytes;
            this.ImagePath = null;

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
                        btnRemove.Visible = true; // نمایش ضربدر
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("خطا در بارگذاری تصویر: " + ex.Message);
                    ClearImage();
                }
            }
            else
            {
                ClearImage();
            }
        }

        public byte[] GetImageBytes()
        {
            if (!string.IsNullOrEmpty(ImagePath) && File.Exists(ImagePath))
            {
                return File.ReadAllBytes(ImagePath);
            }
            return RawImageBytes;
        }

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
                            this.RawImageBytes = fileBytes;
                            btnRemove.Visible = true; // نمایش ضربدر
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

        protected override void OnClick(EventArgs e)
        {
            // در صورتی که کلیک دقیقاً روی دکمه ضربدر انجام شده باشد، کد باز کردن عکس متوقف می‌شود
            if (e is MouseEventArgs mouseArgs && btnRemove.Bounds.Contains(mouseArgs.Location))
                return;

            base.OnClick(e);

            if (this.Image == null) return;

            try
            {
                string targetPath = this.ImagePath;

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