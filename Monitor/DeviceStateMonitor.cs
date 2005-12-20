using System;
using System.Collections.Generic;

using LothianProductions.VoIP.Monitor;
using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP.Monitor {

	public abstract class DeviceStateMonitor : IDeviceStateMonitor {
		protected DeviceState mDeviceState;

		protected String mName;
		public String Name {
			get{ return mName; }
		}
		
		public abstract void Run();

		public DeviceState GetDeviceState() {
			return mDeviceState;
		}
	}
}
