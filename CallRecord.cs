using System;
using System.Collections.Generic;
using System.Text;
using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP
{
    public class CallRecord
    {
        protected Call mCall;
		protected Line mLine;
		protected Device mDevice;
		protected DateTime mStartTime;
		protected DateTime mEndTime;

        public CallRecord() {
        }

        public CallRecord( Device device, Line line, Call call, DateTime startTime, DateTime endTime ) {
			mCall = (Call)call.Clone();
			mLine = (Line)line.Clone();
			mDevice = (Device)device.Clone();
			mStartTime = startTime;
			mEndTime = endTime;
		}

        public Call Call {
            get {
                return mCall;
            }
            set {
                mCall = (Call)value.Clone();
            }
        }

        public Device Device {
            get {
                return mDevice;
            }
            set {
                mDevice = (Device)value.Clone();
            }
        }

        public Line Line {
            get {
                return mLine;
            }
            set {
                mLine = (Line)value.Clone();
            }
        }

		public DateTime StartTime {
			get {
				return mStartTime;
			}
			set {
				mStartTime = value;
			}
		}

		public DateTime EndTime {
			get {
				return mEndTime;
			}
			set {
				mEndTime = value;
			}
		}
	}
}
