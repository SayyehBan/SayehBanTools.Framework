using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.component.CommonControls
{
    /// <summary>
    /// دکمه پایدار و زیبا — بدون انیمیشن ناقص و بدون چشمک زدن
    /// ظاهر ثابت در حالت عادی، هاور، فشرده و غیرفعال + پشتیبانی کامل از آیکون Image
    /// </summary>
    public class Btn : Button
    {
        private Color backColor = Color.Black;
        private Color glowColor = Color.FromArgb(0x8D, 0xBD, 0xFF);
        private Color shineColor = Color.White;

        // حالت‌های بصری
        private bool isHovered;
        private bool isPressed;

        // کش مسیر ترسیم تا در هر paint دوباره ساخته نشود
        private GraphicsPath facePath;
        private int cachedW = -1;
        private int cachedH = -1;

        // دکمه کمکی برای رندر صحیح متن + آیکون
        private Button imageButton;

        private int roundCorner = 6;

        /// <summary>
        /// شعاع گوشه‌های دکمه
        /// </summary>
        [DefaultValue(6), Category("Appearance"), Description("شعاع گردی گوشه‌های دکمه")]
        public int CornerRadius
        {
            get { return roundCorner; }
            set
            {
                roundCorner = value;
                cachedW = -1;
                Invalidate();
            }
        }

        /// <summary>
        /// رنگ پس‌زمینه دکمه
        /// </summary>
        [DefaultValue(typeof(Color), "Black")]
        public new Color BackColor
        {
            get { return backColor; }
            set
            {
                backColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// رنگ متن دکمه
        /// </summary>
        [DefaultValue(typeof(Color), "White")]
        public new Color ForeColor
        {
            get { return base.ForeColor; }
            set
            {
                base.ForeColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// رنگ خط دور دکمه
        /// </summary>
        [DefaultValue(typeof(Color), "Gray"), Category("Appearance"),
         Description("رنگ خط دور دکمه")]
        public Color BorderColor { get; set; } = Color.Gray;

        /// <summary>
        /// رنگ پس‌زمینه در حالت هاور — پیش‌فرض از GlowColor گرفته می‌شود
        /// </summary>
        [Category("Appearance"), Description("رنگ پس‌زمینه هنگام هاور (اگر خالی باشد از GlowColor استفاده می‌شود)")]
        public Color HoverBackColor { get; set; } = Color.Empty;

        /// <summary>
        /// رنگ پس‌زمینه وقتی دکمه فشرده می‌شود
        /// </summary>
        [Category("Appearance"), Description("رنگ پس‌زمینه هنگام فشردن")]
        public Color PressedBackColor { get; set; } = Color.FromArgb(30, 30, 30);

        /// <summary>
        /// رنگ هاله دکمه (همان رنگ هاور پیش‌فرض)
        /// </summary>
        [DefaultValue(typeof(Color), "255,141,189,255"), Category("Appearance"),
         Description("رنگ هاله دکمه در حالت هاور")]
        public Color GlowColor
        {
            get { return glowColor; }
            set
            {
                glowColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// رنگ لایه شیشه‌ای بالای دکمه
        /// </summary>
        [DefaultValue(typeof(Color), "White"), Category("Appearance"),
         Description("شدت افکت شیشه‌ای بالای دکمه")]
        public Color ShineColor
        {
            get { return shineColor; }
            set
            {
                shineColor = value;
                Invalidate();
            }
        }

        private Color disabledBackColor = Color.FromArgb(210, 210, 210);
        private Color disabledForeColor = Color.FromArgb(120, 120, 120);

        /// <summary>
        /// رنگ پس‌زمینه وقتی دکمه غیرفعال است
        /// </summary>
        [Category("Appearance"), Description("رنگ پس‌زمینه هنگام غیرفعال بودن")]
        public Color DisabledBackColor
        {
            get { return disabledBackColor; }
            set
            {
                disabledBackColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// رنگ متن وقتی دکمه غیرفعال است
        /// </summary>
        [Category("Appearance"), Description("رنگ متن هنگام غیرفعال بودن")]
        public Color DisabledForeColor
        {
            get { return disabledForeColor; }
            set
            {
                disabledForeColor = value;
                Invalidate();
            }
        }

        public Btn()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint, true);

            DoubleBuffered = true;
            Cursor = Cursors.Hand;
            BackColor = Color.Black;
            ForeColor = Color.White;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
        }

        #region حالت‌های موس

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                isPressed = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isPressed = false;
            Invalidate();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            isHovered = false;
            isPressed = false;
            Invalidate();
        }

        #endregion

        #region ترسیم

        private static GraphicsPath BuildFacePath(int w, int h, int radius)
        {
            var path = new GraphicsPath();
            int r = Math.Min(radius, Math.Min(w / 2, h / 2));
            if (r <= 0)
            {
                path.AddRectangle(new Rectangle(0, 0, w, h));
                return path;
            }

            int d = r * 2;
            path.AddArc(0, 0, d, d, 180, 90);
            path.AddArc(w - d, 0, d, d, 270, 90);
            path.AddArc(w - d, h - d, d, d, 0, 90);
            path.AddArc(0, h - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        // ترکیب دو رنگ با نسبت درصدی (برای افکت شیشه‌ای)
        private static Color Blend(Color baseColor, Color overlayColor, int alphaPercent)
        {
            int a = Math.Max(0, Math.Min(100, alphaPercent));
            return Color.FromArgb(
                baseColor.A,
                (baseColor.R * (100 - a) + overlayColor.R * a) / 100,
                (baseColor.G * (100 - a) + overlayColor.G * a) / 100,
                (baseColor.B * (100 - a) + overlayColor.B * a) / 100);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int w = Width, h = Height;
            if (w <= 0 || h <= 0) return;

            // مسیر ترسیم فقط وقتی اندازه عوض شد دوباره ساخته می‌شود
            if (facePath == null || cachedW != w || cachedH != h)
            {
                if (facePath != null) facePath.Dispose();
                facePath = BuildFacePath(w, h, roundCorner);
                cachedW = w;
                cachedH = h;
            }

            Color face;
            Color text;

            if (!Enabled)
            {
                face = DisabledBackColor;
                text = DisabledForeColor;
            }
            else if (isPressed)
            {
                face = PressedBackColor;
                text = ForeColor;
            }
            else if (isHovered)
            {
                face = HoverBackColor == Color.Empty ? glowColor : HoverBackColor;
                text = ForeColor;
            }
            else
            {
                face = backColor;
                text = ForeColor;
            }

            // افکت شیشه‌ای: لایه ShineColor روی رنگ پایه (فقط حالت فعال)
            if (Enabled && shineColor.A > 0)
            {
                face = Blend(face, shineColor, 18);
            }

            using (var b = new SolidBrush(face))
            {
                g.FillPath(b, facePath);
            }

            // خط دور — فقط وقتی دکمه فعال است
            if (Enabled)
            {
                using (var p = new Pen(BorderColor, 1f))
                {
                    g.DrawPath(p, facePath);
                }
            }

            // متن و آیکون
            if (Enabled)
            {
                // رندر توسط دکمه کمکی واقعی — چیدمان متن+آیکون دقیقا مثل Button استاندارد
                DrawForegroundFromButton(e, text);
            }
            else
            {
                // حالت غیرفعال: آیکون خاکستری + متن با رنگ DisabledForeColor
                DrawDisabledContent(g, w, h, text);
            }
        }

        // ترسیم متن + آیکون در حالت فعال با استفاده از یک Button کمکی
        private void DrawForegroundFromButton(PaintEventArgs pevent, Color textColor)
        {
            if (imageButton == null)
            {
                imageButton = new Button();
                imageButton.Parent = new TransparentControl();
                imageButton.BackColor = Color.Transparent;
                imageButton.FlatAppearance.BorderSize = 0;
                imageButton.FlatStyle = FlatStyle.Flat;
            }

            imageButton.ForeColor = textColor;
            imageButton.Font = Font;

            // اگر متن فقط از کاراکترهای جهت‌دار (< > « ») ساخته شده، RightToLeft را خاموش کن
            // تا mirroring رخ ندهد (مثلا >> به < تبدیل و برعکس نمایش داده نشود)
            string t = Text ?? "";
            bool directionalOnly = t.Length > 0;
            foreach (char c in t)
            {
                if (c != '<' && c != '>' && c != '«' && c != '»')
                {
                    directionalOnly = false;
                    break;
                }
            }
            imageButton.RightToLeft = directionalOnly ? RightToLeft.No : RightToLeft;

            imageButton.Image = Image;
            imageButton.ImageAlign = ImageAlign;
            imageButton.ImageIndex = ImageIndex;
            imageButton.ImageKey = ImageKey;
            imageButton.ImageList = ImageList;
            imageButton.Padding = Padding;
            imageButton.Size = Size;
            imageButton.Text = Text;
            imageButton.TextAlign = TextAlign;
            imageButton.TextImageRelation = TextImageRelation;
            imageButton.UseCompatibleTextRendering = UseCompatibleTextRendering;
            imageButton.UseMnemonic = UseMnemonic;

            InvokePaint(imageButton, pevent);
        }

        // ترسیم متن + آیکون خاکستری در حالت غیرفعال
        private void DrawDisabledContent(Graphics g, int w, int h, Color textColor)
        {
            // آیکون خاکستری — بر اساس ImageAlign
            Image img = Image;
            if (img != null && img.Width > 0 && img.Height > 0)
            {
                int pad = Math.Max(Padding.Horizontal / 2, 2);
                Rectangle bounds = new Rectangle(pad, pad, w - 2 * pad, h - 2 * pad);
                if (bounds.Width > 0 && bounds.Height > 0)
                {
                    float scale = Math.Min(
                        (float)bounds.Width / img.Width,
                        (float)bounds.Height / img.Height);
                    int w2 = Math.Max(1, (int)(img.Width * scale));
                    int h2 = Math.Max(1, (int)(img.Height * scale));

                    Point pos = CalcAlignmentPosition(bounds, w2, h2, ImageAlign);
                    using (Image gray = CreateGrayImage(img))
                    {
                        g.DrawImage(gray, new Rectangle(pos, new Size(w2, h2)));
                    }
                }
            }

            // متن غیرفعال
            TextRenderer.DrawText(
                g,
                Text,
                Font,
                ClientRectangle,
                textColor,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.NoPadding);
        }

        // محاسبه موقعیت یک مستطیل w2×h2 داخل bounds بر اساس ContentAlignment
        private static Point CalcAlignmentPosition(Rectangle bounds, int w2, int h2, ContentAlignment alignment)
        {
            int x, y;

            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    x = bounds.Left;
                    break;
                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.BottomRight:
                    x = bounds.Right - w2;
                    break;
                default: // TopCenter / MiddleCenter / BottomCenter
                    x = bounds.Left + (bounds.Width - w2) / 2;
                    break;
            }

            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.TopCenter:
                case ContentAlignment.TopRight:
                    y = bounds.Top;
                    break;
                case ContentAlignment.BottomLeft:
                case ContentAlignment.BottomCenter:
                case ContentAlignment.BottomRight:
                    y = bounds.Bottom - h2;
                    break;
                default: // MiddleLeft / MiddleCenter / MiddleRight
                    y = bounds.Top + (bounds.Height - h2) / 2;
                    break;
            }

            return new Point(x, y);
        }

        // نسخه خاکستری آیکون برای حالت غیرفعال
        private static Image CreateGrayImage(Image source)
        {
            Image gray = new Bitmap(source.Width, source.Height);
            using (Graphics g = Graphics.FromImage(gray))
            {
                ColorMatrix matrix = new ColorMatrix(new float[][]
                {
                    new float[] {0.33f, 0.33f, 0.33f, 0, 0},
                    new float[] {0.33f, 0.33f, 0.33f, 0, 0},
                    new float[] {0.33f, 0.33f, 0.33f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                });

                using (ImageAttributes attrs = new ImageAttributes())
                {
                    attrs.SetColorMatrix(matrix);
                    Rectangle r = new Rectangle(0, 0, source.Width, source.Height);
                    g.DrawImage(source, r, 0, 0, source.Width, source.Height,
                                GraphicsUnit.Pixel, attrs);
                }
            }
            return gray;
        }

        private class TransparentControl : Control
        {
            protected override void OnPaintBackground(PaintEventArgs pevent)
            {
            }

            protected override void OnPaint(PaintEventArgs e)
            {
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // ترسیم کامل پس‌زمینه در OnPaint انجام می‌شود
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (facePath != null)
                {
                    facePath.Dispose();
                    facePath = null;
                }
                if (imageButton != null)
                {
                    imageButton.Dispose();
                    imageButton = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}
