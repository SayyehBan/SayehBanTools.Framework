namespace SayehBanTools.Framework.Utilities.win.Learning
{
    partial class FrmLearningPlayer
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.videoView1 = new LibVLCSharp.WinForms.VideoView();
            this.panelControls = new System.Windows.Forms.Panel();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.BtnRestart = new System.Windows.Forms.Button();
            this.BtnBack = new System.Windows.Forms.Button();
            this.BtnPlay = new System.Windows.Forms.Button();
            this.BtnForward = new System.Windows.Forms.Button();
            this.LblTime = new System.Windows.Forms.Label();
            this.LblInfo = new System.Windows.Forms.Label();
            this.CmbSpeed = new System.Windows.Forms.ComboBox();
            this.BtnMute = new System.Windows.Forms.Button();
            this.sliderVolume = new SayehBanTools.Framework.Utilities.win.Learning.PlayerSlider();
            this.BtnTopMost = new System.Windows.Forms.Button();
            this.BtnFullScreen = new System.Windows.Forms.Button();
            this.sliderSeek = new SayehBanTools.Framework.Utilities.win.Learning.PlayerSlider();
            this.timerUi = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.videoView1)).BeginInit();
            this.panelControls.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            //
            // videoView1
            //
            this.videoView1.BackColor = System.Drawing.Color.Black;
            this.videoView1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.videoView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.videoView1.Location = new System.Drawing.Point(0, 0);
            this.videoView1.MediaPlayer = null;
            this.videoView1.Name = "videoView1";
            this.videoView1.Size = new System.Drawing.Size(960, 524);
            this.videoView1.TabIndex = 0;
            this.videoView1.Click += new System.EventHandler(this.videoView1_Click);
            this.videoView1.DoubleClick += new System.EventHandler(this.videoView1_DoubleClick);
            //
            // panelControls
            //
            this.panelControls.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.panelControls.Controls.Add(this.panelButtons);
            this.panelControls.Controls.Add(this.sliderSeek);
            this.panelControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControls.Location = new System.Drawing.Point(0, 524);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(960, 76);
            this.panelControls.TabIndex = 1;
            //
            // panelButtons
            //
            this.panelButtons.Controls.Add(this.BtnRestart);
            this.panelButtons.Controls.Add(this.BtnBack);
            this.panelButtons.Controls.Add(this.BtnPlay);
            this.panelButtons.Controls.Add(this.BtnForward);
            this.panelButtons.Controls.Add(this.LblTime);
            this.panelButtons.Controls.Add(this.LblInfo);
            this.panelButtons.Controls.Add(this.CmbSpeed);
            this.panelButtons.Controls.Add(this.BtnMute);
            this.panelButtons.Controls.Add(this.sliderVolume);
            this.panelButtons.Controls.Add(this.BtnTopMost);
            this.panelButtons.Controls.Add(this.BtnFullScreen);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelButtons.Location = new System.Drawing.Point(0, 22);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(960, 54);
            this.panelButtons.TabIndex = 1;
            //
            // BtnRestart
            //
            this.BtnRestart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnRestart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.BtnRestart.FlatAppearance.BorderSize = 0;
            this.BtnRestart.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.BtnRestart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnRestart.Font = new System.Drawing.Font("Segoe UI Symbol", 14F);
            this.BtnRestart.ForeColor = System.Drawing.Color.White;
            this.BtnRestart.Location = new System.Drawing.Point(8, 8);
            this.BtnRestart.Name = "BtnRestart";
            this.BtnRestart.Size = new System.Drawing.Size(42, 38);
            this.BtnRestart.TabIndex = 0;
            this.BtnRestart.TabStop = false;
            this.BtnRestart.Text = "⏮";
            this.toolTip1.SetToolTip(this.BtnRestart, "پخش از اول (Home)");
            this.BtnRestart.UseVisualStyleBackColor = false;
            this.BtnRestart.Click += new System.EventHandler(this.BtnRestart_Click);
            //
            // BtnBack
            //
            this.BtnBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.BtnBack.FlatAppearance.BorderSize = 0;
            this.BtnBack.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.BtnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnBack.Font = new System.Drawing.Font("Segoe UI Symbol", 14F);
            this.BtnBack.ForeColor = System.Drawing.Color.White;
            this.BtnBack.Location = new System.Drawing.Point(52, 8);
            this.BtnBack.Name = "BtnBack";
            this.BtnBack.Size = new System.Drawing.Size(44, 38);
            this.BtnBack.TabIndex = 1;
            this.BtnBack.TabStop = false;
            this.BtnBack.Text = "⏪";
            this.toolTip1.SetToolTip(this.BtnBack, "۱۰ ثانیه عقب (فلش چپ: ۵ ثانیه)");
            this.BtnBack.UseVisualStyleBackColor = false;
            this.BtnBack.Click += new System.EventHandler(this.BtnBack_Click);
            //
            // BtnPlay
            //
            this.BtnPlay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnPlay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.BtnPlay.FlatAppearance.BorderSize = 0;
            this.BtnPlay.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.BtnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnPlay.Font = new System.Drawing.Font("Segoe UI Symbol", 16F);
            this.BtnPlay.ForeColor = System.Drawing.Color.White;
            this.BtnPlay.Location = new System.Drawing.Point(98, 6);
            this.BtnPlay.Name = "BtnPlay";
            this.BtnPlay.Size = new System.Drawing.Size(52, 42);
            this.BtnPlay.TabIndex = 2;
            this.BtnPlay.TabStop = false;
            this.BtnPlay.Text = "▶";
            this.toolTip1.SetToolTip(this.BtnPlay, "پخش / توقف (Space)");
            this.BtnPlay.UseVisualStyleBackColor = false;
            this.BtnPlay.Click += new System.EventHandler(this.BtnPlay_Click);
            //
            // BtnForward
            //
            this.BtnForward.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnForward.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.BtnForward.FlatAppearance.BorderSize = 0;
            this.BtnForward.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.BtnForward.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnForward.Font = new System.Drawing.Font("Segoe UI Symbol", 14F);
            this.BtnForward.ForeColor = System.Drawing.Color.White;
            this.BtnForward.Location = new System.Drawing.Point(152, 8);
            this.BtnForward.Name = "BtnForward";
            this.BtnForward.Size = new System.Drawing.Size(44, 38);
            this.BtnForward.TabIndex = 3;
            this.BtnForward.TabStop = false;
            this.BtnForward.Text = "⏩";
            this.toolTip1.SetToolTip(this.BtnForward, "۱۰ ثانیه جلو (فلش راست: ۵ ثانیه)");
            this.BtnForward.UseVisualStyleBackColor = false;
            this.BtnForward.Click += new System.EventHandler(this.BtnForward_Click);
            //
            // LblTime
            //
            this.LblTime.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.LblTime.ForeColor = System.Drawing.Color.Gainsboro;
            this.LblTime.Location = new System.Drawing.Point(202, 8);
            this.LblTime.Name = "LblTime";
            this.LblTime.Size = new System.Drawing.Size(160, 38);
            this.LblTime.TabIndex = 4;
            this.LblTime.Text = "00:00 / 00:00";
            this.LblTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // LblInfo
            //
            this.LblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LblInfo.AutoEllipsis = true;
            this.LblInfo.Font = new System.Drawing.Font("B Koodak", 11F, System.Drawing.FontStyle.Bold);
            this.LblInfo.ForeColor = System.Drawing.Color.Gainsboro;
            this.LblInfo.Location = new System.Drawing.Point(366, 8);
            this.LblInfo.Name = "LblInfo";
            this.LblInfo.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.LblInfo.Size = new System.Drawing.Size(266, 38);
            this.LblInfo.TabIndex = 5;
            this.LblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // CmbSpeed
            //
            this.CmbSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CmbSpeed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.CmbSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbSpeed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CmbSpeed.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.CmbSpeed.ForeColor = System.Drawing.Color.White;
            this.CmbSpeed.FormattingEnabled = true;
            this.CmbSpeed.Items.AddRange(new object[] {
            "0.5x",
            "0.75x",
            "1x",
            "1.25x",
            "1.5x",
            "1.75x",
            "2x"});
            this.CmbSpeed.Location = new System.Drawing.Point(638, 13);
            this.CmbSpeed.Name = "CmbSpeed";
            this.CmbSpeed.Size = new System.Drawing.Size(68, 25);
            this.CmbSpeed.TabIndex = 6;
            this.CmbSpeed.TabStop = false;
            this.toolTip1.SetToolTip(this.CmbSpeed, "سرعت پخش");
            this.CmbSpeed.SelectedIndexChanged += new System.EventHandler(this.CmbSpeed_SelectedIndexChanged);
            //
            // BtnMute
            //
            this.BtnMute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnMute.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnMute.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.BtnMute.FlatAppearance.BorderSize = 0;
            this.BtnMute.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.BtnMute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnMute.Font = new System.Drawing.Font("Segoe UI Symbol", 13F);
            this.BtnMute.ForeColor = System.Drawing.Color.White;
            this.BtnMute.Location = new System.Drawing.Point(710, 8);
            this.BtnMute.Name = "BtnMute";
            this.BtnMute.Size = new System.Drawing.Size(42, 38);
            this.BtnMute.TabIndex = 7;
            this.BtnMute.TabStop = false;
            this.BtnMute.Text = "🔊";
            this.toolTip1.SetToolTip(this.BtnMute, "قطع / وصل صدا (M) — فلش بالا و پایین: بلندی صدا");
            this.BtnMute.UseVisualStyleBackColor = false;
            this.BtnMute.Click += new System.EventHandler(this.BtnMute_Click);
            //
            // sliderVolume
            //
            this.sliderVolume.AlwaysShowThumb = true;
            this.sliderVolume.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sliderVolume.Location = new System.Drawing.Point(754, 16);
            this.sliderVolume.Maximum = ((long)(100));
            this.sliderVolume.Name = "sliderVolume";
            this.sliderVolume.ProgressColor = System.Drawing.Color.Gainsboro;
            this.sliderVolume.Size = new System.Drawing.Size(100, 22);
            this.sliderVolume.TabIndex = 8;
            this.sliderVolume.Value = ((long)(100));
            this.sliderVolume.Scrubbing += new System.EventHandler(this.sliderVolume_ValueChanged);
            this.sliderVolume.ValueCommitted += new System.EventHandler(this.sliderVolume_ValueChanged);
            //
            // BtnTopMost
            //
            this.BtnTopMost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnTopMost.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnTopMost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.BtnTopMost.FlatAppearance.BorderSize = 0;
            this.BtnTopMost.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.BtnTopMost.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnTopMost.Font = new System.Drawing.Font("Segoe UI Symbol", 13F);
            this.BtnTopMost.ForeColor = System.Drawing.Color.White;
            this.BtnTopMost.Location = new System.Drawing.Point(862, 8);
            this.BtnTopMost.Name = "BtnTopMost";
            this.BtnTopMost.Size = new System.Drawing.Size(44, 38);
            this.BtnTopMost.TabIndex = 9;
            this.BtnTopMost.TabStop = false;
            this.BtnTopMost.Text = "📌";
            this.toolTip1.SetToolTip(this.BtnTopMost, "همیشه روی پنجره‌های دیگر (برای کار هم‌زمان با فرم)");
            this.BtnTopMost.UseVisualStyleBackColor = false;
            this.BtnTopMost.Click += new System.EventHandler(this.BtnTopMost_Click);
            //
            // BtnFullScreen
            //
            this.BtnFullScreen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnFullScreen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnFullScreen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.BtnFullScreen.FlatAppearance.BorderSize = 0;
            this.BtnFullScreen.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.BtnFullScreen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnFullScreen.Font = new System.Drawing.Font("Segoe UI Symbol", 14F);
            this.BtnFullScreen.ForeColor = System.Drawing.Color.White;
            this.BtnFullScreen.Location = new System.Drawing.Point(908, 8);
            this.BtnFullScreen.Name = "BtnFullScreen";
            this.BtnFullScreen.Size = new System.Drawing.Size(44, 38);
            this.BtnFullScreen.TabIndex = 10;
            this.BtnFullScreen.TabStop = false;
            this.BtnFullScreen.Text = "⛶";
            this.toolTip1.SetToolTip(this.BtnFullScreen, "تمام صفحه (F یا دوبار کلیک روی فیلم، خروج با Esc)");
            this.BtnFullScreen.UseVisualStyleBackColor = false;
            this.BtnFullScreen.Click += new System.EventHandler(this.BtnFullScreen_Click);
            //
            // sliderSeek
            //
            this.sliderSeek.Dock = System.Windows.Forms.DockStyle.Top;
            this.sliderSeek.Location = new System.Drawing.Point(0, 0);
            this.sliderSeek.Name = "sliderSeek";
            this.sliderSeek.Size = new System.Drawing.Size(960, 22);
            this.sliderSeek.TabIndex = 0;
            this.sliderSeek.Scrubbing += new System.EventHandler(this.sliderSeek_Scrubbing);
            this.sliderSeek.ValueCommitted += new System.EventHandler(this.sliderSeek_ValueCommitted);
            //
            // timerUi
            //
            this.timerUi.Interval = 250;
            this.timerUi.Tick += new System.EventHandler(this.timerUi_Tick);
            //
            // FrmLearningPlayer
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(960, 600);
            this.Controls.Add(this.videoView1);
            this.Controls.Add(this.panelControls);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(640, 400);
            this.Name = "FrmLearningPlayer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "آموزشی";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmLearningPlayer_FormClosing);
            this.Load += new System.EventHandler(this.FrmLearningPlayer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.videoView1)).EndInit();
            this.panelControls.ResumeLayout(false);
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private LibVLCSharp.WinForms.VideoView videoView1;
        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.Panel panelButtons;
        private SayehBanTools.Framework.Utilities.win.Learning.PlayerSlider sliderSeek;
        private System.Windows.Forms.Button BtnRestart;
        private System.Windows.Forms.Button BtnBack;
        private System.Windows.Forms.Button BtnPlay;
        private System.Windows.Forms.Button BtnForward;
        private System.Windows.Forms.Label LblTime;
        private System.Windows.Forms.Label LblInfo;
        private System.Windows.Forms.ComboBox CmbSpeed;
        private System.Windows.Forms.Button BtnMute;
        private SayehBanTools.Framework.Utilities.win.Learning.PlayerSlider sliderVolume;
        private System.Windows.Forms.Button BtnTopMost;
        private System.Windows.Forms.Button BtnFullScreen;
        private System.Windows.Forms.Timer timerUi;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}