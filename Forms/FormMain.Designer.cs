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
			this.LabelLinks = new System.Windows.Forms.LinkLabel();
			this.ButtonQuit = new System.Windows.Forms.Button();
			this.ButtonReload = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.TreeStates = new System.Windows.Forms.TreeView();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
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
			// pictureBox1
			// 
			this.pictureBox1.Image = global::LothianProductions.VoIP.Properties.Resources.HVoIPM_64x;
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
			// LabelLinks
			// 
			this.LabelLinks.AutoSize = true;
			this.LabelLinks.LinkArea = new System.Windows.Forms.LinkArea( 0, 0 );
			this.LabelLinks.Location = new System.Drawing.Point( 67, 36 );
			this.LabelLinks.Name = "LabelLinks";
			this.LabelLinks.Size = new System.Drawing.Size( 326, 16 );
			this.LabelLinks.TabIndex = 5;
			this.LabelLinks.Text = "© Lothian Productions 2005, all rights reserved. About HVoIPM.";
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
			this.label3.Size = new System.Drawing.Size( 132, 16 );
			this.label3.TabIndex = 11;
			this.label3.Text = "Full device monitor data:";
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point( 312, 140 );
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size( 89, 20 );
			this.checkBox1.TabIndex = 12;
			this.checkBox1.Text = "Log changes";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// checkBox2
			// 
			this.checkBox2.AutoSize = true;
			this.checkBox2.Location = new System.Drawing.Point( 312, 166 );
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size( 81, 20 );
			this.checkBox2.TabIndex = 13;
			this.checkBox2.Text = "checkBox2";
			this.checkBox2.UseVisualStyleBackColor = true;
			// 
			// TreeStates
			// 
			this.TreeStates.Location = new System.Drawing.Point( 12, 139 );
			this.TreeStates.Name = "TreeStates";
			this.TreeStates.Size = new System.Drawing.Size( 294, 321 );
			this.TreeStates.TabIndex = 14;
			this.TreeStates.AfterSelect += new System.Windows.Forms.TreeViewEventHandler( this.TreeStates_AfterSelect );
			// 
			// listBox1
			// 
			this.listBox1.FormattingEnabled = true;
			this.listBox1.ItemHeight = 16;
			this.listBox1.Location = new System.Drawing.Point( 315, 250 );
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size( 147, 84 );
			this.listBox1.TabIndex = 15;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point( 318, 217 );
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size( 143, 20 );
			this.textBox1.TabIndex = 16;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 16F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 475, 70 );
			this.Controls.Add( this.textBox1 );
			this.Controls.Add( this.listBox1 );
			this.Controls.Add( this.TreeStates );
			this.Controls.Add( this.checkBox2 );
			this.Controls.Add( this.checkBox1 );
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
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.TreeView TreeStates;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.TextBox textBox1;
    }
}

