using System;
using System.Collections.Generic;
using System.Text;

using LothianProductions.Util;

namespace LothianProductions.VoIP.State {
	public abstract class Change {
		protected String mProperty;
		public String Property {
			get{ return mProperty; }
		}
		
		protected String mChangedFrom;
		public String ChangedFrom {
			get{ return StringHelper.NoNull( mChangedFrom ); }
		}
		
		protected String mChangedTo;
		public String ChangedTo {
			get{ return StringHelper.NoNull( mChangedTo ); }
		}
		
		public abstract Object Underlying {
			get;
		}
	}
	
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
	}

	public class LineChange : Change {
		public LineChange( Line line, String property, String changedFrom, String changedTo ) {
			mLine = line;
			mProperty = property;
			mChangedFrom = changedFrom;
			mChangedTo = changedTo;
		}
		
		protected Line mLine;
		public Line Line {
			get{ return mLine; }
		}
		
		public override Object Underlying {
			get{ return mLine; }
		}
	}

	public class CallChange : Change {
		public CallChange( Call call, String property, long changedFrom, long changedTo ) {
			mCall = call;
			mProperty = property;
			mChangedFrom = changedFrom.ToString();
			mChangedTo = changedTo.ToString();			
		}
	
		public CallChange( Call call, String property, String changedFrom, String changedTo ) {
			mCall = call;
			mProperty = property;
			mChangedFrom = changedFrom;
			mChangedTo = changedTo;
		}
		
		protected Call mCall;
		public Call Call {
			get{ return mCall; }
		}
		
		public override Object Underlying {
			get{ return mCall; }
		}
	}
}
