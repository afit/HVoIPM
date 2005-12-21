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

		//// FIXME remove this, it'll slow things down and is unused
		//public override string ToString() {
		//    StringBuilder builder = new StringBuilder();
			
		//    // FIXME: Not really threadsafe.
		//    for( int i = 0; i < LineStates.Length; i++ ) {
		//        builder.Append( "Line #" ).Append( i ).Append( ':' ).AppendLine();
		//        builder.Append( LineStates[ i ].ToString() );
		//    }
			
		//    return builder.ToString();
		//}
	}
}
