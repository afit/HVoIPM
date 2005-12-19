using System;
using System.Collections.Generic;
using System.Text;

namespace LothianProductions.VoIP.State {
	public interface IStateChange {
		String Property {
			get;
		}
		String ChangedFrom {
			get;
		}
		String ChangedTo {
			get;
		}
	}
	
	public interface IDeviceStateChange : IStateChange {
	}

	public interface ILineStateChange : IStateChange {
	}

	public interface ICallStateChange : IStateChange {
	}

}
