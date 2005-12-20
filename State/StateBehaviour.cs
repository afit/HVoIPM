using System;
using System.Collections.Generic;
using System.Text;

namespace LothianProductions.VoIP.State {
				// Options for objects of different types:
			// DeviceStateMonitor -- enabled
				// LineState -- enabled
					// LastCalledNumber -- Bubble alert on change
					//						Systray icon of variable type if not in range of values
					// LastCallerNumber -- Bubble alert on change
					//						Systray icon of variable type if not in range of values
					// RegistrationState -- Bubble alert on change
					//						Systray icon of variable type if not in range of values
					// CallState -- enabled
						// CallActivity -- Bubble alert on change
						//					Systray icon of variable type if not in range of values
						// Duration -- Bubble alert on change
						//					Systray icon of variable type if not in range of values
	
	public enum MonitorBehaviour {
		None,
		BubbleAlert,
		SystemTrayWarning,
		BubbleAlertAndSystemTrayWarning
	}
	
	public class DeviceStateMonitorBehaviour {
		protected bool mEnabled;
		public bool Enabled {
			get{ return mEnabled; }
			set{ mEnabled = value; }
		}
	}
	
	public class LineStateMonitorBehaviour {
		protected bool mEnabled;
		public bool Enabled {
			get{ return mEnabled; }
			set{ mEnabled = value; }
		}
		
		protected MonitorBehaviour mLastCalledNumberBehaviour;
		public MonitorBehaviour LastCalledNumberBehaviour {
			get{ return mLastCalledNumberBehaviour; }
			set{ mLastCalledNumberBehaviour = value; }
		}
		
		protected MonitorBehaviour mLastCallerNumberBehaviour;
		public MonitorBehaviour LastCallerNumberBehaviour {
			get{ return mLastCallerNumberBehaviour; }
			set{ mLastCallerNumberBehaviour = value; }
		}
		
		protected MonitorBehaviour mRegistrationStateBehaviour;
		public MonitorBehaviour RegistrationStateBehaviour {
			get{ return mRegistrationStateBehaviour; }
			set{ mRegistrationStateBehaviour = value; }
		}
	}
	
	public class CallStateMonitorBehaviour {
		protected bool mEnabled;
		public bool Enabled {
			get{ return mEnabled; }
			set{ mEnabled = value; }
		}
		
		protected MonitorBehaviour mCallActivityBehaviour;
		public MonitorBehaviour CallActivityBehaviour {
			get{ return mCallActivityBehaviour; }
			set{ mCallActivityBehaviour = value; }
		}
		
		protected MonitorBehaviour mDurationBehaviour;
		public MonitorBehaviour DurationBehaviour {
			get{ return mDurationBehaviour; }
			set{ mDurationBehaviour = value; }
		}
	}
}
