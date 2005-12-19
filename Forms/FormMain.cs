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
			this.Hide();
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
				String[] splitDeviceState = mDeviceState.Split( '\n' );
				String[] splitNewState = newState.Split( '\n' );
				
				// If the number of lines have changed, a significant configuration
				// change has occured: lines or calls have been added. Note that this
				// might have happened anyway, but the number of lines may remain the same
				// so this is not foolproof.
				if( splitDeviceState.Length != splitNewState.Length )
					NotifyIcon.ShowBalloonTip( 5000, "Significant state change", newState, ToolTipIcon.Warning  );
				else {
					// We know a minor change has occurred, but what is it?					
					StringBuilder builder = new StringBuilder();
					for( int i = 0; i < splitNewState.Length; i++ )
						if( splitDeviceState[ i ] != splitNewState[ i ] )
							builder.Append( splitNewState[ i ] ).AppendLine();

					NotifyIcon.ShowBalloonTip( 1000, "State change", builder.ToString(), ToolTipIcon.Info );
				}
						
				mDeviceState = newState;
			}

			TextBoxStatus.Text = newState;
		}

		private void toolStripQuit_Click( object sender, EventArgs e ) {
			Application.ExitThread();
		}
    }
}