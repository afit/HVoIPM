using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace LothianProductions.VoIP.Monitor {
	public class DeviceMonitorControl {
		
		public DeviceMonitorControl( IDeviceMonitor monitor ) {
			mDeviceMonitor = monitor;
		}
		
		public void Run() {
			// Wrap in a big while to prevent the run method returning.
			while( true ) {
				try {
					mDeviceMonitor.Run();
				} catch (DeviceAccessUnauthorizedException e) {
					Logger.Instance().Log( "The " + mDeviceMonitor.GetType().Name + " reported an authorisation error", e );
					if( MessageBox.Show(
						"The " + mDeviceMonitor.GetType().Name + " reported an authorisation error: " + e.Message +
						"\n\nMaybe the application's configuration is incorrect: consult \"Hardware VoIP Monitor.config\"." +
						"\n\nIf this failure is unexpected you should check for updates before sending your system.xml log file to hvoipm@lothianproductions.co.uk for debugging." +
						"\n\nShould HVoIPM continue polling this device monitor?",
						 "HVoIPM device monitor error", MessageBoxButtons.YesNo, MessageBoxIcon.Error ) == DialogResult.No )
						return;					
				} catch (DeviceNotRespondingException e) {
					Logger.Instance().Log( "The " + mDeviceMonitor.GetType().Name + " reported that its device failed to respond", e );
					if( MessageBox.Show(
						"The " + mDeviceMonitor.GetType().Name + " reported that its device failed to respond: " + e.Message +
						"\n\nMaybe the application's configuration is incorrect: consult \"Hardware VoIP Monitor.config\"." +
						"\n\nIf this failure is unexpected you should check for updates before sending your system.xml log file to hvoipm@lothianproductions.co.uk for debugging." +
						"\n\nShould HVoIPM continue polling this device monitor?",
						 "HVoIPM device monitor error", MessageBoxButtons.YesNo, MessageBoxIcon.Error ) == DialogResult.No )
						return;
					
				} catch (DeviceException e) {
					Logger.Instance().Log( "The " + mDeviceMonitor.GetType().Name + " reported a general error", e );
					if( MessageBox.Show(
						"The " + mDeviceMonitor.GetType().Name + " reported a general error: " + e.Message +
						"\n\nIf this failure is unexpected you should check for updates before sending your system.xml log file to hvoipm@lothianproductions.co.uk for debugging." +
						"\n\nShould HVoIPM continue polling this device monitor?",
						 "HVoIPM device monitor error", MessageBoxButtons.YesNo, MessageBoxIcon.Error ) == DialogResult.No )
						return;
				} catch (ApplicationException e) {
					Logger.Instance().Log( "The " + mDeviceMonitor.GetType().Name + " reported a fatal error", e );
					if( MessageBox.Show(
						"The " + mDeviceMonitor.GetType().Name + " reported a fatal error: " + e.Message +
						"\n\nIf this failure is unexpected you should check for updates before sending your system.xml log file to hvoipm@lothianproductions.co.uk for debugging." +
						"\n\nShould HVoIPM continue polling this device monitor?",
						"HVoIPM device monitor error",
						MessageBoxButtons.YesNo, MessageBoxIcon.Error ) == DialogResult.No )
						return;
				}
			}
		}
		
		protected IDeviceMonitor mDeviceMonitor;
		public IDeviceMonitor DeviceMonitor {
			get{ return mDeviceMonitor; }
			set{ mDeviceMonitor = value; }
		}
	}
}
