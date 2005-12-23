using System;
using System.Collections.Generic;
using System.Text;

namespace LothianProductions.VoIP.State {

    public enum RegistrationState {
        Offline,
        Online,
        Error
    }

    public class Line {
		public Line( String name, Call[] calls ) {
			mName = name;
			Calls = calls;
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

		protected Call[] mCalls;
		public Call[] Calls {
			get{ return mCalls; }
			set{ mCalls = value; }
		}

        protected RegistrationState mRegistrationState;
        public RegistrationState RegistrationState {
            get{ return mRegistrationState; }
            set{ mRegistrationState = value; }
        }
    }
}
