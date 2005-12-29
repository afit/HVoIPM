using System;
using System.Collections.Generic;
using System.Text;

using LothianProductions.VoIP.Monitor;

namespace LothianProductions.VoIP.State {
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
		
		public override IDeviceMonitor GetDeviceMonitor() {
			return StateManager.Instance().GetMonitor( mLine );
		}
	}
}
