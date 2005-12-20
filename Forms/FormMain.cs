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
		protected Dictionary<TreeNode, Object> mTreeMapping = new Dictionary<TreeNode,object>();
		
        public FormMain() {
            InitializeComponent();
            // FIXME implement proper thread-handling, if the textbox is to remain
            Control.CheckForIllegalCrossThreadCalls = false;
			StateManager.Instance().StateUpdate += new StateUpdateHandler( this.StateManagerUpdated );
			LabelLinks.Links.Add( 2, 19, "www.lothianproductions.co.uk" );
			LabelLinks.Links.Add( 49, 12, "www.lothianproductions.co.uk/hvoipm" );
            Logger.Instance().Log("Started Hardware VoIP Monitor application");
			this.Hide();
        }

        private void FormMain_FormClosing( object sender, FormClosingEventArgs e ) {
            // Prevent closure of main window from ending application:
            // cancel closure and hide instead.
            e.Cancel = true;
            this.Hide();
        }

		public void StateManagerUpdated( IDeviceStateMonitor monitor, StateUpdateEventArgs e ) {
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
				TreeStates.Nodes.Clear();
				mTreeMapping.Clear();
				
				TreeNode deviceNode = TreeStates.Nodes.Add( monitor.Name + " (" + monitor.GetType().Name + ")" );
				mTreeMapping.Add( deviceNode, monitor );
				
				for( int i = 0; i < deviceState.LineStates.Length; i++ ) {
					TreeNode lineNode = deviceNode.Nodes.Add( "Line #" + i );
					lineNode.Nodes.Add( "Last called number = " + deviceState.LineStates[ i ].LastCalledNumber );
					lineNode.Nodes.Add( "Last caller number = " + deviceState.LineStates[ i ].LastCallerNumber );
					lineNode.Nodes.Add( "Registration state = " + deviceState.LineStates[ i ].RegistrationState );
					for( int j = 0; j < deviceState.LineStates[ i ].CallStates.Length; j++ ) {
						TreeNode callNode = lineNode.Nodes.Add( "Call #" + j );
						callNode.Nodes.Add( "Call activity = " + deviceState.LineStates[ i ].CallStates[ j ].CallActivity );
						callNode.Nodes.Add( "Duration = " + deviceState.LineStates[ i ].CallStates[ j ].Duration );
					}
				}

				TreeStates.ExpandAll();
			}
			

			
		}

		private void toolStripQuit_Click( object sender, EventArgs e ) {
            Logger.Instance().Log("Ended Hardware VoIP Monitor application");
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
			if( e.Button != MouseButtons.Left )
				return;
			if( this.Visible )
				this.Hide();
			else
				this.Show();
		}

		private void TreeStates_AfterSelect( object sender, TreeViewEventArgs e ) {
			MessageBox.Show( e.Node.Text );
		}
    }
}
