using System;
using System.Collections.Generic;
using System.Text;

using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP.Monitor {
	public interface DeviceMonitor {
		void Run();
		DeviceState GetDeviceState();
	}
}
