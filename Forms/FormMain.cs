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
				null //behaviour.Attributes[ "showCriteria" ].Value.Split( ',' )
			);
		}

		public void StateManagerUpdated( IDeviceStateMonitor monitor, StateUpdateEventArgs e ) {
			
			// Iterate through state changes and deal with them as appropriate:
			StringBuilder bubbleTextBuilder = new StringBuilder();
			bool showApplication = false;
			bool systemTrayWarning = false;
			
			foreach( StateChange change in e.StateChanges ) {
				StateChangeBehaviour behaviour = LookupBehaviour( change.Underlying, change.Property );
			
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
		    
		    // Use each node's key as a property name. Use each node's tag object as a state.

			foreach( IDeviceStateMonitor monitor in StateManager.Instance().DeviceStateMonitors ) {
				DeviceState deviceState = monitor.GetDeviceState();
				
				TreeNode deviceNode = TreeStates.Nodes.Add( "", deviceState.Name + " (" + monitor.GetType().Name + " " + monitor.Name + ")" );
				deviceNode.Tag = deviceState;
				
				for( int i = 0; i < deviceState.LineStates.Length; i++ ) {
					TreeNode lineNode = deviceNode.Nodes.Add( "", "Line " + deviceState.LineStates[ i ].Name );
					lineNode.Tag = deviceState.LineStates[ i ];
					lineNode.Nodes.Add( "lastCalledNumber", "Last called number = " + deviceState.LineStates[ i ].LastCalledNumber ).Tag = deviceState.LineStates[ i ];
					lineNode.Nodes.Add( "lastCallerNumber", "Last caller number = " + deviceState.LineStates[ i ].LastCallerNumber ).Tag = deviceState.LineStates[ i ];
					lineNode.Nodes.Add( "registrationState", "Registration state = " + deviceState.LineStates[ i ].RegistrationState ).Tag = deviceState.LineStates[ i ];
					
					for( int j = 0; j < deviceState.LineStates[ i ].CallStates.Length; j++ ) {
						TreeNode callNode = lineNode.Nodes.Add( "Call " + deviceState.LineStates[ i ].CallStates[ j ].Name );
						callNode.Tag = deviceState.LineStates[ i ].CallStates[ j ];
						callNode.Nodes.Add( "callActivity", "Call activity = " + deviceState.LineStates[ i ].CallStates[ j ].CallActivity ).Tag = deviceState.LineStates[ i ].CallStates[ j ];
						callNode.Nodes.Add( "duration", "Duration = " + deviceState.LineStates[ i ].CallStates[ j ].Duration ).Tag = deviceState.LineStates[ i ].CallStates[ j ];
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
			StateChangeBehaviour behaviour = LookupBehaviour( e.Node.Tag, e.Node.Name );
			MessageBox.Show( e.Node.Text + ":" + behaviour );
		}
    }
}
