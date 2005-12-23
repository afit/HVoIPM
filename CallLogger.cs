using System;
using System.IO;
using System.Web;
using System.Xml;
using System.Diagnostics;

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
            Log(call.Device, call.CallState.ToString(), call.Direction, call.Phone, call.StartTime, call.EndTime, call.Duration);
        }

        public void Log(string device, string callid, string direction, string phone, DateTime starttime, DateTime endtime, string length ) {
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
                strLine += "," + device + "," + callid + "," + direction + "," + phone + "," + starttime + "," + endtime + "," + length;
                objLogFile.WriteLine(strLine);
                objLogFile.Close();
            }
        }
    }
}
