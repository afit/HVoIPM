using System;
using System.Collections.Generic;

using LothianProductions.VoIP.Monitor;
using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP.Monitor {

	public abstract class DeviceStateMonitor : IDeviceStateMonitor {
		protected Device mDeviceState;

		protected String mName;
		public String Name {
			get{ return mName; }
		}
		
		public abstract void Run();

		public Device GetDeviceState() {
			return mDeviceState;
		}
	}
}
