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
    public class Logger
    {
        public Logger() { }
        public string EventLogName;
        protected readonly static Logger mInstance = new Logger();
        public enum LoggerLevel { Error, Warning, Information };
        public static Logger Instance() {
            return mInstance;
        }
        public void Log(string strLogMessage) {
            Log(strLogMessage, LoggerLevel.Error);
        }
        public void Log(string strLogMessage, LoggerLevel level) {
            lock (this) {
                string strType = System.Configuration.ConfigurationManager.AppSettings["logType"];
                switch (strType) {
                    case "eventlog":
                        EventLogEntryType et;
                        switch (level) {
                            case LoggerLevel.Error:
                                et = EventLogEntryType.Error;
                                break;
                            case LoggerLevel.Information:
                                et = EventLogEntryType.Information;
                                break;
                            case LoggerLevel.Warning:
                                et = EventLogEntryType.Warning;
                                break;
                            default:
                                et = EventLogEntryType.Information;
                                break;
                        }
                        StampEventLogEntry(strLogMessage, et);
                        break;
                    case "xml":
                        StampXmlLogEntry(strLogMessage, level);
                        break;
                    default:
                        string strFile = "";
                        try {
                            strFile = System.Configuration.ConfigurationManager.AppSettings["logFile"];
                        } catch (System.Configuration.SettingsPropertyNotFoundException) {
                            strFile = System.Environment.CurrentDirectory + "\\debug.log";
                        }
                        if (strFile == "") {
                            strFile = System.Environment.CurrentDirectory + "\\debug.log";
                        } else {
                            strFile = System.Environment.CurrentDirectory + "\\" + strFile;
                        }
                        StreamWriter objLogFile = File.AppendText(strFile);
                        objLogFile.WriteLine(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.ffffff") + "|" + level.ToString() + "|" + strLogMessage);
                        objLogFile.Close();
                        break;
                }
            }
        }
        public void Log(string strLogMessage, Exception ex) {
            Log(strLogMessage + ": " + ex.ToString());
        }
        public void Log(string strLogMessage, Exception ex, LoggerLevel level) {
            Log(strLogMessage + ": " + ex.ToString(), level);
        }

        private void StampEventLogEntry(string strMessage, EventLogEntryType evtType) {
            if (EventLogName == "") EventLogName = "LovettsUtilities";
            if (!EventLog.SourceExists(EventLogName)) {
                EventLog.CreateEventSource(EventLogName, EventLogName);
            }
            EventLog objLog = new EventLog();
            objLog.Source = EventLogName;
            try {
                objLog.WriteEntry(strMessage, evtType);
            } catch (Exception ex) {
                throw ex;
            }
        }

        private void StampXmlLogEntry(string strMessage, LoggerLevel level) {
            XmlDocument doc = new XmlDocument();
            string strFile = "";
            try {
                strFile = System.Configuration.ConfigurationManager.AppSettings["logFile"];
            } catch (System.Configuration.SettingsPropertyNotFoundException) {
                strFile = System.Environment.CurrentDirectory + "\\debug.log";
            }
            if (strFile == "") {
                strFile = System.Environment.CurrentDirectory + "\\debug.log";
            } else {
                strFile = System.Environment.CurrentDirectory + "\\" + strFile;
            }
            if (File.Exists(strFile)) {
                doc.Load(strFile);
                XmlNode elMessages = doc.LastChild;
                XmlElement eleMessage = doc.CreateElement("message");
                eleMessage.SetAttribute("timestamp", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.ffffff"));
                XmlText messageText = doc.CreateTextNode(strMessage);
                eleMessage.AppendChild(messageText);
                elMessages.AppendChild(eleMessage);
                doc.Save(strFile);
            } else {
                XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "utf-8", null);
                doc.InsertBefore(dec, doc.DocumentElement);
                XmlElement el = doc.CreateElement("logMessages");
                XmlElement eleMessage = doc.CreateElement("message");
                eleMessage.SetAttribute("timestamp", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.ffffff"));
                XmlText messageText = doc.CreateTextNode(strMessage);
                eleMessage.AppendChild(messageText);
                el.AppendChild(eleMessage);
                doc.AppendChild(el);
                doc.Save(strFile);
            }
        }
    }
}
