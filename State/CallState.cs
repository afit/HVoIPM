using System;
using System.Collections.Generic;
using System.Text;

namespace LothianProductions.VoIP.State {

    public enum Activity {
        IdleDisconnected,
        Connected,
        Dialing,
        Ringing,
        
        // FIXME lose or reasses those below:
        // I think they're all replaced now by combinations
        // of properties. "Held" is possibly part of a new property.
        InboundRinging,
        OutboundRinging,
        Outbound,
        Inbound,
        Busy,
        Held,
        Error
    }

    public enum CallType {
        IdleDisconnected,
        Outbound,
        Inbound,
        Error
    }
    
    public enum Tone {
		None,
		Dial,
		SecureCall,
		Call,
		Busy,
		Error
    }

    public class CallState {

		public CallState( String name ) {
			mName = name;
		}
    
		protected String mName;
        public String Name {
            get{ return mName; }
            set{ mName = value; }
        }
    
        protected Activity mActivity;
        public Activity Activity {
            get{ return mActivity; }
            set{ mActivity = value; }
        }

        protected CallType mType;
        public CallType Type {
            get{ return mType; }
            set{ mType = value; }
        }

        protected Tone mTone;
        public Tone Tone {
            get{ return mTone; }
            set{ mTone = value; }
        }

        protected String mDuration;
		public String Duration {
            get{ return mDuration; }
            set{ mDuration = value; }
        }
        
        protected String mEncoder;
		public String Encoder {
            get{ return mEncoder; }
            set{ mEncoder = value; }
        }

        protected String mDecoder;
		public String Decoder {
            get{ return mDecoder; }
            set{ mDecoder = value; }
        }

		protected long mBytesSent;
		public long BytesSent {
            get{ return mBytesSent; }
            set{ mBytesSent = value; }
        }

		protected long mBytesReceived;
		public long BytesReceived {
            get{ return mBytesReceived; }
            set{ mBytesReceived = value; }
        }

        protected long mPacketLoss;
		public long PacketLoss {
            get{ return mPacketLoss; }
            set{ mPacketLoss = value; }
        }

        protected long mPacketError;
		public long PacketError {
            get{ return mPacketError; }
            set{ mPacketError = value; }
        }
        
        protected long mJitter;
		public long Jitter {
            get{ return mJitter; }
            set{ mJitter = value; }
        }
        
        protected long mDecodeLatency;
		public long DecodeLatency {
            get{ return mDecodeLatency; }
            set{ mDecodeLatency = value; }
        }
        
        protected long mRoundTripDelay;
		public long RoundTripDelay {
            get{ return mRoundTripDelay; }
            set{ mRoundTripDelay = value; }
        }      
    }
}
