using System;
using System.Collections.Generic;
using System.Text;

using LothianProductions.Util;
using LothianProductions.VoIP.Monitor;

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
		
		public abstract IDeviceMonitor GetDeviceMonitor();
	}
}
