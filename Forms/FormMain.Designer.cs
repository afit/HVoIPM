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
			this.toolStripShowMain = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripCallRecords = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripCheckForUpdates = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripLoadAtStartup = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripQuit = new System.Windows.Forms.ToolStripMenuItem();
			this.LabelTitle = new System.Windows.Forms.Label();
			this.LabelLinks = new System.Windows.Forms.LinkLabel();
			this.label3 = new System.Windows.Forms.Label();
			this.TimerFlash = new System.Windows.Forms.Timer( this.components );
			this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
			this.TreeStates = new System.Windows.Forms.TreeView();
			this.LabelUpdates = new System.Windows.Forms.LinkLabel();
			this.NotifyIconContextMenuStrip.SuspendLayout();
			( (System.ComponentModel.ISupportInitialize) ( this.pictureBoxIcon ) ).BeginInit();
			this.SuspendLayout();
			// 
			// NotifyIcon
			// 
			this.NotifyIcon.ContextMenuStrip = this.NotifyIconContextMenuStrip;
			this.NotifyIcon.Icon = global::LothianProductions.VoIP.Properties.Resources.HVoIPM_48x;
			this.NotifyIcon.Text = "Hardware VoIP Monitor";
			this.NotifyIcon.Visible = true;
			this.NotifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler( this.NotifyIcon_MouseClick );
			// 
			// NotifyIconContextMenuStrip
			// 
			this.NotifyIconContextMenuStrip.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.toolStripShowMain,
            this.toolStripCallRecords,
            this.toolStripSeparator1,
            this.toolStripCheckForUpdates,
            this.toolStripLoadAtStartup,
            this.toolStripSeparator2,
            this.toolStripQuit} );
			this.NotifyIconContextMenuStrip.Name = "NotifyIconContextMenuStrip";
			this.NotifyIconContextMenuStrip.Size = new System.Drawing.Size( 181, 148 );
			// 
			// toolStripShowMain
			// 
			this.toolStripShowMain.Font = new System.Drawing.Font( "Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte) ( 0 ) ) );
			this.toolStripShowMain.Name = "toolStripShowMain";
			this.toolStripShowMain.Size = new System.Drawing.Size( 180, 22 );
			this.toolStripShowMain.Text = "&Show main window";
			this.toolStripShowMain.Click += new System.EventHandler( this.toolStripShowMain_Click );
			// 
			// toolStripCallRecords
			// 
			this.toolStripCallRecords.Name = "toolStripCallRecords";
			this.toolStripCallRecords.Size = new System.Drawing.Size( 180, 22 );
			this.toolStripCallRecords.Text = "&Call records";
			this.toolStripCallRecords.Click += new System.EventHandler( this.toolStripCallLog_Click );
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size( 177, 6 );
			// 
			// toolStripCheckForUpdates
			// 
			this.toolStripCheckForUpdates.Name = "toolStripCheckForUpdates";
			this.toolStripCheckForUpdates.Size = new System.Drawing.Size( 180, 22 );
			this.toolStripCheckForUpdates.Text = "C&heck for updates";
			this.toolStripCheckForUpdates.Click += new System.EventHandler( this.toolStripCheckForUpdates_Click );
			// 
			// toolStripLoadAtStartup
			// 
			this.toolStripLoadAtStartup.Name = "toolStripLoadAtStartup";
			this.toolStripLoadAtStartup.Size = new System.Drawing.Size( 180, 22 );
			this.toolStripLoadAtStartup.Text = "&Load at startup";
			this.toolStripLoadAtStartup.Click += new System.EventHandler( this.toolStripRunOnStartup_Click );
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size( 177, 6 );
			// 
			// toolStripQuit
			// 
			this.toolStripQuit.Name = "toolStripQuit";
			this.toolStripQuit.Size = new System.Drawing.Size( 180, 22 );
			this.toolStripQuit.Text = "&Quit";
			this.toolStripQuit.Click += new System.EventHandler( this.toolStripQuit_Click );
			// 
			// LabelTitle
			// 
			this.LabelTitle.AutoSize = true;
			this.LabelTitle.Font = new System.Drawing.Font( "Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte) ( 0 ) ) );
			this.LabelTitle.Location = new System.Drawing.Point( 66, 9 );
			this.LabelTitle.Name = "LabelTitle";
			this.LabelTitle.Size = new System.Drawing.Size( 237, 24 );
			this.LabelTitle.TabIndex = 2;
			this.LabelTitle.Text = "Hardware VoIP Monitor";
			// 
			// LabelLinks
			// 
			this.LabelLinks.AutoSize = true;
			this.LabelLinks.LinkArea = new System.Windows.Forms.LinkArea( 0, 0 );
			this.LabelLinks.Location = new System.Drawing.Point( 67, 33 );
			this.LabelLinks.Name = "LabelLinks";
			this.LabelLinks.Size = new System.Drawing.Size( 326, 16 );
			this.LabelLinks.TabIndex = 5;
			this.LabelLinks.Text = "� Lothian Productions 2005, all rights reserved. About HVoIPM.";
			this.LabelLinks.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler( this.LabelLinks_LinkClicked );
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point( 9, 73 );
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size( 132, 16 );
			this.label3.TabIndex = 11;
			this.label3.Text = "Full device monitor data:";
			// 
			// TimerFlash
			// 
			this.TimerFlash.Interval = 1000;
			this.TimerFlash.Tick += new System.EventHandler( this.TimerFlash_Tick );
			// 
			// pictureBoxIcon
			// 
			this.pictureBoxIcon.Image = global::LothianProductions.VoIP.Properties.Resources.HVoIPM_48x1;
			this.pictureBoxIcon.Location = new System.Drawing.Point( 12, 12 );
			this.pictureBoxIcon.Name = "pictureBoxIcon";
			this.pictureBoxIcon.Size = new System.Drawing.Size( 48, 48 );
			this.pictureBoxIcon.TabIndex = 1;
			this.pictureBoxIcon.TabStop = false;
			// 
			// TreeStates
			// 
			this.TreeStates.Location = new System.Drawing.Point( 12, 92 );
			this.TreeStates.Name = "TreeStates";
			this.TreeStates.Size = new System.Drawing.Size( 467, 321 );
			this.TreeStates.TabIndex = 16;
			// 
			// LabelUpdates
			// 
			this.LabelUpdates.AutoSize = true;
			this.LabelUpdates.Location = new System.Drawing.Point( 67, 49 );
			this.LabelUpdates.Name = "LabelUpdates";
			this.LabelUpdates.Size = new System.Drawing.Size( 0, 16 );
			this.LabelUpdates.TabIndex = 17;
			this.LabelUpdates.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler( this.LabelUpdates_LinkClicked );
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 16F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 491, 425 );
			this.Controls.Add( this.LabelUpdates );
			this.Controls.Add( this.TreeStates );
			this.Controls.Add( this.label3 );
			this.Controls.Add( this.LabelTitle );
			this.Controls.Add( this.pictureBoxIcon );
			this.Controls.Add( this.LabelLinks );
			this.Font = new System.Drawing.Font( "Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte) ( 0 ) ) );
			this.Icon = ( (System.Drawing.Icon) ( resources.GetObject( "$this.Icon" ) ) );
			this.Margin = new System.Windows.Forms.Padding( 3, 4, 3, 4 );
			this.Name = "FormMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Hardware VoIP Monitor";
			this.Resize += new System.EventHandler( this.FormMain_Resize );
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.FormMain_FormClosing );
			this.NotifyIconContextMenuStrip.ResumeLayout( false );
			( (System.ComponentModel.ISupportInitialize) ( this.pictureBoxIcon ) ).EndInit();
			this.ResumeLayout( false );
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.NotifyIcon NotifyIcon;
		private System.Windows.Forms.ContextMenuStrip NotifyIconContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem toolStripQuit;
		private System.Windows.Forms.PictureBox pictureBoxIcon;
		private System.Windows.Forms.Label LabelTitle;
		private System.Windows.Forms.LinkLabel LabelLinks;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Timer TimerFlash;
		private System.Windows.Forms.ToolStripMenuItem toolStripCallRecords;
		private System.Windows.Forms.ToolStripMenuItem toolStripShowMain;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem toolStripCheckForUpdates;
		private System.Windows.Forms.TreeView TreeStates;
		private System.Windows.Forms.LinkLabel LabelUpdates;
		private System.Windows.Forms.ToolStripMenuItem toolStripLoadAtStartup;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}

