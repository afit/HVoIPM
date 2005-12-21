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
    }
}