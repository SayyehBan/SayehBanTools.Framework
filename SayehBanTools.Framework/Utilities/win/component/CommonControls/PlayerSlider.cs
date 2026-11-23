using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.component.CommonControls
{
    /// <summary>
    /// نوار باریک پخش‌کننده (مثل نوار زمان یوتیوب) برای زمان فیلم و صدا.
    /// با کلیک به هر نقطه می‌پرد و با کشیدن، مقدار را لحظه‌ای گزارش می‌کند.
    /// </summary>
    [DefaultEvent("ValueCommitted")]
    public class PlayerSlider : Control
    {
        private long _maximum = 1000;
        private long _value;
        private bool _hover;
        private bool _dragging;

        /// <summary>هنگام کشیدن (لحظه‌ای)</summary>
        public event EventHandler Scrubbing;

        /// <summary>رها کردن ماوس یا کلیک: مقدار نهایی انتخاب شد</summary>
        public event EventHandler ValueCommitted;

        public PlayerSlider()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint
                     | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Selectable, false);
            TabStop = false;
            Cursor = Cursors.Hand;
            BackColor = Color.FromArgb(24, 24, 24);
            Size = new Size(200, 22);
        }

        [DefaultValue(1000L)]
        public long Maximum
        {
            get => _maximum;
            set { _maximum = Math.Max(1, value); if (_value > _maximum) _value = _maximum; Invalidate(); }
        }

        [DefaultValue(0L)]
        public long Value
        {
            get => _value;
            set
            {
                long v = Math.Max(0, Math.Min(_maximum, value));
                if (v == _value) return;
                _value = v;
                Invalidate();
            }
        }

        /// <summary>در حال کشیدن توسط کاربر است (در این حالت نباید از بیرون Value را عوض کرد)</summary>
        [Browsable(false)]
        public bool IsDragging => _dragging;

        [DefaultValue(typeof(Color), "229, 57, 53")]
        public Color ProgressColor { get; set; } = Color.FromArgb(229, 57, 53);

        [DefaultValue(typeof(Color), "70, 70, 70")]
        public Color TrackColor { get; set; } = Color.FromArgb(70, 70, 70);

        [DefaultValue(4)]
        public int TrackHeight { get; set; } = 4;

        /// <summary>دکمه گرد همیشه دیده شود (برای نوار صدا)</summary>
        [DefaultValue(false)]
        public bool AlwaysShowThumb { get; set; }

        private const int Pad = 7;

        private Rectangle TrackBounds
        {
            get
            {
                int h = (_hover || _dragging) ? TrackHeight + 2 : TrackHeight;
                return new Rectangle(Pad, (Height - h) / 2, Math.Max(1, Width - Pad * 2), h);
            }
        }

        private long ValueFromX(int x)
        {
            Rectangle r = TrackBounds;
            double ratio = (x - r.Left) / (double)r.Width;
            ratio = Math.Max(0, Math.Min(1, ratio));
            return (long)Math.Round(ratio * _maximum);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle r = TrackBounds;

            using (var track = new SolidBrush(TrackColor))
                g.FillRectangle(track, r);

            int filled = (int)Math.Round(r.Width * (_value / (double)_maximum));
            if (filled > 0)
                using (var prog = new SolidBrush(Enabled ? ProgressColor : Color.Gray))
                    g.FillRectangle(prog, new Rectangle(r.Left, r.Top, filled, r.Height));

            if (_hover || _dragging || AlwaysShowThumb)
            {
                int d = r.Height + 8;
                var thumb = new Rectangle(r.Left + filled - d / 2, r.Top + r.Height / 2 - d / 2, d, d);
                using (var b = new SolidBrush(Enabled ? ProgressColor : Color.Gray))
                    g.FillEllipse(b, thumb);
            }
        }

        protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }

        protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Left || !Enabled) return;
            _dragging = true;
            Capture = true;
            Value = ValueFromX(e.X);
            Scrubbing?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!_dragging) return;
            Value = ValueFromX(e.X);
            Scrubbing?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (!_dragging) return;
            _dragging = false;
            Capture = false;
            Value = ValueFromX(e.X);
            Invalidate();
            ValueCommitted?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnMouseCaptureChanged(EventArgs e)
        {
            base.OnMouseCaptureChanged(e);
            if (_dragging && !Capture)
            {
                _dragging = false;
                Invalidate();
                ValueCommitted?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}