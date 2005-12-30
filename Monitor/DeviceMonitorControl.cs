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
			try {
				mDeviceMonitor.Run();
			} catch (DeviceAccessUnauthorizedException e) {
				MessageBox.Show( "The " + mDeviceMonitor.GetType().Name + " reported an authorisation error: " + e.Message + "\n\nMaybe the application's configuration is incorrect: consult \"Hardware VoIP Monitor.config\".\n\nHVoIPM will stop polling this monitor. If this failure is unexpected you should send your system.xml log file to hvoip@lothianproductions.co.uk for debugging.", "Device monitor error" );
				Logger.Instance().Log( "The " + mDeviceMonitor.GetType().Name + " reported an authorisation error", e );
			} catch (DeviceNotRespondingException e) {
				MessageBox.Show( "The " + mDeviceMonitor.GetType().Name + " reported that its device failed to respond: " + e.Message + "\n\nMaybe the application's configuration is incorrect: consult \"Hardware VoIP Monitor.config\".\n\nHVoIPM will stop polling this monitor. If this failure is unexpected you should send your system.xml log file to hvoip@lothianproductions.co.uk for debugging.", "Device monitor error" );
				Logger.Instance().Log( "The " + mDeviceMonitor.GetType().Name + " reported that its device failed to respond", e );
			} catch (DeviceException e) {
				MessageBox.Show( "The " + mDeviceMonitor.GetType().Name + " reported a general error: " + e.Message + "\n\nHVoIPM will stop polling this monitor. If this failure is unexpected you should send your system.xml log file to hvoip@lothianproductions.co.uk for debugging.", "Device monitor error" );
				Logger.Instance().Log( "The " + mDeviceMonitor.GetType().Name + " reported a general error", e );
			} catch (ApplicationException e) {
				MessageBox.Show( "The " + mDeviceMonitor.GetType().Name + " reported a fatal error: " + e.Message + "\n\nHVoIPM will stop polling this monitor. If this failure is unexpected you should send your system.xml log file to hvoip@lothianproductions.co.uk for debugging.", "Device monitor error" );
				Logger.Instance().Log( "The " + mDeviceMonitor.GetType().Name + " reported a fatal error", e );
			}
		}
		
		protected IDeviceMonitor mDeviceMonitor;
		public IDeviceMonitor DeviceMonitor {
			get{ return mDeviceMonitor; }
			set{ mDeviceMonitor = value; }
		}
	}
}
