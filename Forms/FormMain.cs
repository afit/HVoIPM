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
using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP.Forms {
    public partial class FormMain : Form {
		protected String mDeviceState = "";
		
        public FormMain() {
            InitializeComponent();
            // FIXME implement proper thread-handling, if the textbox is to remain
            Control.CheckForIllegalCrossThreadCalls = false;
			StateManager.Instance().StateUpdate += new StateUpdateHandler( this.StateManagerUpdated );
			this.Hide();
        }

        private void FormMain_FormClosing( object sender, FormClosingEventArgs e ) {
            // Prevent closure of main window from ending application:
            // cancel closure and hide instead.
            e.Cancel = true;
            this.Hide();
        }

		public void StateManagerUpdated( DeviceStateMonitor monitor, StateUpdateEventArgs e ) {
			DeviceState deviceState = monitor.GetDeviceState();
			String newState = deviceState.ToString();
			
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
				
				// Repopulate checkboxes for error checking.
				// Maybe we should only repopulate for the device
				// that's being updated.
				ListboxLines.Items.Clear();
				
				for( int i = 0; i < deviceState.LineStates.Length; i++ ) {
					ListboxLines.Items.Add( monitor.Name + " (" + monitor.GetType().Name + ") #" + i, true );
				}
			}
		}

		private void toolStripQuit_Click( object sender, EventArgs e ) {
			NotifyIcon.Visible = false;
			this.Hide();
			Environment.Exit( 1 );
		}

		private void ButtonQuit_Click( object sender, EventArgs e ) {
			toolStripQuit_Click( sender, e );
		}

		private void ButtonHide_Click( object sender, EventArgs e ) {
			this.Hide();
		}

		private void ButtonDump_Click( object sender, EventArgs e ) {
			MessageBox.Show( mDeviceState, "Monitored device status", MessageBoxButtons.OK, MessageBoxIcon.Information );
		}

		private void ButtonReload_Click( object sender, EventArgs e ) {
			StateManager.Instance().ReloadDeviceStateMonitors();
		}

		private void NotifyIcon_MouseClick( object sender, MouseEventArgs e ) {
			// FIXME need to differentiate buttons here
			if( e.Button != MouseButtons.Left )
				return;
			if( this.Visible )
				this.Hide();
			else
				this.Show();
		}
    }
}