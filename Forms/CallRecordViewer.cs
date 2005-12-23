using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace LothianProductions.VoIP.Forms
{
	public partial class CallRecordViewer : Form
	{
		public CallRecordViewer() {
			InitializeComponent();
		}

		private void CallRecordViewer_Load(object sender, EventArgs e) {
			lock (this) {
				string strFile = "";
				try {
					strFile = System.Configuration.ConfigurationManager.AppSettings["callLogFile"];
				} catch (System.Configuration.SettingsPropertyNotFoundException) {
					strFile = System.Environment.CurrentDirectory + "\\calls.log";
				}
				if (strFile == "") {
					strFile = System.Environment.CurrentDirectory + "\\calls.log";
				} else {
					strFile = System.Environment.CurrentDirectory + "\\" + strFile;
				}

				DataTable dt = new DataTable("calls");
				dt.Columns.Add( new DataColumn( "Date and Time", typeof(DateTime) ) );
				dt.Columns.Add( new DataColumn( "Direction", typeof(string) ) );
				dt.Columns.Add( new DataColumn( "Number", typeof(string) ) );
				dt.Columns.Add( new DataColumn( "Duration", typeof(string) ) );

				FileStream fs = File.OpenRead(strFile);
				StreamReader stream = new StreamReader(fs);
				string line = "";
				while(( line = stream.ReadLine()) != null ) {
					DataRow row = dt.NewRow();
					string[] contents = line.Split(',');
					row["Date and Time"] = contents[6];
					row["Direction"] = contents[4];
					row["Number"] = contents[5];
					row["Duration"] = contents[8];
					dt.Rows.Add(row);
				}
				gridCalls.DataSource = dt;
				stream.Close();
				fs.Close();
			}
		}
	}
}