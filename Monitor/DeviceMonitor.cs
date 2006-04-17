/// ***************************************************************************
/// This file and its related files are protected by copyright and intellectual
/// property disclaimers. Use of it by you or your organisation must be covered
/// by prior agreement, and may involve NDA and other restriction. Duplication,
/// distribution or disclosure of any part of this file are likely prohibited,
/// and violations of the licensing terms will be pursued by law.
/// ***************************************************************************
/// Further details of the stock LothianProductionsCommon usage agreement are
/// available at: http://www.lothianproductions.co.uk/lpc/#license
/// ***************************************************************************
using System;
using System.Collections.Generic;
using System.Configuration;

using LothianProductions.VoIP.Monitor;
using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP.Monitor {

	/// <summary>
	/// Defines structure of all device monitors. These connect to monitored
	/// devices in whichever fashion they require, and bring back information
	/// on them.
	/// 
	/// The device monitor implementation must return Device data from the
	/// GetDeviceState method. This provides a simple representation
	/// of the device and its state.
	/// 
	/// The AbstractWebDeviceMonitor extends this and provides another mechanism
	/// for flexible state representation beyond the core fields.
	/// </summary>
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
