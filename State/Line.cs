using System;
using System.Collections.Generic;
using System.Text;

namespace LothianProductions.VoIP.State {

    public enum RegistrationState {
        Offline,
        Online,
        Error
    }

    public class Line : ICloneable {
		public Line( String name, Call[] calls ) {
			mName = name;
			mCalls = calls;
		}

		public Line(	String name, String lastCalledNumber, String lastCallerNumber, bool messageWaiting,
						Call[] calls, RegistrationState registrationState ) {
			mName = name;
			mLastCalledNumber = lastCalledNumber;
			mLastCallerNumber = lastCallerNumber;
			mMessageWaiting = messageWaiting;
			mCalls = calls;
			mRegistrationState = registrationState;
		}
		
		public Object Clone() {
			return new Line( mName, mLastCalledNumber, mLastCallerNumber, mMessageWaiting, (Call[]) mCalls.Clone(), mRegistrationState );
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
