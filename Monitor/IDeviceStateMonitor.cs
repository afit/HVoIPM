using System;
using System.Collections.Generic;
using System.Text;

using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP.Monitor {
	public interface IDeviceStateMonitor {
		void Run();
		DeviceState GetDeviceState();
		
		String Name {
			get;
		}
	}

    public class DeviceNotRespondingException : ApplicationException
    {
        public DeviceNotRespondingException()
            : base("A device is not responding to a status request") {
        }

        public DeviceNotRespondingException(string strMessage)
            : base(strMessage) {
        }
    }
}
