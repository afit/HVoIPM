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
				builder.Append( " - Last called number: " ).Append( LineStates[ i ].LastCalledNumber ).AppendLine();
				builder.Append( " - Last caller number: " ).Append( LineStates[ i ].LastCallerNumber ).AppendLine();
				builder.Append( " - Registration state: " ).Append( LineStates[ i ].RegistrationState ).AppendLine();
				
				for( int j = 0; j < LineStates[ i ].CallStates.Length; j++ ) {
					builder.Append( " - Call #" ).Append( j ).Append( ':' ).AppendLine();
					builder.Append( " -- Call activity: " ).Append( LineStates[ i ].CallStates[ j ].CallActivity ).AppendLine();
					builder.Append( " -- Call duration: " ).Append( LineStates[ i ].CallStates[ j ].Duration ).AppendLine();
				}
			}
			
			return builder.ToString();
		}
	}
}
