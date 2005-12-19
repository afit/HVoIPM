using System;
using System.Collections.Generic;
using System.Text;

using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP.Monitor {
	public interface DeviceStateMonitor {
		void Run();
		DeviceState GetDeviceState();
		
		String Name {
			get;
		}
	}
}
