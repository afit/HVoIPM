using System;
using System.Collections.Generic;
using System.Text;

using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP.Monitor {
	public interface IDeviceMonitor {
		void Run();
		Device GetDeviceState();
		
		String Name {
			get;
		}
	}
}
