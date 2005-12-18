using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using LothianProductions.Util.Http;
using LothianProductions.VoIP;
using LothianProductions.VoIP.Monitor;

namespace LothianProductions.VoIP.Forms {
    public partial class FormMain : Form {
		protected StateManager mStateManager = StateManager.Instance();
		protected String mDeviceState = "";
		
        public FormMain() {
            InitializeComponent();
            // FIXME implement proper thread-handling, if the textbox is to remain
            Control.CheckForIllegalCrossThreadCalls = false;
			mStateManager.StateUpdate += new StateUpdateHandler( this.StateManagerUpdated );
        }

        private void FormMain_FormClosing( object sender, FormClosingEventArgs e ) {
            // Prevent closure of main window from ending application:
            // cancel closure and hide instead.
            e.Cancel = true;
            this.Hide();
        }

        private void NotifyIcon_Click( object sender, EventArgs e ) {
			// FIXME need to differentiate buttons here
			if( this.Visible )
				this.Hide();
			else
				this.Show();
        }

		public void StateManagerUpdated( DeviceMonitor monitor, StateUpdateEventArgs e ) {
			String newState = monitor.GetDeviceState().ToString();
			
			if( mDeviceState != newState ) {
				NotifyIcon.BalloonTipIcon = ToolTipIcon.Info;
				NotifyIcon.BalloonTipTitle = "State changed";
				NotifyIcon.BalloonTipText = newState; //newState.Substring( 0, 50 );
				NotifyIcon.ShowBalloonTip( 5000 );
				mDeviceState = newState;
			}
			
			TextBoxStatus.Text = newState;
		}

		private void toolStripQuit_Click( object sender, EventArgs e ) {
			Application.ExitThread();
		}
    }
}