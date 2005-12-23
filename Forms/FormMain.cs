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
		Dictionary<Object, Dictionary<String, StateChangeBehaviour>> mStateProperties = new Dictionary<Object, Dictionary<String, StateChangeBehaviour>>();
				
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

		protected StateChangeBehaviour LookupBehaviour( Object state, String property ) {		
			// Behaviour not set yet.
			if( ! mStateProperties.ContainsKey( state ) )
				mStateProperties.Add( state, new Dictionary<String, StateChangeBehaviour>() );
			
			Dictionary<String, StateChangeBehaviour> propertyBehaviours = mStateProperties[ state ];
			
			if( ! propertyBehaviours.ContainsKey( property ) )
				propertyBehaviours.Add( property, GetBehaviourFromXml( state.GetType().Name, property ) );
				
			return propertyBehaviours[ property ];			
		}
		
		protected StateChangeBehaviour GetBehaviourFromXml( String stateType, String property ) {
			XmlNode node = (XmlNode) ConfigurationManager.GetSection( "hvoipm/behaviours" );
			
			if( node == null )
				throw new ConfigurationErrorsException( "Could not find behaviours section in application configuration." );

			XmlNode behaviour = node.SelectSingleNode( "behaviour[@stateType='" + stateType + "' and @property='" + property + "']" );
			if( behaviour == null )
				throw new ConfigurationErrorsException( "Could not find behaviour description for " + stateType + "." + property + "\" in application configuration." );

			return new StateChangeBehaviour(
				Boolean.Parse( behaviour.Attributes[ "showBubble" ].Value ),
				behaviour.Attributes[ "bubbleText" ].Value,
				Boolean.Parse( behaviour.Attributes[ "systemTrayWarning" ].Value ),
				Boolean.Parse( behaviour.Attributes[ "showApplication" ].Value ),
				( behaviour.Attributes[ "showCriteria" ] == null ? "" : behaviour.Attributes[ "showCriteria" ].Value ).Split( ',' ),
				Boolean.Parse( behaviour.Attributes[ "log" ].Value )
			);
		}

		protected IList<String> mWarnings = new List<String>();
		public void StateManagerUpdated( IDeviceStateMonitor monitor, StateUpdateEventArgs e ) {
			
			// Iterate through state changes and deal with them as appropriate:
			StringBuilder bubbleTextBuilder = new StringBuilder();
			bool showApplication = false;
			
			// FIXME this is very inefficient
			List<StateChange> changes = new List<StateChange>();
			foreach( StateChange change in e.DeviceStateChanges )
				changes.Add( change );
			foreach( StateChange change in e.LineStateChanges )
				changes.Add( change );
			foreach( StateChange change in e.CallStateChanges )
				changes.Add( change );
			
			foreach( StateChange change in changes ) {
				StateChangeBehaviour behaviour = LookupBehaviour( change.Underlying, change.Property );
			
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
				NotifyIcon.Icon = global::LothianProductions.VoIP.Properties.Resources.HVoIPM;
			

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
	
		public delegate void MonitorPassingDelegate(	IList<DeviceStateChange> deviceChanges, IList<LineStateChange> lineChanges,
														IList<CallStateChange> callChanges );

		private void UpdateTree(	IList<DeviceStateChange> deviceChanges, IList<LineStateChange> lineChanges,
									IList<CallStateChange> callChanges ) {
			// If the change has occurred on an object that doesn't exist in the tree
			// then create it. Otherwise, update it.
			
			// Ensure that each device is already in the tree:
			foreach( DeviceStateChange change in deviceChanges ) {
				bool found = false;
				
				foreach( TreeNode node in TreeStates.Nodes )
					if( node.Tag == change.DeviceState ) {
						found = true;
						node.Text = change.DeviceState.Name;
					}
									
				if( ! found ) {
					AddMonitorToTree( StateManager.Instance().GetMonitor( change.DeviceState ) );
					// FIXME inefficient
					UpdateTree( deviceChanges, new List<LineStateChange>(), new List<CallStateChange>() );
				}
			}
			
			foreach( LineStateChange change in lineChanges ) {
				bool found = false;
				
				foreach( TreeNode deviceNode in TreeStates.Nodes )
					foreach( TreeNode node in deviceNode.Nodes )
						if( node.Tag == change.LineState ) {
							found = true;
							node.Text = "Line " + change.LineState.Name;
							
							if( ! node.Nodes.ContainsKey( change.Property ) )
								node.Nodes.Add( change.Property, change.Property + " = " + change.ChangedTo ).Tag = change.LineState;
							else
								node.Nodes[ change.Property ].Text = change.Property + " = " + change.ChangedTo;

						}
									
				if( ! found ) {
					AddMonitorToTree( StateManager.Instance().GetMonitor( change.LineState ) );
					// FIXME inefficient
					UpdateTree( new List<DeviceStateChange>(), lineChanges, new List<CallStateChange>() );
				}
			}
			
			foreach( CallStateChange change in callChanges ) {
				bool found = false;
				
				foreach( TreeNode deviceNode in TreeStates.Nodes )
					foreach( TreeNode lineNode in deviceNode.Nodes )
							foreach( TreeNode node in lineNode.Nodes )
							if( node.Tag == change.CallState ) {
								found = true;
								node.Text = "Call " + change.CallState.Name;
								
								if( ! node.Nodes.ContainsKey( change.Property ) )
									node.Nodes.Add( change.Property, change.Property + " = " + change.ChangedTo ).Tag = change.CallState;
								else
									node.Nodes[ change.Property ].Text = change.Property + " = " + change.ChangedTo;
							}
									
				if( ! found ) {
					AddMonitorToTree( StateManager.Instance().GetMonitor( change.CallState ) );
					// FIXME inefficient
					UpdateTree( new List<DeviceStateChange>(), new List<LineStateChange>(), callChanges );
				}
			}
		}
		
		protected void AddMonitorToTree( IDeviceStateMonitor monitor ) {
		    // Use each node's key as a property name. Use each node's tag object as a state.
			DeviceState deviceState = monitor.GetDeviceState();
			
			TreeNode deviceNode = TreeStates.Nodes.Add( "", deviceState.Name );
			deviceNode.Tag = deviceState;
			
			for( int i = 0; i < deviceState.LineStates.Length; i++ ) {
				TreeNode lineNode = deviceNode.Nodes.Add( "", "" );
				lineNode.Tag = deviceState.LineStates[ i ];
				
				for( int j = 0; j < deviceState.LineStates[ i ].CallStates.Length; j++ )
					lineNode.Nodes.Add( "", "" ).Tag = deviceState.LineStates[ i ].CallStates[ j ];
			}

		    deviceNode.ExpandAll();
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
			//StateChangeBehaviour behaviour = LookupBehaviour( e.Node.Tag, e.Node.Name );
			//MessageBox.Show( e.Node.Text + ":" + behaviour );
		}

		protected bool mFlashState = false;
		private void TimerFlash_Tick( object sender, EventArgs e ) {
			// FIXME why can't we compare icons or their handles?
			// if( NotifyIcon.Icon.Handle == global::LothianProductions.VoIP.Properties.Resources.HVoIPM.Handle ) 
			
			if( mFlashState )
				NotifyIcon.Icon = global::LothianProductions.VoIP.Properties.Resources.HVoIPM_flash;
			else
				NotifyIcon.Icon = global::LothianProductions.VoIP.Properties.Resources.HVoIPM;
				
			mFlashState = ! mFlashState;									
		}
    }
}
