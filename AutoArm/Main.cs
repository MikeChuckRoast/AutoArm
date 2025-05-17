namespace AutoArm
{
    public partial class Main : Form
    {
        public int udpPort
        {
            get { return int.Parse(udpPortTextBox.Text); }
        }

        public int delay
        {
            get { return int.Parse(delayTextBox.Text); }
        }

        private NotifyIcon notifyIcon;
        private ContextMenuStrip notifyIconContextMenu;

        private Rectangle buttonWatchRegion;

        private LynxInterface? lynxInterface;

        public LynxButtonWatcher? lynxButtonWatcher;

        public Main()
        {
            InitializeComponent();
            LoadSettings();
            UpdateUiControls(false);
            InitializeNotifyIcon();
            InitializeButtonWatcher();
        }

        #region NotifyIcon
        private void InitializeNotifyIcon()
        {
            // Create a context menu for the NotifyIcon
            notifyIconContextMenu = new ContextMenuStrip();
            notifyIconContextMenu.Items.Add("Settings", null, ShowMainForm);
            notifyIconContextMenu.Items.Add("Exit", null, ExitApplication);

            // Create and configure the NotifyIcon
            notifyIcon = new NotifyIcon
            {
                Icon = new Icon("Resources/ArmDisabled.ico"),
                Visible = true,
                Text = "AutoArm Notification",
                ContextMenuStrip = notifyIconContextMenu,
            };

            notifyIcon.DoubleClick += ShowMainForm;
        }

        public void ShowNotification(
            string title,
            string message,
            ToolTipIcon icon = ToolTipIcon.Info
        )
        {
            if (notifyIcon != null)
            {
                notifyIcon.BalloonTipTitle = title;
                notifyIcon.BalloonTipText = message;
                notifyIcon.BalloonTipIcon = icon;
                notifyIcon.ShowBalloonTip(3000); // Display for 3 seconds
            }
        }

        public void UpdateNotifyIcon(AutoArmState status)
        {
            notifyIcon.Icon = new Icon("path_to_new_icon.ico");
        }

        private void ShowMainForm(object? sender, EventArgs e)
        {
            // Restore the main form
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void ExitApplication(object? sender, EventArgs e)
        {
            // Fully exit the application
            notifyIcon.Visible = false; // Hide the NotifyIcon
            notifyIcon.Dispose();
            Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Minimize to tray instead of closing the application
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
            else
            {
                base.OnFormClosing(e);
            }
        }

        #endregion NotifyIcon

        private void InitializeButtonWatcher()
        {
            // If we have saved settings, try initializing the button watcher
            if (buttonWatchRegion != Rectangle.Empty)
            {
                try
                {
                    lynxButtonWatcher = new LynxButtonWatcher(buttonWatchRegion);
                    var (analysis, bitmap) = lynxButtonWatcher.AnalyzeScreen();
                    pictureBox1.Image = bitmap;
                }
                catch { }
            }

            // If we don't have saved settings, prompt the user to select a region
            if (lynxButtonWatcher == null)
            {
                selectButtonWatchRegion();
            }
        }

        private void EnableButton_Click(object sender, EventArgs e)
        {
            // Connect to Lynx
            lynxInterface = new LynxInterface(this);
            lynxInterface.AutoArmStateChanged += LynxInterface_AutoArmStateChanged;
            lynxInterface.AutoArmWarning += LynxInterface_AutoArmWarning;

            // Update buttons
            UpdateUiControls(true);
        }

        private void DisableButton_Click(object sender, EventArgs e)
        {
            // Disconnect from Lynx
            if (lynxInterface != null)
            {
                lynxInterface.AutoArmStateChanged -= LynxInterface_AutoArmStateChanged;
                lynxInterface.AutoArmWarning -= LynxInterface_AutoArmWarning;
                lynxInterface.Dispose();
                lynxInterface = null;
            }

            // Update buttons
            UpdateArmStatus("Disabled");
            notifyIcon.Icon = new Icon("Resources/ArmDisabled.ico");
            UpdateUiControls(false);
        }

        private void LoadSettings()
        {
            udpPortTextBox.Text = Properties.Settings.Default.UdpPort.ToString();
            delayTextBox.Text = Properties.Settings.Default.Delay.ToString();
            buttonWatchRegion = new Rectangle(
                Properties.Settings.Default.ButtonWatchRegionX,
                Properties.Settings.Default.ButtonWatchRegionY,
                Properties.Settings.Default.ButtonWatchRegionWidth,
                Properties.Settings.Default.ButtonWatchRegionHeight
            );
        }

        private void LynxInterface_AutoArmStateChanged(object? sender, AutoArmState e)
        {
            try
            {
                switch (e)
                {
                    case AutoArmState.Disabled:
                        UpdateArmStatus("Disabled");
                        notifyIcon.Icon = new Icon("Resources/ArmDisabled.ico");
                        break;
                    case AutoArmState.WaitingForStart:
                        UpdateArmStatus("Waiting for start...");
                        notifyIcon.Icon = new Icon("Resources/ArmWait.ico");
                        break;
                    case AutoArmState.WaitingForDelay:
                        UpdateArmStatus("Waiting for delay...");
                        notifyIcon.Icon = new Icon("Resources/ArmWait.ico");
                        break;
                    case AutoArmState.Verified:
                        UpdateArmStatus("Verified");
                        notifyIcon.Icon = new Icon("Resources/ArmDone.ico");
                        break;
                    case AutoArmState.NotVerified:
                        UpdateArmStatus("Not verified");
                        notifyIcon.Icon = new Icon("Resources/ArmError.ico");
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Error in LynxInterface_AutoArmStateChanged: {ex.Message}"
                );
            }
        }

        private void LynxInterface_AutoArmWarning(object? sender, AutoArmErrorState e)
        {
            try
            {
                switch (e)
                {
                    case AutoArmErrorState.AlreadyEnabled:
                        ShowNotification("AutoArm", "Capture already enabled.", ToolTipIcon.Info);
                        break;
                    case AutoArmErrorState.ButtonNotVisible:
                        ShowNotification(
                            "AutoArm",
                            "Button not visible in selected region.",
                            ToolTipIcon.Warning
                        );
                        var (analysis, bitmap) = lynxButtonWatcher!.AnalyzeScreen();
                        pictureBox1.Image = bitmap;
                        break;
                    case AutoArmErrorState.CaptureNotEnabled:
                        ShowNotification("AutoArm", "Capture not enabled!", ToolTipIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Error in LynxInterface_AutoArmWarning: {ex.Message}"
                );
            }
        }

        private void UpdateUiControls(bool enabled)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateUiControls(enabled)));
                return;
            }
            enableButton.Enabled = !enabled;
            enableButton.BackColor = enabled ? Color.Gray : Color.Lime;
            disableButton.Enabled = enabled;
            disableButton.BackColor = enabled ? Color.Red : Color.Gray;
            udpPortTextBox.Enabled = !enabled;
            statusValue.Text = enabled ? "Enabled" : "Disabled";
        }

        public void UpdateLynxStatus(string status)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateLynxStatus(status)));
                return;
            }
            lynxStatusValue.Text = status;
        }

        public void UpdateArmStatus(string status)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateArmStatus(status)));
                return;
            }
            statusValue.Text = status;
        }

        private void delayTextBox_TextChanged(object sender, EventArgs e)
        {
            int delay;
            if (!int.TryParse(delayTextBox.Text, out delay))
            {
                delayTextBox.Text = Properties.Settings.Default.Delay.ToString();
            }
            else
            {
                Properties.Settings.Default.Delay = delay;
                Properties.Settings.Default.Save();
            }
        }

        private void udpPortTextBox_TextChanged(object sender, EventArgs e)
        {
            int udpPort;
            if (!int.TryParse(udpPortTextBox.Text, out udpPort))
            {
                udpPortTextBox.Text = Properties.Settings.Default.UdpPort.ToString();
            }
            else
            {
                Properties.Settings.Default.UdpPort = udpPort;
                Properties.Settings.Default.Save();
            }
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            selectButtonWatchRegion();
        }

        private void selectButtonWatchRegion()
        {
            Rectangle region = Rectangle.Empty;
            using (ScreenSelector selector = new ScreenSelector())
            {
                if (selector.ShowDialog() == DialogResult.OK)
                {
                    region = selector.SelectedRegion;
                }
            }
            if (region != Rectangle.Empty)
            {
                buttonWatchRegion = region;
                Properties.Settings.Default.ButtonWatchRegionX = region.X;
                Properties.Settings.Default.ButtonWatchRegionY = region.Y;
                Properties.Settings.Default.ButtonWatchRegionWidth = region.Width;
                Properties.Settings.Default.ButtonWatchRegionHeight = region.Height;
                Properties.Settings.Default.Save();

                try
                {
                    lynxButtonWatcher = new LynxButtonWatcher(buttonWatchRegion);
                    var (analysis, bitmap) = lynxButtonWatcher.AnalyzeScreen();
                    pictureBox1.Image = bitmap;
                }
                catch
                {
                    lynxButtonWatcher = null;
                    MessageBox.Show("Could not find button in selected area. Please try again.");
                }
            }
            else
            {
                MessageBox.Show("No region selected.");
            }
        }
    }
}
