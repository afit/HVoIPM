using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using LothianProductions.Util.Http;
using LothianProductions.VoIP;
using LothianProductions.VoIP.Behaviour;
using LothianProductions.VoIP.Monitor;
using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP.Forms {
    public partial class FormMain : Form {
		protected Dictionary<TreeNode, Object> mTreeMapping = new Dictionary<TreeNode,object>();
		
        public FormMain() {
            InitializeComponent();
            // FIXME implement proper thread-handling, if the textbox is to remain
            Control.CheckForIllegalCrossThreadCalls = false;
			
			LabelLinks.Links.Add( 2, 19, "www.lothianproductions.co.uk" );
			LabelLinks.Links.Add( 49, 12, "www.lothianproductions.co.uk/hvoipm" );
			
            Logger.Instance().Log( "Started Hardware VoIP Monitor application" );
            
			this.Hide();
        }

        private void FormMain_FormClosing( object sender, FormClosingEventArgs e ) {
            // Prevent closure of main window from ending application:
            // cancel closure and hide instead.
            e.Cancel = true;
            this.Hide();
        }

		protected StateChangeBehaviour LookupBehaviour( StateChange change ) {
			return new StateChangeBehaviour();
		}

		public void StateManagerUpdated( IDeviceStateMonitor monitor, StateUpdateEventArgs e ) {
		
			// Iterate through state changes and deal with them as appropriate:
			StringBuilder bubbleTextBuilder = new StringBuilder();
			bool showApplication = false;
			bool systemTrayWarning = false;
			
			foreach( StateChange change in e.StateChanges ) {
				StateChangeBehaviour behaviour = LookupBehaviour( change );
			
				if( behaviour.ShowBubble ) {
					if( bubbleTextBuilder.Length > 0 )
						bubbleTextBuilder.AppendLine();
			
					// Substitute in values from pattern.			
					bubbleTextBuilder.Append( String.Format( behaviour.BubbleText, new String[] { change.Property, change.ChangedFrom, change.ChangedTo } ) );
				}
				
				if( behaviour.SystemTrayWarning )
					systemTrayWarning = true;
				
				if( behaviour.ShowApplication )
					showApplication = true;
			}

			if( showApplication )
				this.Show();

			// FIXME implement this: if( systemTrayWarning )
			

			if( bubbleTextBuilder.Length > 0 )
				NotifyIcon.ShowBalloonTip(
					1000,
					monitor.Name + "'s state changed",
					bubbleTextBuilder.ToString(),
					ToolTipIcon.Info
				);
				
			// FIXME need to support line and call addition or removal
			
			TreeStates.Invoke( new MonitorPassingDelegate( UpdateTree ), new Object[] { e.StateChanges } );			
		}
	
		public delegate void MonitorPassingDelegate( IList<StateChange> changes );

		private void UpdateTree( IList<StateChange> changes ) {
		    // Redraw tree of devices being watched.
		    // FIXME Maybe we should only repopulate for the device
		    // that's being updated.
		    TreeStates.Nodes.Clear();
		    mTreeMapping.Clear();

			foreach( IDeviceStateMonitor monitor in StateManager.Instance().DeviceStateMonitors ) {
				TreeNode deviceNode = TreeStates.Nodes.Add( monitor.Name + " (" + monitor.GetType().Name + ")" );
				mTreeMapping.Add( deviceNode, monitor );
			    DeviceState deviceState = monitor.GetDeviceState();
				
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
			}

		    TreeStates.ExpandAll();
		}

		private void toolStripQuit_Click( object sender, EventArgs e ) {
            Logger.Instance().Log( "Ended Hardware VoIP Monitor application" );
            NotifyIcon.Visible = false;
			this.Hide();
			Environment.Exit( 1 );
		}

		private void ButtonQuit_Click( object sender, EventArgs e ) {
			toolStripQuit_Click( sender, e );
		}

		//private void ButtonDump_Click( object sender, EventArgs e ) {
		//    MessageBox.Show( mDeviceState, "Monitored device status", MessageBoxButtons.OK, MessageBoxIcon.Information );
		//}

		private void ButtonReload_Click( object sender, EventArgs e ) {
			StateManager.Instance().ReloadDeviceStateMonitors();
		}

		private void NotifyIcon_MouseClick( object sender, MouseEventArgs e ) {
			if( e.Button != MouseButtons.Left )
				return;
			if( this.Visible )
				this.Hide();
			else {		
				// Part of workaround for .NET bug as described
				// in Program.cs.
				this.Show();
				
				if( WindowState != FormWindowState.Normal )
					WindowState = FormWindowState.Normal;
				
				if( ! ShowInTaskbar )
					ShowInTaskbar = true;
			}
		}

		private void TreeStates_AfterSelect( object sender, TreeViewEventArgs e ) {
			MessageBox.Show( e.Node.Text );
		}
    }
}
