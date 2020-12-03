namespace View
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.GroupBox = new System.Windows.Forms.GroupBox();
            this.helpButton = new System.Windows.Forms.Button();
            this.nameLabel = new System.Windows.Forms.Label();
            this.serverLabel = new System.Windows.Forms.Label();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.serverTextBox = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.controlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GroupBox.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox
            // 
            this.GroupBox.BackColor = System.Drawing.Color.White;
            this.GroupBox.Controls.Add(this.helpButton);
            this.GroupBox.Controls.Add(this.nameLabel);
            this.GroupBox.Controls.Add(this.serverLabel);
            this.GroupBox.Controls.Add(this.ConnectButton);
            this.GroupBox.Controls.Add(this.nameTextBox);
            this.GroupBox.Controls.Add(this.serverTextBox);
            this.GroupBox.Location = new System.Drawing.Point(0, 0);
            this.GroupBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.GroupBox.Name = "GroupBox";
            this.GroupBox.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.GroupBox.Size = new System.Drawing.Size(800, 38);
            this.GroupBox.TabIndex = 0;
            this.GroupBox.TabStop = false;
            // 
            // helpButton
            // 
            this.helpButton.Location = new System.Drawing.Point(742, 9);
            this.helpButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(54, 21);
            this.helpButton.TabIndex = 5;
            this.helpButton.TabStop = false;
            this.helpButton.Text = "Help";
            this.helpButton.UseVisualStyleBackColor = true;
            this.helpButton.Click += new System.EventHandler(this.HelpButton_Click);
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(162, 12);
            this.nameLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(44, 15);
            this.nameLabel.TabIndex = 4;
            this.nameLabel.Text = "Name:";
            // 
            // serverLabel
            // 
            this.serverLabel.AutoSize = true;
            this.serverLabel.Location = new System.Drawing.Point(4, 12);
            this.serverLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.serverLabel.Name = "serverLabel";
            this.serverLabel.Size = new System.Drawing.Size(45, 15);
            this.serverLabel.TabIndex = 3;
            this.serverLabel.Text = "Server:";
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(328, 7);
            this.ConnectButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(88, 24);
            this.ConnectButton.TabIndex = 2;
            this.ConnectButton.TabStop = false;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(202, 10);
            this.nameTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.nameTextBox.MaxLength = 16;
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(111, 20);
            this.nameTextBox.TabIndex = 1;
            this.nameTextBox.Text = "player";
            // 
            // serverTextBox
            // 
            this.serverTextBox.Location = new System.Drawing.Point(46, 10);
            this.serverTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.serverTextBox.Name = "serverTextBox";
            this.serverTextBox.Size = new System.Drawing.Size(102, 20);
            this.serverTextBox.TabIndex = 0;
            this.serverTextBox.Text = "localhost";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.controlsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(211, 80);
            // 
            // controlsToolStripMenuItem
            // 
            this.controlsToolStripMenuItem.Name = "controlsToolStripMenuItem";
            this.controlsToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.controlsToolStripMenuItem.Text = "Controls";
            this.controlsToolStripMenuItem.Click += new System.EventHandler(this.ControlsToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(800, 652);
            this.Controls.Add(this.GroupBox);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MinimumSize = new System.Drawing.Size(604, 699);
            this.Name = "Form1";
            this.Text = "Tank Wars!!!";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.GroupBox.ResumeLayout(false);
            this.GroupBox.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox GroupBox;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label serverLabel;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.TextBox serverTextBox;
        private System.Windows.Forms.Button helpButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem controlsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}

