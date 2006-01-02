using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

//using Microsoft.Office.Interop.Outlook;

using LothianProductions.Util;
using LothianProductions.Util.Http;
using LothianProductions.Util.Settings;
using LothianProductions.VoIP;
using LothianProductions.VoIP.Monitor;
using LothianProductions.VoIP.State;

using Microsoft.Win32;

namespace LothianProductions.VoIP.Forms {
    public partial class FormMain : Form {
    
		protected bool mFlashState = false;
		protected IList<String> mWarnings = new List<String>();
		protected IList<DeviceMonitor> mMonitorsStarted = new List<DeviceMonitor>();
		protected const int BUBBLE_TIMEOUT = 1000;
		protected const String UPDATE_URL = "http://www.lothianproductions.co.uk/hvoipm/latest-version";
		protected const String STARTUP_REG_KEY = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
    
        public FormMain() {
            InitializeComponent();

			bool runOnStartup = CheckStartupRegKey();
			if ( runOnStartup )
				toolStripLoadAtStartup.Checked = true;

			LabelLinks.Links.Add( 2, 19, "http://www.lothianproductions.co.uk" );
			LabelLinks.Links.Add( 49, 12, "http://www.lothianproductions.co.uk/hvoipm" );
			
			this.Text = this.Text + " " + Application.ProductVersion;
			NotifyIcon.Text = NotifyIcon.Text + " " + Application.ProductVersion;
			// LabelTitle.Text = LabelTitle.Text + " " + Application.ProductVersion;
			
			if( Boolean.Parse( ((NameValueCollection) ConfigurationManager.GetSection( "hvoipm/preferences" ))[ "startupCheckForUpdates" ] ) ) {
				String latestVersion;
				
				try {
					latestVersion = FindLatestVersion();
					
					if( latestVersion != Application.ProductVersion ) {
						LabelUpdates.Text = "Newer version " + FindLatestVersion() + " available.";
						LabelUpdates.Links.Add( 0, LabelUpdates.Text.Length, "http://www.lothianproductions.co.uk/hvoipm" );
					}
				} catch (WebException) {
					LabelUpdates.Text = "The update server cannot be contacted. Check manually for updates?";		
					LabelUpdates.Links.Clear();
				}
			}
			
            Logger.Instance().Log( "Started Hardware VoIP Monitor application" );
            
			this.Hide();
        }


		public void StateManagerUpdated( IDeviceMonitor monitor, StateUpdateEventArgs e ) {
			
			// Iterate through state changes and deal with them as appropriate:
			List<String> externalProcesses = new List<String>();
			StringBuilder bubbleTextBuilder = new StringBuilder();
			bool showApplication = false;
			
			// FIXME this is very inefficient
			List<PropertyChange> changes = new List<PropertyChange>();
			foreach( PropertyChange change in e.DeviceStateChanges )
				changes.Add( change );
			foreach( PropertyChange change in e.LineStateChanges )
				changes.Add( change );
			foreach( PropertyChange change in e.CallStateChanges )
				changes.Add( change );
			
			foreach( PropertyChange change in changes ) {
				PropertyChangeBehaviour behaviour = StateManager.Instance().LookupPropertyChangeBehaviour( change.Underlying, change.Property, change.ChangedTo );
			
				if( behaviour.ShowBubble ) {
					if( bubbleTextBuilder.Length > 0 )
						bubbleTextBuilder.AppendLine();
			
					// Substitute in values from pattern.			
					bubbleTextBuilder.Append( FormatString( behaviour.BubbleText, change.Property, change.ChangedFrom, change.ChangedTo ) );
				}
				
				// Check to see if warning criteria is met.
				if( behaviour.SystemTrayWarning ) {			
					// Activate warning state, if it isn't already:
					if( ! mWarnings.Contains( change.Underlying.GetHashCode() + ":" + change.Underlying.GetType().Name + ":" + change.Property ) )
						mWarnings.Add( change.Underlying.GetHashCode() + ":" + change.Underlying.GetType().Name + ":" + change.Property );
				} else {
					mWarnings.Remove( change.Underlying.GetHashCode() + ":" + change.Underlying.GetType().Name + ":" + change.Property );
				}
				
				if( behaviour.ShowApplication )
					showApplication = true;

				if( behaviour.ExternalProcess != "" )
					externalProcesses.Add( FormatString( behaviour.ExternalProcess, change.Property, change.ChangedFrom, change.ChangedTo ) );
			}

			TreeStates.Invoke(
			    new MonitorPassingDelegate( UpdateTree ),
			    new Object[] { changes }
			);

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
			
			foreach( String externalProcess in externalProcesses )
				try {
					Process.Start( externalProcess );
				} catch( Win32Exception ) {
					if( bubbleTextBuilder.Length > 0 )
						bubbleTextBuilder.AppendLine();
					bubbleTextBuilder.Append( "Failed to launch external process \"" + externalProcess + "\"" );
				}
				
			if( bubbleTextBuilder.Length > 0 )
				NotifyIcon.ShowBalloonTip(
					BUBBLE_TIMEOUT,
					monitor.Name + "'s state changed",
					bubbleTextBuilder.ToString(),
					ToolTipIcon.Info
				);
		}
		
