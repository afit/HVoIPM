using System;
using System.Collections.Generic;
using System.Text;

namespace LothianProductions.VoIP.State {
	public class Device : ICloneable {
		public Device( String name, Line[] lines ) {
			mName = name;
			mLines = lines;
		}
		
		public Object Clone() {
			return new Device( mName, (Line[]) mLines.Clone() );
		}
		
		protected String mName;
        public String Name {
            get{ return mName; }
            set{ mName = value; }
        }
		
	    protected Line[] mLines;
        public Line[] Lines {
            get{ return mLines; }
            set{ mLines = value; }
        }
	}
}
