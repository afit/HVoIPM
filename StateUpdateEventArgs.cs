using System;
using System.Collections.Generic;
using System.Text;

using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP {
	public class StateUpdateEventArgs : EventArgs {
		public StateUpdateEventArgs(	IList<DevicePropertyChange> deviceChanges, IList<LinePropertyChange> lineChanges,
										IList<CallPropertyChange> callChanges ) {
			mDeviceStateChanges = deviceChanges;
			mLineStateChanges = lineChanges;
			mCallStateChanges = callChanges;
		}

		protected IList<DevicePropertyChange> mDeviceStateChanges;
		public IList<DevicePropertyChange> DeviceStateChanges {
			get{ return mDeviceStateChanges; }
		}

		protected IList<LinePropertyChange> mLineStateChanges;
		public IList<LinePropertyChange> LineStateChanges {
			get{ return mLineStateChanges; }
		}

		protected IList<CallPropertyChange> mCallStateChanges;
		public IList<CallPropertyChange> CallStateChanges {
			get{ return mCallStateChanges; }
		}
	}
}
