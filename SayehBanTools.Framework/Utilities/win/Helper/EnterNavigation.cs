using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.Helper
{
    /// <summary>
    /// حرکت با Enter در همه فرم‌ها (بدون نیاز به Tab یا موس):
    /// - کادر متن (تک‌خطی)، چک‌باکس، رادیوباتن، لیست کشویی، تاریخ و عدد: با Enter به کنترل بعدی (به ترتیب TabIndex) می‌رود،
    ///   فقط برای کنترل‌هایی که خودشان رویداد KeyDown ندارند (کنترل‌هایی که KeyDown دارند، مثل جستجو با Enter، دست نمی‌خورند).
    /// - cmbbox سایه‌بان خودش EnterToTab دارد و تغییر نمی‌کند.
    /// - DGV سایه‌بان (جدول ویرایشی، ReadOnly نیست): GONextCell روشن می‌شود تا Enter به خانه بعدی برود.
    /// - استثنا: اگر Tag کنترل شامل «NoEnterMove» باشد، کاری با آن نمی‌شود.
    /// به همه فرم‌هایی که با AddFittedForm ثبت می‌شوند (فرم‌های فعلی و فرم‌های بعدی) خودکار وصل می‌شود.
    /// </summary>
    public static class EnterNavigation
    {
        public const string SkipTag = "NoEnterMove";

        private static readonly object EventKeyDownKey =
            typeof(Control).GetField("EventKeyDown", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null);

        private static readonly PropertyInfo EventsProperty =
            typeof(Component).GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);

        public static void Attach(Form form)
        {
            if (form == null) return;
            form.Load += (s, e) => Apply(form);
        }

        /// <summary>همه کنترل‌های فرم (و کنترل‌هایی که بعداً اضافه می‌شوند) را تنظیم می‌کند</summary>
        public static void Apply(Control root)
        {
            if (root == null) return;
            foreach (Control c in root.Controls)
            {
                Setup(c);
                Apply(c);
            }
            root.ControlAdded -= Root_ControlAdded;
            root.ControlAdded += Root_ControlAdded;
        }

        private static void Root_ControlAdded(object sender, ControlEventArgs e)
        {
            Setup(e.Control);
            Apply(e.Control);
        }

        private static bool Skip(Control c) => c.Tag is string tag && tag.IndexOf(SkipTag, StringComparison.OrdinalIgnoreCase) >= 0;

        /// <summary>آیا کنترل رویداد KeyDown دارد (مثلاً FocuseMoveControl یا جستجو با Enter)</summary>
        private static bool HasKeyDown(Control c)
        {
            if (EventKeyDownKey == null || EventsProperty == null) return true;   // اگر بررسی ممکن نبود، دست نزن
            var list = EventsProperty.GetValue(c) as EventHandlerList;
            return list?[EventKeyDownKey] != null;
        }

        private static void Setup(Control c)
        {
            if (c == null) return;

            // همه جدول‌ها (DGV، DGVPagin و DataGridView): داده‌ها راست‌چین
            if (c is DataGridView anyGrid) GridRightAlign.Attach(anyGrid);

            if (Skip(c)) return;

            // جدول ویرایشی سایه‌بان: Enter = خانه بعدی
            if (c is SayehBanTools.Framework.Utilities.win.component.CommonControls.DGV grid)
            {
                if (!grid.ReadOnly) grid.GONextCell = true;
                return;
            }

            // cmbbox سایه‌بان خودش با Enter به کنترل بعدی می‌رود (EnterToTab)
            if (c is SayehBanTools.Framework.Utilities.win.component.CommonControls.cmbbox) return;

            bool supported =
                (c is TextBoxBase tb && !(tb.Multiline && (tb as TextBox)?.AcceptsReturn == true)) ||
                c is CheckBox || c is RadioButton || c is ComboBox || c is DateTimePicker || c is NumericUpDown;
            if (!supported || HasKeyDown(c)) return;

            c.KeyDown += FocuseMoveControl;
        }

        /// <summary>
        /// همان FocuseMoveControl فرم‌ها: Enter = رفتن به کنترل بعدی.
        /// به جای SendKeys از SelectNextControl استفاده شده (همان کار Tab، بدون وابستگی به فوکوس پنجره).
        /// </summary>
        public static void FocuseMoveControl(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter || e.Alt || e.Control || !(sender is Control c)) return;
            // اگر فرم بعداً (مثلاً بعد از await در Load) KeyDown خودش را اضافه کرده، کار را به همان می‌سپاریم
            var list = (EventKeyDownKey != null ? EventsProperty?.GetValue(c) as EventHandlerList : null);
            if (list != null && list[EventKeyDownKey] is Delegate d && d.GetInvocationList().Length > 1) return;
            if (c is ComboBox cb && cb.DroppedDown) cb.DroppedDown = false;   // لیست باز: اول انتخاب، بعد رفتن
            e.Handled = true;
            e.SuppressKeyPress = true;
            MoveNext(c);
        }

        public static void MoveNext(Control c)
        {
            Control container = c.FindForm() ?? c.Parent;
            container?.SelectNextControl(c, true, true, true, true);
        }
    }
}