using LibVLCSharp.Shared;
using SayehBanTools.Framework.Utilities.win.Helper;
using System;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.Learning
{
    /// <summary>
    /// پخش‌کننده سراسری فیلم‌های آموزشی (موتور VLC: تقریباً همه فرمت‌ها).
    /// محل توقف هر فیلم ذخیره می‌شود و دفعه بعد از همان‌جا ادامه می‌دهد.
    /// از فرم‌ها مستقیم باز نکنید؛ از <see cref="LearningVideo.Show"/> استفاده کنید.
    /// موتور VLC به صورت تنبل (Lazy) و فقط هنگام باز شدن پخش‌کننده مقداردهی می‌شود؛ اگر پوشه libvlc
    /// کنار برنامه نباشد، پیام مناسب می‌دهد و بقیه کتابخانه بدون مشکل کار می‌کند.
    /// </summary>
    public partial class FrmLearningPlayer : Form
    {
        private static LibVLC _libVlc;
        private static FrmLearningPlayer _current;
        private static readonly float[] Rates = { 0.5f, 0.75f, 1f, 1.25f, 1.5f, 1.75f, 2f };

        private readonly string _file;
        private readonly string _title;
        private MediaPlayer _player;
        private Media _media;

        /// <summary>لوگوی برنامه (اختیاری، آرایه بایت تصویر) برای آیکون پنجره</summary>
        public static byte[] AppLogo { get; set; }

        // کارهای کند VLC (Stop) روی نخ پس‌زمینه و پشت سر هم اجرا می‌شوند تا فرم قفل نکند
        private Task _background = Task.FromResult(0);

        private bool _ended;
        private bool _muted;
        private bool _readyToClose;
        private bool _fullScreen;
        private FormBorderStyle _prevBorder;
        private FormWindowState _prevState;
        private int _saveTicks;
        private int _infoTicks;
        private long _lastSavedMs = -1;

        public FrmLearningPlayer()
        {
            InitializeComponent();
        }

        private FrmLearningPlayer(string file, string title) : this()
        {
            _file = file;
            _title = title;
        }

        /// <summary>
        /// پخش فیلم. اگر پخش‌کننده‌ای باز باشد بسته می‌شود (محلش ذخیره می‌شود) و پنجره تازه باز می‌شود،
        /// تا اگر از داخل یک فرم مودال صدا زده شد، پنجره قابل استفاده باشد.
        /// </summary>
        public static void Open(string file, string title)
        {
            if (!EnsureEngine()) return;

            if (_current != null && !_current.IsDisposed)
                _current.Close();

            var frm = new FrmLearningPlayer(file, title);
            _current = frm;
            frm.Show();
            frm.Activate();
        }

        private static bool EnsureEngine()
        {
            if (_libVlc != null) return true;
            try
            {
                Core.Initialize();   // libvlc\win-x86 یا win-x64 کنار exe
                _libVlc = new LibVLC("--no-video-title-show", "--quiet");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("موتور پخش ویدیو (پوشه libvlc کنار برنامه) پیدا نشد.\n\n" + ex.Message,
                    "آموزشی", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading);
                return false;
            }
        }

        #region بارگذاری و بستن

        private void FrmLearningPlayer_Load(object sender, EventArgs e)
        {
            if (_file == null || _libVlc == null) return;   // حالت طراحی

            if (AppLogo != null && AppLogo.Length > 0) this.SetAppIcon(AppLogo);
            this.SetAppTitle(_title, "آموزش ");

            var settings = LearningProgress.Settings;
            sliderVolume.Value = Math.Max(0, Math.Min(100, settings.Volume));
            int rateIndex = Array.IndexOf(Rates, settings.Rate);
            CmbSpeed.SelectedIndex = rateIndex >= 0 ? rateIndex : Array.IndexOf(Rates, 1f);
            TopMost = settings.TopMost;
            UpdateTopMostButton();
            UpdateMuteButton();
            LblInfo.Text = _title;

            _player = new MediaPlayer(_libVlc)
            {
                EnableMouseInput = false,   // کلیک و کلیدها به فرم برسد، نه به VLC
                EnableKeyInput = false
            };
            _player.Playing += (s, a) => Ui(ApplyAudioAndRate);
            _player.EndReached += (s, a) => Ui(OnEnded);
            _player.EncounteredError += (s, a) => Ui(() => ShowInfo("خطا در پخش فیلم", 40));
            videoView1.MediaPlayer = _player;

            long resume = LearningProgress.GetResumePosition(_file);
            StartMedia(resume);
            if (resume > 0)
                ShowInfo("ادامه از جای قبلی: " + Fmt(resume), 40);

            timerUi.Start();
        }

        private void FrmLearningPlayer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_readyToClose || _player == null) return;

            SaveProgress(true);
            var settings = LearningProgress.Settings;
            settings.Volume = (int)sliderVolume.Value;
            settings.Rate = CurrentRate;
            settings.TopMost = TopMost;
            LearningProgress.Save();
            timerUi.Stop();
            if (_current == this) _current = null;

            MediaPlayer player = _player;
            Media media = _media;

            if (e.CloseReason != CloseReason.UserClosing)
            {
                // خروج از برنامه: لغو نمی‌کنیم؛ حداکثر کمی صبر برای توقف
                try { Task.Run(() => player.Stop()).Wait(1500); } catch { }
                return;
            }

            // Stop در VLC ممکن است منتظر نخ فرم بماند؛ پس پنجره را پنهان، در پس‌زمینه متوقف و بعد واقعاً می‌بندیم
            e.Cancel = true;
            Hide();
            _background = _background.ContinueWith(_ =>
            {
                try { player.Stop(); } catch { }
                Ui(() =>
                {
                    videoView1.MediaPlayer = null;
                    player.Dispose();
                    media?.Dispose();
                    _player = null;
                    _readyToClose = true;
                    Close();
                });
            });
        }

        #endregion

        #region پخش

        /// <summary>شروع (یا شروع دوباره) فیلم از یک نقطه</summary>
        private void StartMedia(long startMs)
        {
            var media = new Media(_libVlc, _file, FromType.FromPath);
            if (startMs > 0)
                media.AddOption(":start-time=" + (startMs / 1000.0).ToString("0.###", CultureInfo.InvariantCulture));

            Media old = _media;
            _media = media;
            _ended = false;
            MediaPlayer player = _player;

            if (old == null)
            {
                player.Play(media);
                return;
            }

            _background = _background.ContinueWith(_ =>
            {
                try
                {
                    player.Stop();
                    player.Play(media);
                }
                catch { }
                old.Dispose();
            });
        }

        private bool IsFinished => _ended || _player.State == VLCState.Ended || _player.State == VLCState.Stopped || _player.State == VLCState.Error;

        private void TogglePlay()
        {
            if (_player == null) return;
            if (IsFinished) { StartMedia(0); return; }
            _player.SetPause(_player.IsPlaying);
        }

        private void SeekTo(long ms)
        {
            if (_player == null) return;
            long len = _player.Length;
            if (len <= 0) return;
            ms = Math.Max(0, Math.Min(len - 500, ms));

            if (IsFinished) { StartMedia(ms); return; }
            if (_player.IsSeekable) _player.Time = ms;
            UpdateUi();
        }

        private void Skip(long deltaMs)
        {
            if (_player == null) return;
            SeekTo((IsFinished ? _player.Length : _player.Time) + deltaMs);
        }

        private void OnEnded()
        {
            if (_player == null) return;
            _ended = true;
            LearningProgress.SavePosition(_file, 0, _player.Length);   // تا آخر دیده شد: دفعه بعد از اول
            _lastSavedMs = 0;
            ShowInfo("پایان فیلم؛ برای دیدن دوباره، پخش را بزنید", 0);
            UpdateUi();
        }

        private float CurrentRate => CmbSpeed.SelectedIndex >= 0 ? Rates[CmbSpeed.SelectedIndex] : 1f;

        private void ApplyAudioAndRate()
        {
            if (_player == null) return;
            _player.Volume = (int)sliderVolume.Value;
            _player.Mute = _muted;
            _player.SetRate(CurrentRate);
        }

        private void SaveProgress(bool force = false)
        {
            if (_player == null || _ended) return;
            long len = _player.Length;
            long pos = _player.Time;
            if (len <= 0 || pos < 0) return;
            if (!force && Math.Abs(pos - _lastSavedMs) < 1000) return;   // تغییری نکرده (مکث)
            LearningProgress.SavePosition(_file, pos, len);
            _lastSavedMs = pos;
        }

        #endregion

        #region رابط کاربری

        private void timerUi_Tick(object sender, EventArgs e)
        {
            UpdateUi();

            if (++_saveTicks >= 20)   // هر ۵ ثانیه
            {
                _saveTicks = 0;
                SaveProgress();
            }

            if (_infoTicks > 0 && --_infoTicks == 0)
                LblInfo.Text = _title;
        }

        private void UpdateUi()
        {
            if (_player == null) return;
            long len = _player.Length;
            long time = _ended ? len : _player.Time;

            if (len > 0)
            {
                sliderSeek.Maximum = len;
                if (!sliderSeek.IsDragging) sliderSeek.Value = Math.Max(0, time);
            }

            long shown = sliderSeek.IsDragging ? sliderSeek.Value : time;
            LblTime.Text = Fmt(shown) + " / " + Fmt(len);
            BtnPlay.Text = _player.IsPlaying ? "⏸" : "▶";
        }

        /// <param name="ticks">چند تیک تایمر (۲۵۰ میلی‌ثانیه) بماند؛ صفر = بماند</param>
        private void ShowInfo(string text, int ticks)
        {
            LblInfo.Text = text;
            _infoTicks = ticks;
        }

        private void UpdateMuteButton()
        {
            BtnMute.Text = _muted || sliderVolume.Value == 0 ? "🔇" : "🔊";
        }

        private void UpdateTopMostButton()
        {
            BtnTopMost.BackColor = TopMost ? Color.FromArgb(229, 57, 53) : panelControls.BackColor;
        }

        private void SetVolume(long volume)
        {
            volume = Math.Max(0, Math.Min(100, volume));
            sliderVolume.Value = volume;
            if (volume > 0) _muted = false;
            if (_player != null)
            {
                _player.Volume = (int)volume;
                _player.Mute = _muted;
            }
            UpdateMuteButton();
            ShowInfo("صدا: " + volume + "٪", 6);
        }

        private void ToggleFullScreen()
        {
            if (!_fullScreen)
            {
                _prevBorder = FormBorderStyle;
                _prevState = WindowState;
                if (WindowState == FormWindowState.Maximized) WindowState = FormWindowState.Normal;
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
                _fullScreen = true;
            }
            else
            {
                FormBorderStyle = _prevBorder;
                WindowState = _prevState;
                _fullScreen = false;
            }
        }

        private static string Fmt(long ms)
        {
            if (ms < 0) ms = 0;
            TimeSpan t = TimeSpan.FromMilliseconds(ms);
            return t.TotalHours >= 1 ? t.ToString(@"h\:mm\:ss") : t.ToString(@"mm\:ss");
        }

        /// <summary>اجرای کد روی نخ فرم (رویدادهای VLC از نخ دیگری می‌آیند)</summary>
        private void Ui(Action action)
        {
            if (IsDisposed || !IsHandleCreated) return;
            try { BeginInvoke(action); }
            catch (InvalidOperationException) { }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Space:
                case Keys.K:
                    TogglePlay(); return true;
                case Keys.Left:
                    Skip(-5000); return true;
                case Keys.Right:
                    Skip(5000); return true;
                case Keys.Control | Keys.Left:
                    Skip(-30000); return true;
                case Keys.Control | Keys.Right:
                    Skip(30000); return true;
                case Keys.Up:
                    SetVolume(sliderVolume.Value + 5); return true;
                case Keys.Down:
                    SetVolume(sliderVolume.Value - 5); return true;
                case Keys.M:
                    BtnMute_Click(this, EventArgs.Empty); return true;
                case Keys.Home:
                    BtnRestart_Click(this, EventArgs.Empty); return true;
                case Keys.F:
                case Keys.F11:
                    ToggleFullScreen(); return true;
                case Keys.Escape:
                    if (_fullScreen) ToggleFullScreen(); else Close();
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region رویدادهای کنترل‌ها

        private void BtnPlay_Click(object sender, EventArgs e) => TogglePlay();

        private void videoView1_Click(object sender, EventArgs e) => TogglePlay();

        private void videoView1_DoubleClick(object sender, EventArgs e)
        {
            TogglePlay();   // کلیک اول دوبار‌کلیک، پخش را عوض کرده بود؛ برمی‌گردانیم
            ToggleFullScreen();
        }

        private void BtnRestart_Click(object sender, EventArgs e)
        {
            if (_player == null) return;
            if (IsFinished) StartMedia(0);
            else
            {
                _player.Time = 0;
                _player.SetPause(false);
            }
            ShowInfo(_title, 0);
        }

        private void BtnBack_Click(object sender, EventArgs e) => Skip(-10000);

        private void BtnForward_Click(object sender, EventArgs e) => Skip(10000);

        private void sliderSeek_Scrubbing(object sender, EventArgs e) => UpdateUi();

        private void sliderSeek_ValueCommitted(object sender, EventArgs e) => SeekTo(sliderSeek.Value);

        private void sliderVolume_ValueChanged(object sender, EventArgs e) => SetVolume(sliderVolume.Value);

        private void BtnMute_Click(object sender, EventArgs e)
        {
            _muted = !_muted;
            if (!_muted && sliderVolume.Value == 0) sliderVolume.Value = 50;
            ApplyAudioAndRate();
            UpdateMuteButton();
        }

        private void CmbSpeed_SelectedIndexChanged(object sender, EventArgs e)
        {
            _player?.SetRate(CurrentRate);
            ActiveControl = null;   // کلیدهای میان‌بر دوباره به پخش‌کننده برسد
        }

        private void BtnTopMost_Click(object sender, EventArgs e)
        {
            TopMost = !TopMost;
            UpdateTopMostButton();
        }

        private void BtnFullScreen_Click(object sender, EventArgs e) => ToggleFullScreen();

        #endregion
    }
}