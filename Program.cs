using System;
using System.Collections.Generic;
using System.Windows.Forms;

using LothianProductions.VoIP.Forms;

using Microsoft.Win32;

namespace LothianProductions.VoIP {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );
            
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler( MonitorFailureHandler );
			SystemEvents.SessionEnding += new SessionEndingEventHandler(SystemEvents_SessionEnding); 
			
			
            // Workaround for .NET bug -- have to show & then hide
            // form in order to create control handles for event-driven
            // delegates to call back to. Simply constructing isn't enough.
            FormMain form = new FormMain();
			form.ShowInTaskbar = false;
			form.WindowState = FormWindowState.Minimized;
			form.Show();
			form.Hide();
			StateManager.Instance().StateUpdate += new StateUpdateHandler( form.StateManagerUpdated );
            
            Application.Run();
        }

		private static void MonitorFailureHandler(Object sender, UnhandledExceptionEventArgs e) { 
			if( e.ExceptionObject != null )
				MessageBox.Show(
					"A fatal, unhandled error has occurred within HVoIPM, and the application must close." +
					"We're very sorry this happened. If you report the error below to the developers, at " +
					"hvoipm@lothianproductions.co.uk, they may be able to help you fix the problem.\n\n" +
					e.ExceptionObject.ToString(), "Hardware VoIP Monitor", MessageBoxButtons.OK );
		}

		private static void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e) {
			Environment.Exit(1);
		}
	}
}