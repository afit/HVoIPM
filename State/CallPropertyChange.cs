using System;
using System.Collections.Generic;
using System.Text;

using LothianProductions.VoIP.Monitor;

namespace LothianProductions.VoIP.State {
	public class CallPropertyChange : PropertyChange {
		public CallPropertyChange( Call call, String property, long changedFrom, long changedTo ) {
			mCall = call;
			mProperty = property;
			mChangedFrom = changedFrom.ToString();
			mChangedTo = changedTo.ToString();			
		}
	
		public CallPropertyChange( Call call, String property, String changedFrom, String changedTo ) {
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

		public override IDeviceMonitor GetDeviceMonitor() {
			return StateManager.Instance().GetMonitor( mCall );
		}
	}
}
