using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.component.CommonControls
{
    public class PanelGlass : Panel
    {
        private Color glassColor = Color.CadetBlue;
        private string title = string.Empty;
        private Font titleFont = new Font("Segoe UI", 10, FontStyle.Bold);
        private Color titleColor = Color.White;
        private HorizontalAlignment titleAlignment = HorizontalAlignment.Left; // ویژگی جدید جهت‌دهی

        public PanelGlass()
        {
            // جلوگیری از فلیکر و بهبود رندرینگ
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            UpdateStyles();
        }

        [Category("Appearance")]
        [Description("رنگ اصلی افکت شیشه‌ای پنل")]
        [DefaultValue(typeof(Color), "0x5f9ea0")]
        public Color GlassColor
        {
            get => glassColor;
            set
            {
                glassColor = value;
                DrawGlassPanel();
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("عنوان پنل (اگر خالی باشد، هدر نمایش داده نمی‌شود)")]
        public string Title
        {
            get => title;
            set
            {
                title = value;
                DrawGlassPanel();
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("فونت عنوان پنل")]
        public Font TitleFont
        {
            get => titleFont;
            set
            {
                titleFont = value;
                DrawGlassPanel();
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("رنگ متن عنوان پنل")]
        public Color TitleColor
        {
            get => titleColor;
            set
            {
                titleColor = value;
                DrawGlassPanel();
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("جهت‌دهی متن عنوان (چپ، وسط، راست)")]
        [DefaultValue(HorizontalAlignment.Left)]
        public HorizontalAlignment TitleAlignment
        {
            get => titleAlignment;
            set
            {
                titleAlignment = value;
                DrawGlassPanel();
                Invalidate();
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            BackColor = Color.Transparent;
            BackgroundImageLayout = ImageLayout.None;
            DrawGlassPanel();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (Width > 20 && Height > 20)
            {
                DrawGlassPanel();
                Invalidate();
            }
        }

        // اگر پراپرتی RightToLeft تغییر کرد، دوباره رسم شود
        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            DrawGlassPanel();
            Invalidate();
        }

        private GraphicsPath DrawRoundRect(float x, float y, float width, float height, float radius)
        {
            GraphicsPath gp = new GraphicsPath();
            if (width <= 0 || height <= 0) return gp;

            float r2 = radius * 2;
            gp.AddLine(x + radius, y, x + width - r2, y);
            gp.AddArc(x + width - r2, y, r2, r2, 270, 90);
            gp.AddLine(x + width, y + radius, x + width, y + height - r2);
            gp.AddArc(x + width - r2, y + height - r2, r2, r2, 0, 90);
            gp.AddLine(x + width - r2, y + height, x + radius, y + height);
            gp.AddArc(x, y + height - r2, r2, r2, 90, 90);
            gp.AddLine(x, y + height - r2, x, y + radius);
            gp.AddArc(x, y, r2, r2, 180, 90);
            gp.CloseFigure();
            return gp;
        }

        public void DrawGlassPanel()
        {
            if (Width < 20 || Height < 20) return;

            Bitmap b = new Bitmap(Width, Height);
            using (Graphics gr = Graphics.FromImage(b))
            {
                gr.SmoothingMode = SmoothingMode.AntiAlias;
                gr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                Rectangle boundsRect = new Rectangle(0, 0, Width, Height);

                // 1. رسم سایه نرم دور پنل
                for (int i = 0; i < 6; i++)
                {
                    using (GraphicsPath gpShadow = DrawRoundRect(i, i, Width - (i * 2) - 1, Height - (i * 2) - 1, 8))
                    {
                        using (LinearGradientBrush brShadow = new LinearGradientBrush(boundsRect, Color.FromArgb(i * 4, Color.Black), Color.FromArgb(i * 4, Color.Black), LinearGradientMode.Vertical))
                        {
                            using (Pen pShadow = new Pen(brShadow, 2))
                            {
                                gr.DrawPath(pShadow, gpShadow);
                            }
                        }
                    }
                }

                // 2. بدنه اصلی شیشه‌ای
                using (GraphicsPath gpMain = DrawRoundRect(6, 6, Width - 13, Height - 13, 8))
                {
                    using (LinearGradientBrush brWhite = new LinearGradientBrush(boundsRect, Color.FromArgb(35, Color.White), Color.FromArgb(15, Color.White), LinearGradientMode.Vertical))
                    {
                        gr.FillPath(brWhite, gpMain);
                    }

                    using (LinearGradientBrush brGlass = new LinearGradientBrush(boundsRect, Color.FromArgb(60, glassColor), Color.FromArgb(90, glassColor), LinearGradientMode.Vertical))
                    {
                        gr.FillPath(brGlass, gpMain);
                    }
                }

                // 3. اگر Title مقدار داشت، هدر بالا رسم شود
                int headerHeight = 0;
                if (!string.IsNullOrEmpty(title))
                {
                    headerHeight = 32;
                    using (GraphicsPath gpHeader = DrawRoundRect(7, 7, Width - 15, headerHeight, 6))
                    {
                        using (LinearGradientBrush brHeader = new LinearGradientBrush(
                            new Rectangle(7, 7, Width, headerHeight),
                            Color.FromArgb(50, Color.White),
                            Color.FromArgb(10, Color.Black),
                            LinearGradientMode.Vertical))
                        {
                            gr.FillPath(brHeader, gpHeader);
                        }
                    }

                    // تنظیمات چیدمان متن (Alignment)
                    using (Brush textBrush = new SolidBrush(titleColor))
                    {
                        StringFormat sf = new StringFormat
                        {
                            LineAlignment = StringAlignment.Center // تراز عمودی همیشه وسط
                        };

                        // تنظیم تراز افقی بر اساس ویژگی TitleAlignment
                        switch (titleAlignment)
                        {
                            case HorizontalAlignment.Left:
                                sf.Alignment = StringAlignment.Near;
                                break;
                            case HorizontalAlignment.Center:
                                sf.Alignment = StringAlignment.Center;
                                break;
                            case HorizontalAlignment.Right:
                                sf.Alignment = StringAlignment.Far;
                                break;
                        }

                        // پشتیبانی از زبان‌های راست‌چین (RTL)
                        if (RightToLeft == RightToLeft.Yes)
                        {
                            sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
                        }

                        gr.DrawString(title, titleFont, textBrush, new RectangleF(15, 7, Width - 30, headerHeight), sf);
                    }

                    // خط جداکننده زیر هدر
                    using (Pen linePen = new Pen(Color.FromArgb(40, Color.White)))
                    {
                        gr.DrawLine(linePen, 8, 7 + headerHeight, Width - 9, 7 + headerHeight);
                    }
                }

                // 4. حاشیه داخلی و بیرونی شیشه
                using (GraphicsPath gpInnerBorder = DrawRoundRect(6, 6, Width - 13, Height - 13, 8))
                {
                    using (Pen pInner = new Pen(Color.FromArgb(180, Color.White)))
                    {
                        gr.DrawPath(pInner, gpInnerBorder);
                    }
                }

                using (GraphicsPath gpOuterBorder = DrawRoundRect(5, 5, Width - 11, Height - 11, 8))
                {
                    using (Pen pOuter = new Pen(Color.FromArgb(120, Color.Black)))
                    {
                        gr.DrawPath(pOuter, gpOuterBorder);
                    }
                }

                // 5. افکت نور گوشه‌ها
                using (GraphicsPath gpGlow = DrawRoundRect(6, 6, Width - 13, Height - 13, 8))
                {
                    using (PathGradientBrush brGlow1 = new PathGradientBrush(gpGlow))
                    {
                        brGlow1.CenterPoint = new PointF(10f, 10f);
                        brGlow1.CenterColor = Color.FromArgb(70, Color.White);
                        brGlow1.SurroundColors = new Color[] { Color.Transparent };
                        gr.FillPath(brGlow1, gpGlow);
                    }
                }
            }

            BackgroundImage = b;
        }
    }
}