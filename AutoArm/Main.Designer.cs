namespace AutoArm
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            enableButton = new Button();
            disableButton = new Button();
            statusLabel = new Label();
            statusValue = new Label();
            lynxStatusLabel = new Label();
            label1 = new Label();
            delayTextBox = new TextBox();
            lynxStatusValue = new Label();
            label2 = new Label();
            udpPortTextBox = new TextBox();
            SuspendLayout();
            // 
            // enableButton
            // 
            enableButton.BackColor = Color.Lime;
            enableButton.FlatStyle = FlatStyle.Popup;
            enableButton.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            enableButton.Location = new Point(44, 34);
            enableButton.Name = "enableButton";
            enableButton.Size = new Size(100, 32);
            enableButton.TabIndex = 0;
            enableButton.Text = "Enable";
            enableButton.UseVisualStyleBackColor = false;
            enableButton.Click += EnableButton_Click;
            // 
            // disableButton
            // 
            disableButton.BackColor = Color.Red;
            disableButton.Enabled = false;
            disableButton.FlatStyle = FlatStyle.Popup;
            disableButton.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            disableButton.ForeColor = Color.White;
            disableButton.Location = new Point(175, 34);
            disableButton.Name = "disableButton";
            disableButton.Size = new Size(100, 32);
            disableButton.TabIndex = 1;
            disableButton.Text = "Disable";
            disableButton.UseVisualStyleBackColor = false;
            disableButton.Click += DisableButton_Click;
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            statusLabel.Location = new Point(35, 82);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(120, 20);
            statusLabel.TabIndex = 2;
            statusLabel.Text = "Auto Arm Status:";
            // 
            // statusValue
            // 
            statusValue.AutoSize = true;
            statusValue.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            statusValue.Location = new Point(166, 82);
            statusValue.Name = "statusValue";
            statusValue.Size = new Size(68, 20);
            statusValue.TabIndex = 3;
            statusValue.Text = "Disabled";
            // 
            // lynxStatusLabel
            // 
            lynxStatusLabel.AutoSize = true;
            lynxStatusLabel.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lynxStatusLabel.Location = new Point(35, 158);
            lynxStatusLabel.Name = "lynxStatusLabel";
            lynxStatusLabel.Size = new Size(82, 20);
            lynxStatusLabel.TabIndex = 4;
            lynxStatusLabel.Text = "Lynx Timer:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(35, 120);
            label1.Name = "label1";
            label1.Size = new Size(70, 20);
            label1.TabIndex = 5;
            label1.Text = "Delay (s):";
            // 
            // delayTextBox
            // 
            delayTextBox.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            delayTextBox.Location = new Point(166, 117);
            delayTextBox.Name = "delayTextBox";
            delayTextBox.Size = new Size(39, 27);
            delayTextBox.TabIndex = 6;
            delayTextBox.Text = "3";
            delayTextBox.TextAlign = HorizontalAlignment.Center;
            delayTextBox.TextChanged += delayTextBox_TextChanged;
            // 
            // lynxStatusValue
            // 
            lynxStatusValue.AutoSize = true;
            lynxStatusValue.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lynxStatusValue.Location = new Point(166, 158);
            lynxStatusValue.Name = "lynxStatusValue";
            lynxStatusValue.Size = new Size(70, 20);
            lynxStatusValue.TabIndex = 7;
            lynxStatusValue.Text = "Unknown";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(35, 197);
            label2.Name = "label2";
            label2.Size = new Size(71, 20);
            label2.TabIndex = 8;
            label2.Text = "UDP Port:";
            // 
            // udpPortTextBox
            // 
            udpPortTextBox.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            udpPortTextBox.Location = new Point(166, 194);
            udpPortTextBox.Name = "udpPortTextBox";
            udpPortTextBox.Size = new Size(68, 27);
            udpPortTextBox.TabIndex = 9;
            udpPortTextBox.Text = "8113";
            udpPortTextBox.TextAlign = HorizontalAlignment.Center;
            udpPortTextBox.TextChanged += udpPortTextBox_TextChanged;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(314, 241);
            Controls.Add(udpPortTextBox);
            Controls.Add(label2);
            Controls.Add(lynxStatusValue);
            Controls.Add(delayTextBox);
            Controls.Add(label1);
            Controls.Add(lynxStatusLabel);
            Controls.Add(statusValue);
            Controls.Add(statusLabel);
            Controls.Add(disableButton);
            Controls.Add(enableButton);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Main";
            Text = "Lynx Auto Arm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button enableButton;
        private Button disableButton;
        private Label statusLabel;
        private Label statusValue;
        private Label lynxStatusLabel;
        private Label label1;
        private TextBox delayTextBox;
        private Label lynxStatusValue;
        private Label label2;
        private TextBox udpPortTextBox;
    }
}
