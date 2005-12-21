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
		
		protected int mIndex;
		public int Index {
			get{ return mIndex; }
		}
	}
	
	public class DeviceStateChange : StateChange {
		public DeviceStateChange( int index, String property, String changedFrom, String changedTo ) {
			mIndex = index;
			mProperty = property;
			mChangedFrom = changedFrom;
			mChangedTo = changedTo;
		}
	}

	public class LineStateChange : StateChange {
		public LineStateChange( int index, String property, String changedFrom, String changedTo ) {
			mIndex = index;
			mProperty = property;
			mChangedFrom = changedFrom;
			mChangedTo = changedTo;
		}
	}

	public class CallStateChange : StateChange {
		public CallStateChange( int index, String property, String changedFrom, String changedTo ) {
			mIndex = index;
			mProperty = property;
			mChangedFrom = changedFrom;
			mChangedTo = changedTo;
		}
	}

}
