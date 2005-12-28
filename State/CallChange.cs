using System;
using System.Collections.Generic;
using System.Text;

namespace LothianProductions.VoIP.State {
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
