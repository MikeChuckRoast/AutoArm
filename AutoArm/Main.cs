using System.Drawing;

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

        private Rectangle buttonWatchRegion;

        private LynxInterface? lynxInterface;

        private LynxButtonWatcher? lynxButtonWatcher;

        public Main()
        {
            InitializeComponent();
            LoadSettings();
            UpdateUiControls(false);
            InitializeButtonWatcher();
        }

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

            // Update buttons
            UpdateUiControls(true);
        }

        private void DisableButton_Click(object sender, EventArgs e)
        {
            // Disconnect from Lynx
            if (lynxInterface != null)
            {
                lynxInterface.Dispose();
                lynxInterface = null;
            }

            // Update buttons
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
