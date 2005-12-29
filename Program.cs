using System;
using System.Collections.Generic;
using System.Windows.Forms;

using LothianProductions.VoIP.Forms;

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
			//Clipboard.SetText( e.ExceptionObject.ToString() );   
			if( e.ExceptionObject != null )
				MessageBox.Show(
					"A fatal, unhandled error has occurred within HVoIPM, and the application must close." +
					"This was probably caused by a fault within a 3rd party device monitor plugin.\n\n" + 
					"We're very sorry this happened. If you report the error below to the developers, at " +
					"hvoipm@lothianproductions.co.uk, they may be able to help you fix the problem.\n\n" +
					e.ExceptionObject.ToString(), "Hardware VoIP Monitor", MessageBoxButtons.OK );
		}
    }
    
    
}