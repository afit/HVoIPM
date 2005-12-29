using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
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
    
		protected bool mFlashState = false;
    
        public FormMain() {
            InitializeComponent();

			LabelLinks.Links.Add( 2, 19, "www.lothianproductions.co.uk" );
			LabelLinks.Links.Add( 49, 12, "www.lothianproductions.co.uk/hvoipm" );
			
            Logger.Instance().Log( "Started Hardware VoIP Monitor application" );
            
			this.Hide();
        }


		protected IList<String> mWarnings = new List<String>();
		protected IList<DeviceMonitor> mMonitorsStarted = new List<DeviceMonitor>();
		public void StateManagerUpdated( IDeviceMonitor monitor, StateUpdateEventArgs e ) {
			
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
				
				if( behaviour.SystemTrayWarning ) {
					// Check to see if warning criteria is met.
					int f = Array.IndexOf( behaviour.WarningCriteria, change.ChangedTo );
					if( f > -1 ) {
						// Activate warning state, if it isn't already:
						if( ! mWarnings.Contains( change.Underlying.GetHashCode() + ":" + change.Underlying.GetType().Name + ":" + change.Property ) )
							mWarnings.Add( change.Underlying.GetHashCode() + ":" + change.Underlying.GetType().Name + ":" + change.Property );
					} else {
						mWarnings.Remove( change.Underlying.GetHashCode() + ":" + change.Underlying.GetType().Name + ":" + change.Property );
					}
				}
				
				if( behaviour.ShowApplication && Array.IndexOf( behaviour.WarningCriteria, change.ChangedTo ) > -1 )
					showApplication = true;
			}

			if( showApplication )
				this.Invoke(
					new ShowFormDelegate( ShowFormMain ),
					new Object[] {}
				);

			// Enable timer if there are warnings.
			this.Invoke(
				new SetTimer( SetTimerFlash ),
				new Object[] { mWarnings.Count > 0 }
			);
			
			if( ! TimerFlash.Enabled )
			    NotifyIcon.Icon = global::LothianProductions.VoIP.Properties.Resources.HVoIPM_48x;
			
			if( bubbleTextBuilder.Length > 0 )
				NotifyIcon.ShowBalloonTip(
					1000,
					monitor.Name + "'s state changed",
					bubbleTextBuilder.ToString(),
					ToolTipIcon.Info
				);
				
			TreeStates.Invoke(
			    new MonitorPassingDelegate( UpdateTree ),
			    new Object[] { changes }
			);
		}

		private delegate void SetTimer( bool enabled );
		private void SetTimerFlash( bool enabled ) {
			TimerFlash.Enabled = enabled;
		}

		private delegate void ShowFormDelegate();
		private void ShowFormMain() {
			// Part of workaround for .NET bug as described
			// in Program.cs.
			this.Show();
			
			if( WindowState != FormWindowState.Normal )
				WindowState = FormWindowState.Normal;
				
			if( ! ShowInTaskbar )
				ShowInTaskbar = true;
		}
		
		private delegate void MonitorPassingDelegate( IList<Change> changes );
		private void UpdateTree( IList<Change> changes ) {
			// If the change has occurred on an object that doesn't exist in the tree
			// then create it. Otherwise, update it.
			
			// FIXME add support for name changes?	
			foreach( Change change in changes ) {
				TreeNode[] nodes = TreeStates.Nodes.Find( DeviceMonitor.PROPERTY_NAME, true );
				bool found = false;
				
				foreach( TreeNode node in nodes )
					if( node.Tag == change.Underlying ) {
						found = true;
						EnsureNodeContains( node, change.Underlying, change.Property, change.ChangedFrom, change.ChangedTo );
						break;
					}
				
				if( ! found )
					AddMonitorToTree( change.GetDeviceMonitor() );
			}
		}
		
		public static void EnsureNodeContains( TreeNode node, Object state, String property, String changedFrom, String changedTo ) {
			if( ! node.Nodes.ContainsKey( property ) )
				node.Nodes.Add( property, String.Format( StateManager.Instance().LookupBehaviour( state, property ).BubbleText, new String[] { property, changedFrom, changedTo } ) ).Tag = state;
			else
				node.Nodes[ property ].Text = String.Format( StateManager.Instance().LookupBehaviour( state, property ).BubbleText, new String[] { property, changedFrom, changedTo } );
		}
		
		private void AddMonitorToTree( IDeviceMonitor monitor ) {
		    // Use each node's key as a property name. Use each node's tag object as a state.
			Device deviceState = monitor.GetDeviceState();
			
			TreeNode deviceNode = TreeStates.Nodes.Add( DeviceMonitor.PROPERTY_NAME, deviceState.Name );
			deviceNode.Tag = deviceState;
			
			for( int i = 0; i < deviceState.Lines.Length; i++ ) {
				TreeNode lineNode = deviceNode.Nodes.Add( DeviceMonitor.PROPERTY_NAME, deviceState.Lines[ i ].Name );
				lineNode.Tag = deviceState.Lines[ i ];
				
				EnsureNodeContains( lineNode, deviceState.Lines[ i ], DeviceMonitor.PROPERTY_LASTCALLEDNUMBER, "", deviceState.Lines[ i ].LastCalledNumber );
				EnsureNodeContains( lineNode, deviceState.Lines[ i ], DeviceMonitor.PROPERTY_LASTCALLERNUMBER, "", deviceState.Lines[ i ].LastCallerNumber );
				EnsureNodeContains( lineNode, deviceState.Lines[ i ], DeviceMonitor.PROPERTY_MESSAGEWAITING, "", deviceState.Lines[ i ].MessageWaiting.ToString() );
				EnsureNodeContains( lineNode, deviceState.Lines[ i ], DeviceMonitor.PROPERTY_REGISTRATIONSTATE, "", deviceState.Lines[ i ].RegistrationState.ToString() );
				
				for( int j = 0; j < deviceState.Lines[ i ].Calls.Length; j++ ) {
					TreeNode callNode = lineNode.Nodes.Add( DeviceMonitor.PROPERTY_NAME, deviceState.Lines[ i ].Calls[ j ].Name );
					callNode.Tag = deviceState.Lines[ i ].Calls[ j ];
					
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_ACTIVITY, "", deviceState.Lines[ i ].Calls[ j ].Activity.ToString() );
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_BYTESRECEIVED, "", deviceState.Lines[ i ].Calls[ j ].BytesReceived.ToString() );
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_BYTESSENT, "", deviceState.Lines[ i ].Calls[ j ].BytesSent.ToString() );
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_DECODELATENCY, "", deviceState.Lines[ i ].Calls[ j ].DecodeLatency.ToString() );
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_DECODER, "", deviceState.Lines[ i ].Calls[ j ].Decoder );
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_DURATION, "", deviceState.Lines[ i ].Calls[ j ].Duration );
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_ENCODER, "", deviceState.Lines[ i ].Calls[ j ].Encoder );
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_JITTER, "", deviceState.Lines[ i ].Calls[ j ].Jitter.ToString() );
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_PACKETERROR, "", deviceState.Lines[ i ].Calls[ j ].PacketError.ToString() );
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_PACKETLOSS, "", deviceState.Lines[ i ].Calls[ j ].PacketLoss.ToString() );
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_ROUNDTRIPDELAY, "", deviceState.Lines[ i ].Calls[ j ].RoundTripDelay.ToString() );
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_TONE, "", deviceState.Lines[ i ].Calls[ j ].Tone.ToString() );
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_TYPE, "", deviceState.Lines[ i ].Calls[ j ].Type.ToString() );
				}
			}

		    deviceNode.ExpandAll();
		}

		#region Form Events
        private void FormMain_FormClosing( object sender, FormClosingEventArgs e ) {
            // Prevent closure of main window from ending application:
            // cancel closure and hide instead.
            e.Cancel = true;
            this.Hide();
        }

		private void NotifyIcon_MouseClick( object sender, MouseEventArgs e ) {
			if( e.Button != MouseButtons.Left )
				return;
			
			if( this.Visible )
				this.Hide();
			else
				ShowFormMain();
		}

		private void TimerFlash_Tick( object sender, EventArgs e ) {					
			// FIXME why can't we compare icons or their handles?
			// if( NotifyIcon.Icon.Handle == global::LothianProductions.VoIP.Properties.Resources.HVoIPM.Handle ) 
			if( mFlashState )
				NotifyIcon.Icon = global::LothianProductions.VoIP.Properties.Resources.HVoIPM_48x_other;
			else
				NotifyIcon.Icon = global::LothianProductions.VoIP.Properties.Resources.HVoIPM_48x;
				
			mFlashState = ! mFlashState;
		}
		
		private void LabelLinks_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e ) {
			Process.Start( e.Link.LinkData as String );
		}

		private void toolStripCallLog_Click(object sender, EventArgs e) {
			new CallRecordViewer().Show();
		}

		private void toolStripQuit_Click( object sender, EventArgs e ) {
            Logger.Instance().Log( "Ended Hardware VoIP Monitor application" );
            NotifyIcon.Visible = false;
			this.Hide();
			Environment.Exit( 1 );
		}

		private void toolStripShowMain_Click( object sender, EventArgs e ) {
			ShowFormMain();
		}
		#endregion
    }
}