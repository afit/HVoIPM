using System;
using System.IO;
using System.Web;
using System.Xml;
using System.Diagnostics;

using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP
{
    /// <summary>
    /// Class to log error/applications messages
    /// </summary>
    public class CallLogger
    {
        public CallLogger() { }
        protected readonly static CallLogger mInstance = new CallLogger();
        public static CallLogger Instance() {
            return mInstance;
        }

        public void Log(CallRecord call) {
            Log(call.Device, call.Line, call.Call, call.StartTime, call.EndTime );
        }

        public void Log( Device device, Line line, Call call, DateTime starttime, DateTime endtime ) {
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
                StreamWriter objLogFile = File.AppendText(strFile);
                string strLine = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.ffffff");
				string phone = "unknown";
				if (call.Type == CallType.Inbound) {
					phone = line.LastCallerNumber;
				} else {
					phone = line.LastCalledNumber;
				}
				strLine += "," + device.Name + "," + call.Name + "," +  line.Name + "," + call.Type.ToString() +"," + phone + "," + starttime + "," + endtime + "," + call.Duration;
                objLogFile.WriteLine(strLine);
                objLogFile.Close();
            }
        }
    }
}
