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
			this.label1 = new System.Windows.Forms.Label();
			this.LabelLinks = new System.Windows.Forms.LinkLabel();
			this.ButtonQuit = new System.Windows.Forms.Button();
			this.ButtonReload = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.TreeStates = new System.Windows.Forms.TreeView();
			this.TimerFlash = new System.Windows.Forms.Timer( this.components );
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.TextboxBehaviour = new System.Windows.Forms.TextBox();
			this.NotifyIconContextMenuStrip.SuspendLayout();
			( (System.ComponentModel.ISupportInitialize) ( this.pictureBox1 ) ).BeginInit();
			this.SuspendLayout();
			// 
			// NotifyIcon
			// 
			this.NotifyIcon.ContextMenuStrip = this.NotifyIconContextMenuStrip;
			this.NotifyIcon.Icon = global::LothianProductions.VoIP.Properties.Resources.HVoIPM;
			this.NotifyIcon.Text = "Hardware VoIP Monitor";
			this.NotifyIcon.Visible = true;
			this.NotifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler( this.NotifyIcon_MouseClick );
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
			// LabelLinks
			// 
			this.LabelLinks.AutoSize = true;
			this.LabelLinks.LinkArea = new System.Windows.Forms.LinkArea( 0, 0 );
			this.LabelLinks.Location = new System.Drawing.Point( 67, 36 );
			this.LabelLinks.Name = "LabelLinks";
			this.LabelLinks.Size = new System.Drawing.Size( 326, 16 );
			this.LabelLinks.TabIndex = 5;
			this.LabelLinks.Text = "� Lothian Productions 2005, all rights reserved. About HVoIPM.";
			// 
			// ButtonQuit
			// 
			this.ButtonQuit.Location = new System.Drawing.Point( 479, 9 );
			this.ButtonQuit.Name = "ButtonQuit";
			this.ButtonQuit.Size = new System.Drawing.Size( 82, 27 );
			this.ButtonQuit.TabIndex = 8;
			this.ButtonQuit.Text = "&Quit";
			this.ButtonQuit.UseVisualStyleBackColor = true;
			this.ButtonQuit.Click += new System.EventHandler( this.ButtonQuit_Click );
			// 
			// ButtonReload
			// 
			this.ButtonReload.Location = new System.Drawing.Point( 479, 42 );
			this.ButtonReload.Name = "ButtonReload";
			this.ButtonReload.Size = new System.Drawing.Size( 82, 27 );
			this.ButtonReload.TabIndex = 10;
			this.ButtonReload.Text = "&Reload";
			this.ButtonReload.UseVisualStyleBackColor = true;
			this.ButtonReload.Click += new System.EventHandler( this.ButtonReload_Click );
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point( 9, 63 );
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size( 132, 16 );
			this.label3.TabIndex = 11;
			this.label3.Text = "Full device monitor data:";
			// 
			// TreeStates
			// 
			this.TreeStates.Location = new System.Drawing.Point( 12, 82 );
			this.TreeStates.Name = "TreeStates";
			this.TreeStates.Size = new System.Drawing.Size( 294, 317 );
			this.TreeStates.TabIndex = 14;
			this.TreeStates.AfterSelect += new System.Windows.Forms.TreeViewEventHandler( this.TreeStates_AfterSelect );
			// 
			// TimerFlash
			// 
			this.TimerFlash.Interval = 1000;
			this.TimerFlash.Tick += new System.EventHandler( this.TimerFlash_Tick );
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ( (System.Drawing.Image) ( resources.GetObject( "pictureBox1.Image" ) ) );
			this.pictureBox1.Location = new System.Drawing.Point( 12, 12 );
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size( 48, 48 );
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			// 
			// TextboxBehaviour
			// 
			this.TextboxBehaviour.Location = new System.Drawing.Point( 312, 82 );
			this.TextboxBehaviour.Multiline = true;
			this.TextboxBehaviour.Name = "TextboxBehaviour";
			this.TextboxBehaviour.ReadOnly = true;
			this.TextboxBehaviour.Size = new System.Drawing.Size( 249, 317 );
			this.TextboxBehaviour.TabIndex = 15;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 16F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 573, 411 );
			this.Controls.Add( this.TextboxBehaviour );
			this.Controls.Add( this.TreeStates );
			this.Controls.Add( this.label3 );
			this.Controls.Add( this.ButtonReload );
			this.Controls.Add( this.ButtonQuit );
			this.Controls.Add( this.label1 );
			this.Controls.Add( this.pictureBox1 );
			this.Controls.Add( this.LabelLinks );
			this.Font = new System.Drawing.Font( "Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte) ( 0 ) ) );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Margin = new System.Windows.Forms.Padding( 3, 4, 3, 4 );
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
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
		private System.Windows.Forms.LinkLabel LabelLinks;
		private System.Windows.Forms.Button ButtonQuit;
		private System.Windows.Forms.Button ButtonReload;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TreeView TreeStates;
		private System.Windows.Forms.Timer TimerFlash;
		private System.Windows.Forms.TextBox TextboxBehaviour;
    }
}

