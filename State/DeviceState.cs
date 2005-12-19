using System;
using System.Collections.Generic;
using System.Text;

namespace LothianProductions.VoIP.State {
	public class DeviceState {
		public	DeviceState( LineState[] lineStates ) {
			LineStates = lineStates;
		}
		
	    protected LineState[] mLineStates;
        public LineState[] LineStates {
            get{ return mLineStates; }
            set{ mLineStates = value; }
        }

		public override string ToString() {
			StringBuilder builder = new StringBuilder();
			
			// FIXME: Not really threadsafe.
			for( int i = 0; i < LineStates.Length; i++ ) {
				builder.Append( "Line #" ).Append( i ).Append( ':' ).AppendLine();
				builder.Append( LineStates[ i ].ToString() );
			}
			
			return builder.ToString();
		}
	}
}
