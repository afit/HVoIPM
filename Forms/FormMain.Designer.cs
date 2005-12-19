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
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.linkLabel2 = new System.Windows.Forms.LinkLabel();
			this.ListboxLines = new System.Windows.Forms.CheckedListBox();
			this.ButtonDump = new System.Windows.Forms.Button();
			this.ButtonQuit = new System.Windows.Forms.Button();
			this.ButtonHide = new System.Windows.Forms.Button();
			this.ButtonReload = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.NotifyIconContextMenuStrip.SuspendLayout();
			( (System.ComponentModel.ISupportInitialize) ( this.pictureBox1 ) ).BeginInit();
			this.SuspendLayout();
			// 
			// NotifyIcon
			// 
			this.NotifyIcon.ContextMenuStrip = this.NotifyIconContextMenuStrip;
			this.NotifyIcon.Icon = ( (System.Drawing.Icon) ( resources.GetObject( "NotifyIcon.Icon" ) ) );
			this.NotifyIcon.Text = "Hardware VoIP Monitor";
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
			// pictureBox1
			// 
			this.pictureBox1.Image = global::LothianProductions.VoIP.Monitor.Properties.Resources.HVoIPM_64x;
			this.pictureBox1.Location = new System.Drawing.Point( 12, 12 );
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size( 48, 48 );
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font( "Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte) ( 0 ) ) );
			this.label1.Location = new System.Drawing.Point( 66, 12 );
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size( 237, 24 );
			this.label1.TabIndex = 2;
			this.label1.Text = "Hardware VoIP Monitor";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point( 67, 36 );
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size( 326, 16 );
			this.label2.TabIndex = 3;
			this.label2.Text = "© Lothian Productions 2005, all rights reserved. About HVoIPM.";
			// 
			// linkLabel1
			// 
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Location = new System.Drawing.Point( 311, 51 );
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size( 78, 16 );
			this.linkLabel1.TabIndex = 4;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "About HVoIPM";
			// 
			// linkLabel2
			// 
			this.linkLabel2.AutoSize = true;
			this.linkLabel2.Location = new System.Drawing.Point( 78, 51 );
			this.linkLabel2.Name = "linkLabel2";
			this.linkLabel2.Size = new System.Drawing.Size( 109, 16 );
			this.linkLabel2.TabIndex = 5;
			this.linkLabel2.TabStop = true;
			this.linkLabel2.Text = "Lothian Productions";
			// 
			// ListboxLines
			// 
			this.ListboxLines.FormattingEnabled = true;
			this.ListboxLines.Location = new System.Drawing.Point( 12, 139 );
			this.ListboxLines.Name = "ListboxLines";
			this.ListboxLines.Size = new System.Drawing.Size( 403, 184 );
			this.ListboxLines.TabIndex = 6;
			// 
			// ButtonDump
			// 
			this.ButtonDump.Location = new System.Drawing.Point( 15, 79 );
			this.ButtonDump.Name = "ButtonDump";
			this.ButtonDump.Size = new System.Drawing.Size( 83, 27 );
			this.ButtonDump.TabIndex = 7;
			this.ButtonDump.Text = "&Dump status";
			this.ButtonDump.UseVisualStyleBackColor = true;
			this.ButtonDump.Click += new System.EventHandler( this.ButtonDump_Click );
			// 
			// ButtonQuit
			// 
			this.ButtonQuit.Location = new System.Drawing.Point( 283, 79 );
			this.ButtonQuit.Name = "ButtonQuit";
			this.ButtonQuit.Size = new System.Drawing.Size( 82, 27 );
			this.ButtonQuit.TabIndex = 8;
			this.ButtonQuit.Text = "&Quit";
			this.ButtonQuit.UseVisualStyleBackColor = true;
			this.ButtonQuit.Click += new System.EventHandler( this.ButtonQuit_Click );
			// 
			// ButtonHide
			// 
			this.ButtonHide.Location = new System.Drawing.Point( 188, 79 );
			this.ButtonHide.Name = "ButtonHide";
			this.ButtonHide.Size = new System.Drawing.Size( 89, 27 );
			this.ButtonHide.TabIndex = 9;
			this.ButtonHide.Text = "&Hide";
			this.ButtonHide.UseVisualStyleBackColor = true;
			this.ButtonHide.Click += new System.EventHandler( this.ButtonHide_Click );
			// 
			// ButtonReload
			// 
			this.ButtonReload.Location = new System.Drawing.Point( 104, 79 );
			this.ButtonReload.Name = "ButtonReload";
			this.ButtonReload.Size = new System.Drawing.Size( 78, 27 );
			this.ButtonReload.TabIndex = 10;
			this.ButtonReload.Text = "&Reload";
			this.ButtonReload.UseVisualStyleBackColor = true;
			this.ButtonReload.Click += new System.EventHandler( this.ButtonReload_Click );
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point( 9, 120 );
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size( 221, 16 );
			this.label3.TabIndex = 11;
			this.label3.Text = "Monitor the following lines for fatal errors:";
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 16F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 427, 337 );
			this.Controls.Add( this.label3 );
			this.Controls.Add( this.ButtonReload );
			this.Controls.Add( this.ButtonHide );
			this.Controls.Add( this.ButtonQuit );
			this.Controls.Add( this.ButtonDump );
			this.Controls.Add( this.ListboxLines );
			this.Controls.Add( this.linkLabel2 );
			this.Controls.Add( this.linkLabel1 );
			this.Controls.Add( this.label2 );
			this.Controls.Add( this.label1 );
			this.Controls.Add( this.pictureBox1 );
			this.Font = new System.Drawing.Font( "Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte) ( 0 ) ) );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Margin = new System.Windows.Forms.Padding( 3, 4, 3, 4 );
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormMain";
			this.Text = "Hardware VoIP Monitor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.FormMain_FormClosing );
			this.NotifyIconContextMenuStrip.ResumeLayout( false );
			( (System.ComponentModel.ISupportInitialize) ( this.pictureBox1 ) ).EndInit();
			this.ResumeLayout( false );
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.NotifyIcon NotifyIcon;
		private System.Windows.Forms.ContextMenuStrip NotifyIconContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem toolStripQuit;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.LinkLabel linkLabel2;
		private System.Windows.Forms.CheckedListBox ListboxLines;
		private System.Windows.Forms.Button ButtonDump;
		private System.Windows.Forms.Button ButtonQuit;
		private System.Windows.Forms.Button ButtonHide;
		private System.Windows.Forms.Button ButtonReload;
		private System.Windows.Forms.Label label3;
    }
}

