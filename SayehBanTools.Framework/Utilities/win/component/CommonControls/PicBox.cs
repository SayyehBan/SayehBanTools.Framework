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

        /// <summary>
        /// تصویر کنترل. با هر بار مقداردهی (حتی مستقیم از فرم مثل <c>PicBox.Image = ...</c>)
        /// دکمه ضربدر حذف خودکار نمایش داده یا مخفی می‌شود.
        /// (قبلاً فقط SetImageFromBytes و دابل‌کلیک ضربدر را نشان می‌دادند)
        /// </summary>
        [System.ComponentModel.Localizable(true)]
        public new Image Image
        {
            get { return base.Image; }
            set
            {
                // تصویری که مستقیم ست می‌شود بایت/مسیر قبلی را بی‌اعتبار می‌کند
                RawImageBytes = null;
                ImagePath = null;
                SetImageInternal(value);
            }
        }

        private void SetImageInternal(Image value)
        {
            base.Image = value;
            UpdateRemoveButton();
        }

        private void UpdateRemoveButton()
        {
            if (btnRemove == null) return;
            bool hasImage = base.Image != null;
            if (btnRemove.Visible != hasImage) btnRemove.Visible = hasImage;
            if (hasImage)
            {
                UpdateBtnPosition();
                btnRemove.BringToFront();
            }
        }

        public PicBox()
        {
            // نمایش کشیده (پر کردن کامل کادر) به‌صورت پیش‌فرض
            this.SizeMode = PictureBoxSizeMode.StretchImage;
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
            UpdateBtnPosition();
        }

        // اگر تصویر از مسیری غیر از خاصیت Image این کلاس عوض شده باشد (مثلاً از طریق مرجع PictureBox)،
        // وضعیت ضربدر هنگام رسم هماهنگ می‌شود
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            if (btnRemove != null && btnRemove.Visible != (base.Image != null))
            {
                BeginInvoke((Action)UpdateRemoveButton);
            }
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            UpdateBtnPosition();
        }

        // رویداد کلیک دکمه ضربدر
        private void BtnRemove_Click(object sender, EventArgs e)
        {
            ClearImage();
        }

        // متد پاکسازی تصویر و ریست متغیرها
        public void ClearImage()
        {
            Image old = base.Image;
            this.RawImageBytes = null;
            this.ImagePath = null;
            SetImageInternal(null);
            if (old != null)
            {
                old.Dispose();
            }
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
            if (btnRemove == null) return;
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
                    Image old = base.Image;
                    // کپی Bitmap تا تصویر به MemoryStream بسته‌شده وابسته نماند
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    using (Image loaded = System.Drawing.Image.FromStream(ms))
                    {
                        SetImageInternal(new Bitmap(loaded)); // نمایش ضربدر
                    }
                    if (old != null) old.Dispose();
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
            if (RawImageBytes != null) return RawImageBytes;

            // تصویری که مستقیم با خاصیت Image ست شده: به PNG تبدیل می‌شود
            if (base.Image == null) return null;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    base.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
            }
            catch
            {
                return null;
            }
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

                        Image old = base.Image;
                        using (MemoryStream ms = new MemoryStream(fileBytes))
                        using (Image loaded = System.Drawing.Image.FromStream(ms))
                        {
                            SetImageInternal(new Bitmap(loaded)); // نمایش ضربدر
                        }
                        if (old != null) old.Dispose();
                        this.ImagePath = dialog.FileName;
                        this.RawImageBytes = fileBytes;
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