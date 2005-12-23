using System;
using System.Collections.Generic;

using LothianProductions.VoIP.Monitor;
using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP.Monitor {

	public abstract class DeviceStateMonitor : IDeviceStateMonitor {
		public const String PROPERTY_LASTCALLEDNUMBER = "lastCalledNumber";
		public const String PROPERTY_LASTCALLERNUMBER = "lastCallerNumber";
		public const String PROPERTY_REGISTRATIONSTATE = "registrationState";
		public const String PROPERTY_MESSAGEWAITING = "messageWaiting";
		public const String PROPERTY_ACTIVITY = "activity";
		public const String PROPERTY_DURATION = "duration";
		public const String PROPERTY_TYPE = "type";
		public const String PROPERTY_TONE = "tone";
		public const String PROPERTY_ENCODER = "encoder";
		public const String PROPERTY_DECODER = "decoder";
		public const String PROPERTY_BYTESSENT = "bytesSent";
		public const String PROPERTY_BYTESRECEIVED = "bytesReceived";
		public const String PROPERTY_PACKETLOSS = "packetLoss";
		public const String PROPERTY_PACKETERROR = "packetError";
		public const String PROPERTY_JITTER = "jitter";
		public const String PROPERTY_DECODELATENCY = "decodeLatency";
		public const String PROPERTY_ROUNDTRIPDELAY = "roundTripDelay";
	
		protected Device mDeviceState;

		protected String mName;
		public String Name {
			get{ return mName; }
		}
		
		public abstract void Run();

		public Device GetDeviceState() {
			return mDeviceState;
		}
	}
}
