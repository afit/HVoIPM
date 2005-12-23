using System;
using System.Collections.Generic;
using System.Text;

namespace LothianProductions.VoIP.State {

    public enum RegistrationState {
        Offline,
        Online,
        Error
    }

    public class LineState {
		public LineState( String name, CallState[] callStates ) {
			mName = name;
			CallStates = callStates;
		}
    
		protected String mName;
        public String Name {
            get{ return mName; }
            set{ mName = value; }
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

		protected bool mMessageWaiting;
        public bool MessageWaiting {
            get{ return mMessageWaiting; }
            set{ mMessageWaiting = value; }
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
