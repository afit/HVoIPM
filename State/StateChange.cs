using System;
using System.Collections.Generic;
using System.Text;

namespace LothianProductions.VoIP.State {
	public abstract class StateChange {
		protected String mProperty;
		public String Property {
			get{ return mProperty; }
		}
		
		protected String mChangedFrom;
		public String ChangedFrom {
			get{ return mChangedFrom; }
		}
		
		protected String mChangedTo;
		public String ChangedTo {
			get{ return mChangedTo; }
		}
		
		public abstract Object Underlying {
			get;
		}
	}
	
	public class DeviceStateChange : StateChange {
		public DeviceStateChange( DeviceState deviceState, String property, String changedFrom, String changedTo ) {
			mDeviceState = deviceState;
			mProperty = property;
			mChangedFrom = changedFrom;
			mChangedTo = changedTo;
		}
		
		protected DeviceState mDeviceState;
		public DeviceState DeviceState {
			get{ return mDeviceState; }
		}
		
		public override Object Underlying {
			get{ return mDeviceState; }
		}
	}

	public class LineStateChange : StateChange {
		public LineStateChange( LineState lineState, String property, String changedFrom, String changedTo ) {
			mLineState = lineState;
			mProperty = property;
			mChangedFrom = changedFrom;
			mChangedTo = changedTo;
		}
		
		protected LineState mLineState;
		public LineState LineState {
			get{ return mLineState; }
		}
		
		public override Object Underlying {
			get{ return mLineState; }
		}
	}

	public class CallStateChange : StateChange {
		public CallStateChange( CallState callState, String property, String changedFrom, String changedTo ) {
			mCallState = callState;
			mProperty = property;
			mChangedFrom = changedFrom;
			mChangedTo = changedTo;
		}
		
		protected CallState mCallState;
		public CallState CallState {
			get{ return mCallState; }
		}
		
		public override Object Underlying {
			get{ return mCallState; }
		}
	}
}
