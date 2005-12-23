using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

using LothianProductions.Util;
using LothianProductions.Util.Http;
using LothianProductions.Util.Settings;
using LothianProductions.VoIP;
using LothianProductions.VoIP.Behaviour;
using LothianProductions.VoIP.Monitor;
using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP.Forms {
    public partial class FormMain : Form {
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

		protected IList<String> mWarnings = new List<String>();
		public void StateManagerUpdated( IDeviceStateMonitor monitor, StateUpdateEventArgs e ) {
			
			// Iterate through state changes and deal with them as appropriate:
			StringBuilder bubbleTextBuilder = new StringBuilder();
			bool showApplication = false;
			
			// FIXME this is very inefficient
			List<Change> changes = new List<Change>();
			foreach( Change change in e.DeviceStateChanges )
				changes.Add( change );
			foreach( Change change in e.LineStateChanges )
				changes.Add( change );
			foreach( Change change in e.CallStateChanges )
				changes.Add( change );
			
			foreach( Change change in changes ) {
				StateChangeBehaviour behaviour = StateManager.Instance().LookupBehaviour( change.Underlying, change.Property );
			
				if( behaviour.ShowBubble ) {
					if( bubbleTextBuilder.Length > 0 )
						bubbleTextBuilder.AppendLine();
			
					// Substitute in values from pattern.			
					bubbleTextBuilder.Append( String.Format( behaviour.BubbleText, new String[] { change.Property, change.ChangedFrom, change.ChangedTo } ) );
				}
				
				if( behaviour.SystemTrayWarning )
					// Check to see if warning criteria is met.
					if( Array.IndexOf( behaviour.WarningCriteria, change.ChangedTo ) > -1 ) {
						// Activate warning state, if it isn't already:
						if( ! mWarnings.Contains( change.Underlying.GetHashCode() + ":" + change.Underlying.GetType().Name + ":" + change.Property ) )
							mWarnings.Add( change.Underlying.GetHashCode() + ":" + change.Underlying.GetType().Name + ":" + change.Property );
					} else {
						mWarnings.Remove( change.Underlying.GetHashCode() + ":" + change.Underlying.GetType().Name + ":" + change.Property );
					}
				
				if( behaviour.ShowApplication && Array.IndexOf( behaviour.WarningCriteria, change.ChangedTo ) > -1 )
					showApplication = true;
			}

			if( showApplication )
				this.Show();

			// Enable timer if there are warnings.
			TimerFlash.Enabled = mWarnings.Count > 0;
			
			if( ! TimerFlash.Enabled )
				NotifyIcon.Icon = global::LothianProductions.VoIP.Properties.Resources.HVoIPM_48x;
			

			if( bubbleTextBuilder.Length > 0 )
				NotifyIcon.ShowBalloonTip(
					1000,
					monitor.Name + "'s state changed",
					bubbleTextBuilder.ToString(),
					ToolTipIcon.Info
				);
				
			// FIXME need to support line and call addition or removal
			
			TreeStates.Invoke(
				new MonitorPassingDelegate( UpdateTree ),
				new Object[] { e.DeviceStateChanges, e.LineStateChanges, e.CallStateChanges }
			);
		}
	
		public delegate void MonitorPassingDelegate(	IList<DeviceChange> deviceChanges, IList<LineChange> lineChanges,
														IList<CallChange> callChanges );

		private void UpdateTree(	IList<DeviceChange> deviceChanges, IList<LineChange> lineChanges,
									IList<CallChange> callChanges ) {
			// If the change has occurred on an object that doesn't exist in the tree
			// then create it. Otherwise, update it.
			
			// Ensure that each device is already in the tree:
			foreach( DeviceChange change in deviceChanges ) {
				bool found = false;
				
				foreach( TreeNode node in TreeStates.Nodes )
					if( node.Tag == change.Device ) {
						found = true;
						node.Text = change.Device.Name;
					}
									
				if( ! found ) {
					AddMonitorToTree( StateManager.Instance().GetMonitor( change.Device ) );
					// FIXME inefficient
					UpdateTree( deviceChanges, new List<LineChange>(), new List<CallChange>() );
				}
			}
			
			foreach( LineChange change in lineChanges ) {
				bool found = false;
				
				foreach( TreeNode deviceNode in TreeStates.Nodes )
					foreach( TreeNode node in deviceNode.Nodes )
						if( node.Tag == change.Line ) {
							found = true;
							
							if( ! node.Nodes.ContainsKey( change.Property ) )
								node.Nodes.Add( change.Property, String.Format( StateManager.Instance().LookupBehaviour( change.Line, change.Property ).BubbleText, new String[] { change.Property, change.ChangedFrom, change.ChangedTo } ) ).Tag = change.Line;
							else
								node.Nodes[ change.Property ].Text = String.Format( StateManager.Instance().LookupBehaviour( change.Line, change.Property ).BubbleText, new String[] { change.Property, change.ChangedFrom, change.ChangedTo } );

						}
									
				if( ! found ) {
					AddMonitorToTree( StateManager.Instance().GetMonitor( change.Line ) );
					// FIXME inefficient
					UpdateTree( new List<DeviceChange>(), lineChanges, new List<CallChange>() );
				}
			}
			
			foreach( CallChange change in callChanges ) {
				bool found = false;
				
				foreach( TreeNode deviceNode in TreeStates.Nodes )
					foreach( TreeNode lineNode in deviceNode.Nodes )
							foreach( TreeNode node in lineNode.Nodes )
							if( node.Tag == change.Call ) {
								found = true;
								
								if( ! node.Nodes.ContainsKey( change.Property ) )
									node.Nodes.Add( change.Property, String.Format( StateManager.Instance().LookupBehaviour( change.Call, change.Property ).BubbleText, new String[] { change.Property, change.ChangedFrom, change.ChangedTo } ) ).Tag = change.Call;
								else
									node.Nodes[ change.Property ].Text = String.Format( StateManager.Instance().LookupBehaviour( change.Call, change.Property ).BubbleText, new String[] { change.Property, change.ChangedFrom, change.ChangedTo } );
									//node.Nodes[ change.Property ].Text = StateManager.Instance().LookupBehaviour( change.Call, change.Property ).BubbleText + " = " + change.ChangedTo;
							}
									
				if( ! found ) {
					AddMonitorToTree( StateManager.Instance().GetMonitor( change.Call ) );
					// FIXME inefficient
					UpdateTree( new List<DeviceChange>(), new List<LineChange>(), callChanges );
				}
			}
		}
		
		protected void AddMonitorToTree( IDeviceStateMonitor monitor ) {
		    // Use each node's key as a property name. Use each node's tag object as a state.
		    // FIXME should we implement name-change support?
			Device deviceState = monitor.GetDeviceState();
			
			TreeNode deviceNode = TreeStates.Nodes.Add( "name", deviceState.Name );
			deviceNode.Tag = deviceState;
			
			for( int i = 0; i < deviceState.Lines.Length; i++ ) {
				TreeNode lineNode = deviceNode.Nodes.Add( "name", deviceState.Lines[ i ].Name );
				lineNode.Tag = deviceState.Lines[ i ];
				
				for( int j = 0; j < deviceState.Lines[ i ].Calls.Length; j++ )
					lineNode.Nodes.Add( "name", deviceState.Lines[ i ].Calls[ j ].Name ).Tag = deviceState.Lines[ i ].Calls[ j ];
			}

		    deviceNode.ExpandAll();
		}

		private void toolStripQuit_Click( object sender, EventArgs e ) {
            Logger.Instance().Log( "Ended Hardware VoIP Monitor application" );
            NotifyIcon.Visible = false;
			this.Hide();
			Environment.Exit( 1 );
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

		//private void TreeStates_AfterSelect( object sender, TreeViewEventArgs e ) {
		//    StateChangeBehaviour behaviour = StateManager.Instance().LookupBehaviour( e.Node.Tag, e.Node.Name );
		//    TextboxBehaviour.Text = e.Node.Text + "\n\n" + behaviour.ToString();
		//}

		protected bool mFlashState = false;
		private void TimerFlash_Tick( object sender, EventArgs e ) {
			// FIXME why can't we compare icons or their handles?
			// if( NotifyIcon.Icon.Handle == global::LothianProductions.VoIP.Properties.Resources.HVoIPM.Handle ) 
			
			if( mFlashState )
				NotifyIcon.Icon = global::LothianProductions.VoIP.Properties.Resources.HVoIPM_flash;
			else
				NotifyIcon.Icon = global::LothianProductions.VoIP.Properties.Resources.HVoIPM_48x;
				
			mFlashState = ! mFlashState;									
		}
    }
}
