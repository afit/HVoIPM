using System;
using System.Collections.Generic;
using System.Text;

namespace LothianProductions.VoIP.State {

    public enum RegistrationState {
        Offline,
        Online,
        Other
    }

    public class LineState {
		public LineState( CallState[] callStates ) {
			CallStates = callStates;
		}
    
        protected String mLastCalledNumber;
        public String LastCalledNumber {
            get{ return mLastCalledNumber; }
            set{ mLastCalledNumber = value; }
        }

		protected String mLastCallerNumber;
        public String LastCallerNumber {
            get{ return mLastCallerNumber; }
            set{ mLastCallerNumber = value; }
        }

		protected CallState[] mCallStates;
		public CallState[] CallStates {
			get{ return mCallStates; }
			set{ mCallStates = value; }
		}

        protected RegistrationState mRegistrationState;
        public RegistrationState RegistrationState {
            get{ return mRegistrationState; }
            set{ mRegistrationState = value; }
        }
    }
}
