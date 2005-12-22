using System;
using System.Collections.Generic;
using System.Text;

namespace LothianProductions.VoIP.State {
	public class DeviceState {
		public	DeviceState( String name, LineState[] lineStates ) {
			mName = name;
			mLineStates = lineStates;
		}
		
		protected String mName;
        public String Name {
            get{ return mName; }
            set{ mName = value; }
        }
		
	    protected LineState[] mLineStates;
        public LineState[] LineStates {
            get{ return mLineStates; }
            set{ mLineStates = value; }
        }
	}
}
