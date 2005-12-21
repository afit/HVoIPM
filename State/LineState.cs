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

		//public override string ToString() {
		//    StringBuilder builder = new StringBuilder();
		//    builder.Append( " - Last called number: " ).Append( LastCalledNumber ).AppendLine();
		//    builder.Append( " - Last caller number: " ).Append( LastCallerNumber ).AppendLine();
		//    builder.Append( " - Registration state: " ).Append( RegistrationState ).AppendLine();

		//    for( int i = 0; i < CallStates.Length; i++ ) {
		//        builder.Append( " - Call #" ).Append( i ).Append( ':' ).AppendLine();
		//        builder.Append( CallStates[ i ].ToString() );
		//    }
			
		//    return builder.ToString();
		//}
    }
}
