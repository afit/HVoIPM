namespace LothianProductions.VoIP.Forms
{
	partial class CallRecordViewer
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CallRecordViewer));
			this.gridCalls = new System.Windows.Forms.DataGridView();
			((System.ComponentModel.ISupportInitialize)(this.gridCalls)).BeginInit();
			this.SuspendLayout();
			// 
			// gridCalls
			// 
			this.gridCalls.AllowUserToAddRows = false;
			this.gridCalls.AllowUserToDeleteRows = false;
			this.gridCalls.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.gridCalls.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gridCalls.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridCalls.Location = new System.Drawing.Point(0, 0);
			this.gridCalls.Name = "gridCalls";
			this.gridCalls.ReadOnly = true;
			this.gridCalls.RowHeadersVisible = false;
			this.gridCalls.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.gridCalls.Size = new System.Drawing.Size(512, 418);
			this.gridCalls.TabIndex = 0;
			// 
			// CallRecordViewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(512, 418);
			this.Controls.Add(this.gridCalls);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "CallRecordViewer";
			this.Text = "Call Record Viewer";
			this.Load += new System.EventHandler(this.CallRecordViewer_Load);
			((System.ComponentModel.ISupportInitialize)(this.gridCalls)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView gridCalls;
	}
}