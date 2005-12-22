using System;
using System.Collections.Generic;
using System.Text;

namespace LothianProductions.VoIP.State {

    public enum CallActivity {
        Idle,
        Outbound,
        Inbound,
        Held,
        Other
    }

    public class CallState {

		public CallState( String name ) {
			mName = name;
		}
    
		protected String mName;
        public String Name {
            get{ return mName; }
            set{ mName = value; }
        }
    
        protected CallActivity mCallActivity;
        public CallActivity CallActivity {
            get{ return mCallActivity; }
            set{ mCallActivity = value; }
        }

        protected String mDuration;
		public String Duration {
            get{ return mDuration; }
            set{ mDuration = value; }
        }
    }
}
