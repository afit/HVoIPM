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
            new FormMain();
            Application.Run();
        }
    }
}