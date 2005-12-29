using System;
using System.Collections.Generic;
using System.Text;
using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP {
    public class CallRecord {
        
        public CallRecord(	Device device, Line line, Call call,
							DateTime startTime, DateTime endTime ) {
			Device = device;
			Line = line;
			Call = call;			
			StartTime = startTime;
			EndTime = endTime;
		}

		protected Device mDevice;
        public Device Device {
            get{ return mDevice; }
            set{ mDevice = (Device) value.Clone(); }
        }

		protected Line mLine;
        public Line Line {
            get{ return mLine; }
            set{ mLine = (Line) value.Clone(); }
        }

		protected Call mCall;
        public Call Call {
            get{ return mCall; }
            set{ mCall = (Call) value.Clone(); }
        }

		protected DateTime mStartTime;
		public DateTime StartTime {
			get{ return  mStartTime; }
			set{ mStartTime = value; }
		}

		protected DateTime mEndTime;
		public DateTime EndTime {
			get{ return mEndTime; }
			set{ mEndTime = value; }
		}
	}
}