		public static String FormatString( String template, String property, String changedFrom, String changedTo ) {
			return String.Format( template, new String[] { property, changedFrom, changedTo } );
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
			
			if( WindowState == FormWindowState.Minimized )
				WindowState = FormWindowState.Normal;
				
			if( ! ShowInTaskbar )
				ShowInTaskbar = true;
		}
		
		private delegate void MonitorPassingDelegate( IList<PropertyChange> changes );
		private void UpdateTree( IList<PropertyChange> changes ) {
			// If the change has occurred on an object that doesn't exist in the tree
			// then create it. Otherwise, update it.
			
			// FIXME add support for name changes?	
			foreach( PropertyChange change in changes ) {
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
				node.Nodes.Add( property, FormatString( StateManager.Instance().LookupPropertyBehaviour( state, property ).Label, property, changedFrom, changedTo ) ).Tag = state;
			else
				node.Nodes[ property ].Text = FormatString( StateManager.Instance().LookupPropertyBehaviour( state, property ).Label, property, changedFrom, changedTo );
		}
		
		private void AddMonitorToTree( IDeviceMonitor monitor ) {
		    // Use each node's key as a property name. Use each node's tag object as a state.
			Device deviceState = monitor.GetDeviceState();
			
			TreeNode deviceNode = TreeStates.Nodes.Add( DeviceMonitor.PROPERTY_NAME, deviceState.Name );
			deviceNode.Tag = deviceState;
			
			for( int i = 0; i < deviceState.Lines.Length; i++ ) {
				TreeNode lineNode = deviceNode.Nodes.Add( DeviceMonitor.PROPERTY_NAME, deviceState.Lines[ i ].Name );
				lineNode.Tag = deviceState.Lines[ i ];
				
				EnsureNodeContains( lineNode, deviceState.Lines[ i ], DeviceMonitor.PROPERTY_LINE_LASTCALLEDNUMBER, "", deviceState.Lines[ i ].LastCalledNumber );
				EnsureNodeContains( lineNode, deviceState.Lines[ i ], DeviceMonitor.PROPERTY_LINE_LASTCALLERNUMBER, "", deviceState.Lines[ i ].LastCallerNumber );
				EnsureNodeContains( lineNode, deviceState.Lines[ i ], DeviceMonitor.PROPERTY_LINE_MESSAGEWAITING, "", deviceState.Lines[ i ].MessageWaiting.ToString() );
				EnsureNodeContains( lineNode, deviceState.Lines[ i ], DeviceMonitor.PROPERTY_LINE_REGISTRATIONSTATE, "", deviceState.Lines[ i ].RegistrationState.ToString() );
				
				for( int j = 0; j < deviceState.Lines[ i ].Calls.Length; j++ ) {
					TreeNode callNode = lineNode.Nodes.Add( DeviceMonitor.PROPERTY_NAME, deviceState.Lines[ i ].Calls[ j ].Name );
					callNode.Tag = deviceState.Lines[ i ].Calls[ j ];
					
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_CALL_ACTIVITY, "", deviceState.Lines[ i ].Calls[ j ].Activity.ToString() );
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_CALL_BYTESRECEIVED, "", deviceState.Lines[ i ].Calls[ j ].BytesReceived.ToString() );
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_CALL_BYTESSENT, "", deviceState.Lines[ i ].Calls[ j ].BytesSent.ToString() );
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_CALL_DECODELATENCY, "", deviceState.Lines[ i ].Calls[ j ].DecodeLatency.ToString() );
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_CALL_DECODER, "", deviceState.Lines[ i ].Calls[ j ].Decoder );
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_CALL_DURATION, "", deviceState.Lines[ i ].Calls[ j ].Duration );
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_CALL_ENCODER, "", deviceState.Lines[ i ].Calls[ j ].Encoder );
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_CALL_JITTER, "", deviceState.Lines[ i ].Calls[ j ].Jitter.ToString() );
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_CALL_PACKETERROR, "", deviceState.Lines[ i ].Calls[ j ].PacketError.ToString() );
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_CALL_PACKETLOSS, "", deviceState.Lines[ i ].Calls[ j ].PacketLoss.ToString() );
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_CALL_ROUNDTRIPDELAY, "", deviceState.Lines[ i ].Calls[ j ].RoundTripDelay.ToString() );
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_CALL_TONE, "", deviceState.Lines[ i ].Calls[ j ].Tone.ToString() );
					EnsureNodeContains( callNode, deviceState.Lines[ i ].Calls[ j ], DeviceMonitor.PROPERTY_CALL_TYPE, "", deviceState.Lines[ i ].Calls[ j ].Type.ToString() );
				}
			}

		    deviceNode.ExpandAll();
		}

		public static String FindLatestVersion() {
			return new UTF8Encoding().GetString( new WebClient().DownloadData( UPDATE_URL ) );
		}

		public static bool CheckStartupRegKey() {
			// Check for existance of startup key.
			RegistryKey key = Registry.CurrentUser.OpenSubKey( STARTUP_REG_KEY, true );
			
			if( key == null || key.GetValue( Application.ProductName, null ) == null )
				return false;
			
			// If the key exists, but the path is wrong (ie. app has moved), correct it.
			if( ((String) key.GetValue( Application.ProductName, null )) != Application.ExecutablePath ) {
				Logger.Instance().Log( "Application is set to load on startup, but startup path is wrong. Correcting now." );
				key.SetValue( Application.ProductName, Application.ExecutablePath );
			}

			return true;
		}

		private void toolStripRunOnStartup_Click( object sender, EventArgs e ) {
			if( toolStripLoadAtStartup.Checked )
				Registry.CurrentUser.OpenSubKey( STARTUP_REG_KEY, true ).DeleteValue( Application.ProductName, false );
			else
				Registry.CurrentUser.OpenSubKey( STARTUP_REG_KEY, true ).SetValue( Application.ProductName, Application.ExecutablePath );					
			
			toolStripLoadAtStartup.Checked = ! toolStripLoadAtStartup.Checked;
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
			Process.Start( (String) e.Link.LinkData );
		}

		private void toolStripCallLog_Click(object sender, EventArgs e) {
			new FormCallRecords().Show();
		}

		private void toolStripQuit_Click( object sender, EventArgs e ) {            
            NotifyIcon.Visible = false;
			this.Hide();
			Program.SystemEvents_SessionEnding( this, null );
		}

		private void toolStripShowMain_Click( object sender, EventArgs e ) {
			ShowFormMain();
		}

		private void toolStripCheckForUpdates_Click( object sender, EventArgs e ) {
			String latestVersion;
			
			try {
				latestVersion = FindLatestVersion();
			} catch (WebException) {
				MessageBox.Show( this, "The update server cannot be contacted. Check that your machine is connected to the Internet, and consult the HVoIPM web-site if this problem persists.", "Update server could not be contacted" ); 
				return;
			}
			
			if( latestVersion != Application.ProductVersion ) {
				if( MessageBox.Show( this, "A version " + latestVersion + " of HVoIPM is now available for download.\n\nYou are running " + Application.ProductVersion + ". Do you want to go to the HVoIPM home to upgrade?", "Update available", MessageBoxButtons.YesNo ) == DialogResult.Yes )
					Process.Start( "http://www.lothianproductions.co.uk/hvoipm" );
			} else {
				MessageBox.Show( this, "You are running the latest version (" + Application.ProductVersion + ") of HVoIPM already.", "No update available", MessageBoxButtons.OK );
			}
			
		}
		
		private void FormMain_Resize( object sender, EventArgs e ) {
			TreeStates.Width = this.Size.Width - 32;
			TreeStates.Height = this.Size.Height - 138;
		}
		
		private void LabelUpdates_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e ) {
			Process.Start( (String) e.Link.LinkData );
		}
		#endregion
	
		  //Microsoft.Office.Interop.Outlook.ApplicationClass outLookApp = new Microsoft.Office.Interop.Outlook.ApplicationClass();
		  
		  ////outLookApp.

		  //  //Get Calender Item 
		  //  Microsoft.Office.Interop.Outlook.NameSpace outlookNS = outLookApp.GetNamespace("MAPI");
			
		  //  Microsoft.Office.Interop.Outlook.AddressLists addressLists = outlookNS.AddressLists;
			
		  //  foreach( Microsoft.Office.Interop.Outlook.AddressList addressList in addressLists ) {
		  //      MessageBox.Show( addressList.Name + ":" + addressList.ToString() );
		  //      try {
		  //      foreach( Microsoft.Office.Interop.Outlook.AddressEntries addressEntry in addressList.AddressEntries ) {
		  //          MessageBox.Show( "entry:" + addressEntry.Class + ":" + addressEntry.ToString() + ":" + addressEntry.RawTable );
		  //      }
		  //      } catch (System.Runtime.InteropServices.COMException) {}
		  //  }		
			
		//    MAPI.SessionClass oSession = new MAPI.SessionClass();
		//    oSession.Logon("OUTLOOK", System.Reflection.Missing.Value, true, true,
		//    System.Reflection.Missing.Value, false, System.Reflection.Missing.Value);

		//    MAPI.AddressList oAddressList = (MAPI.AddressList)
		//    oSession.GetAddressList(MAPI.CdoAddressListTypes.CdoAddressListGAL);
		//    Console.WriteLine(oAddressList.Name);
		//    MAPI.AddressEntries oAddressEntries = (MAPI.AddressEntries)oAddressList.AddressEntries;

		//    for(int j = 1; j <= (int)oAddressEntries.Count; j++) {
		//        MAPI.AddressEntry oAddressEntry = (MAPI.AddressEntry)oAddressEntries.get_Item(j);

		//        if(oAddressEntry.DisplayType.Equals(0) ||oAddressEntry.DisplayType.Equals(6))	{
		//            Console.WriteLine(" " + oAddressEntry.Name + " " + oAddressEntry.Fields);
		//        }
		//    }
	}
}