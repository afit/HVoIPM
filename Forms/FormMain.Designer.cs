namespace LothianProductions.VoIP.Forms {
    partial class FormMain {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing ) {
            if( disposing && ( components != null ) ) {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( FormMain ) );
			this.NotifyIcon = new System.Windows.Forms.NotifyIcon( this.components );
			this.NotifyIconContextMenuStrip = new System.Windows.Forms.ContextMenuStrip( this.components );
			this.toolStripQuit = new System.Windows.Forms.ToolStripMenuItem();
			this.TextBoxStatus = new System.Windows.Forms.TextBox();
			this.NotifyIconContextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// NotifyIcon
			// 
			this.NotifyIcon.ContextMenuStrip = this.NotifyIconContextMenuStrip;
			this.NotifyIcon.Icon = ( (System.Drawing.Icon) ( resources.GetObject( "NotifyIcon.Icon" ) ) );
			this.NotifyIcon.Text = "notifyIcon1";
			this.NotifyIcon.Visible = true;
			this.NotifyIcon.Click += new System.EventHandler( this.NotifyIcon_Click );
			// 
			// NotifyIconContextMenuStrip
			// 
			this.NotifyIconContextMenuStrip.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.toolStripQuit} );
			this.NotifyIconContextMenuStrip.Name = "NotifyIconContextMenuStrip";
			this.NotifyIconContextMenuStrip.Size = new System.Drawing.Size( 95, 26 );
			// 
			// toolStripQuit
			// 
			this.toolStripQuit.Name = "toolStripQuit";
			this.toolStripQuit.Size = new System.Drawing.Size( 94, 22 );
			this.toolStripQuit.Text = "&Quit";
			this.toolStripQuit.Click += new System.EventHandler( this.toolStripQuit_Click );
			// 
			// TextBoxStatus
			// 
			this.TextBoxStatus.Location = new System.Drawing.Point( 16, 23 );
			this.TextBoxStatus.Multiline = true;
			this.TextBoxStatus.Name = "TextBoxStatus";
			this.TextBoxStatus.Size = new System.Drawing.Size( 261, 284 );
			this.TextBoxStatus.TabIndex = 0;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 16F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 292, 327 );
			this.Controls.Add( this.TextBoxStatus );
			this.Font = new System.Drawing.Font( "Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte) ( 0 ) ) );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Margin = new System.Windows.Forms.Padding( 3, 4, 3, 4 );
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormMain";
			this.Text = "Hardware VoIP Monitor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.FormMain_FormClosing );
			this.NotifyIconContextMenuStrip.ResumeLayout( false );
			this.ResumeLayout( false );
			this.PerformLayout();			
        }

        #endregion

		private System.Windows.Forms.NotifyIcon NotifyIcon;
        private System.Windows.Forms.TextBox TextBoxStatus;
		private System.Windows.Forms.ContextMenuStrip NotifyIconContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem toolStripQuit;
    }
}

