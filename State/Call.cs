using System;
using System.Collections.Generic;
using System.Text;

namespace LothianProductions.VoIP.State {

    public enum Activity {
        IdleDisconnected,
        Connected,
        Dialing,
        Ringing,
        
        // FIXME remove these obsoleted calls
		[Obsolete("This needs to be implemented in a separate property.")]
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
		Error,
		InvalidNumber
    }

    public class Call : ICloneable {
		public Call( String name ) {
			mName = name;
		}
		
		public Call(	String name, Activity activity, CallType type, Tone tone,
						String duration, String encoder, String decoder, long bytesSent,
						long bytesReceived, long packetLoss, long packetError, long jitter,
						long decodeLatency, long roundTripDelay ) {
			mName = name;
			mActivity = activity;
			mType = type;
			mTone = tone;
			mDuration = duration;
			mEncoder = encoder;
			mDecoder = decoder;
			mBytesSent = bytesSent;
			mBytesReceived = bytesReceived;
			mPacketLoss = packetLoss;
			mPacketError = packetError;
			mJitter = jitter;
			mDecodeLatency = decodeLatency;
			mRoundTripDelay = roundTripDelay;
		}
		
		public Object Clone() {
			return new Call(  mName, mActivity, mType, mTone, mDuration, mEncoder, mDuration,
								mBytesSent, mBytesReceived, mPacketLoss, mPacketError, mJitter,
								mDecodeLatency, mRoundTripDelay );
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
