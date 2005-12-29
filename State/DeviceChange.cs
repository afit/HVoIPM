using System;
using System.Collections.Generic;
using System.Text;

using LothianProductions.VoIP.Monitor;

namespace LothianProductions.VoIP.State {
	public class DeviceChange : Change {
		public DeviceChange( Device device, String property, String changedFrom, String changedTo ) {
			mDevice = device;
			mProperty = property;
			mChangedFrom = changedFrom;
			mChangedTo = changedTo;
		}
		
		protected Device mDevice;
		public Device Device {
			get{ return mDevice; }
		}
		
		public override Object Underlying {
			get{ return mDevice; }
		}
		
		public override IDeviceMonitor GetDeviceMonitor() {
			return StateManager.Instance().GetMonitor( mDevice );
		}
	}
}
