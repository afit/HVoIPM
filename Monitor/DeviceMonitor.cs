using System;
using System.Collections.Generic;
using System.Configuration;

using LothianProductions.VoIP.Monitor;
using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP.Monitor {

	public abstract class DeviceMonitor : IDeviceMonitor {
		public const String PROPERTY_NAME = "name";
		public const String PROPERTY_LINE_LASTCALLEDNUMBER = "lastCalledNumber";
		public const String PROPERTY_LINE_LASTCALLERNUMBER = "lastCallerNumber";
		public const String PROPERTY_LINE_REGISTRATIONSTATE = "registrationState";
		public const String PROPERTY_LINE_MESSAGEWAITING = "messageWaiting";
		public const String PROPERTY_CALL_ACTIVITY = "activity";
		public const String PROPERTY_CALL_DURATION = "duration";
		public const String PROPERTY_CALL_TYPE = "type";
		public const String PROPERTY_CALL_TONE = "tone";
		public const String PROPERTY_CALL_ENCODER = "encoder";
		public const String PROPERTY_CALL_DECODER = "decoder";
		public const String PROPERTY_CALL_BYTESSENT = "bytesSent";
		public const String PROPERTY_CALL_BYTESRECEIVED = "bytesReceived";
		public const String PROPERTY_CALL_PACKETLOSS = "packetLoss";
		public const String PROPERTY_CALL_PACKETERROR = "packetError";
		public const String PROPERTY_CALL_JITTER = "jitter";
		public const String PROPERTY_CALL_DECODELATENCY = "decodeLatency";
		public const String PROPERTY_CALL_ROUNDTRIPDELAY = "roundTripDelay";
	
		protected Device mDeviceState;

		protected String mName;
		public String Name {
			get{ return mName; }
		}
		
		public abstract void Run();

		public Device GetDeviceState() {
			return mDeviceState;
		}
		
		public String GetConfigurationValue( String key ) {
			return ConfigurationManager.AppSettings[ GetType().Name + ":" + key ];
		}
	}
}
