using System;
using System.Collections.Generic;
using System.Text;

using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP {
	public class StateUpdateEventArgs : EventArgs {
		public StateUpdateEventArgs(	IList<DeviceChange> deviceChanges, IList<LineChange> lineChanges,
										IList<CallChange> callChanges ) {
			mDeviceStateChanges = deviceChanges;
			mLineStateChanges = lineChanges;
			mCallStateChanges = callChanges;
		}

		protected IList<DeviceChange> mDeviceStateChanges;
		public IList<DeviceChange> DeviceStateChanges {
			get{ return mDeviceStateChanges; }
		}

		protected IList<LineChange> mLineStateChanges;
		public IList<LineChange> LineStateChanges {
			get{ return mLineStateChanges; }
		}

		protected IList<CallChange> mCallStateChanges;
		public IList<CallChange> CallStateChanges {
			get{ return mCallStateChanges; }
		}
	}
}
