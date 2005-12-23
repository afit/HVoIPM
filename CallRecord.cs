using System;
using System.Collections.Generic;
using System.Text;
using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP
{
    public class CallRecord
    {
        protected Call mCallState;
        protected string mDevice;
        protected string mPhone;
        protected string mDirection;
        protected DateTime mStartTime;
        protected DateTime mEndTime;
        protected string mDuration;

        public CallRecord() {
        }

        public CallRecord( string device, Call cs, string direction, string phone, DateTime startTime, DateTime endTime, string duration ) {
            mCallState = cs;
            mDevice = device;
            mDirection = direction;
            mPhone = phone;
            mStartTime = startTime;
            mEndTime = endTime;
            mDuration = duration;
        }

        public Call CallState {
            get {
                return mCallState;
            }
            set {
                mCallState = value;
            }
        }

        public string Device {
            get {
                return mDevice;
            }
            set {
                mDevice = value;
            }
        }

        public string Direction {
            get {
                return mDirection;
            }
            set {
                mDirection = value;
            }
        }

        public string Phone {
            get {
                return mPhone;
            }
            set {
                mPhone = value;
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

        public string Duration {
            get {
                return mDuration;
            }
            set {
                mDuration = value;
            }
        }
    }
}
