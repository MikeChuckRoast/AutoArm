namespace AutoArm
{
    public partial class Main : Form
    {

        public int udpPort
        {
            get
            {
                return int.Parse(udpPortTextBox.Text);
            }
        }

        public int delay
        {
            get
            {
                return int.Parse(delayTextBox.Text);
            }
        }

        private LynxInterface? lynxInterface;

        public Main()
        {
            InitializeComponent();
            UpdateUiControls(false);
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
    }
}
