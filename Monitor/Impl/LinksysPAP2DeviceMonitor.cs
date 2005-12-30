using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text;
using System.Threading;

using LothianProductions.Util;
using LothianProductions.Util.Http;
using LothianProductions.VoIP.Monitor;
using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP.Monitor.Impl {

	/// <summary>
	/// Implementation of DeviceMonitor for the Linksys PAP2
	/// VoIP adapter device. As there's no neat way to get into
	/// the thing, this implementation works just by HTML scraping.
	/// 
	/// This implementation is really just a template, and it's not
	/// at all efficient. There's a heck of a lot of String creation
	/// & disposal that's highly unecessary.
	/// </summary>
    public class LinksysPAP2DeviceMonitor : DeviceMonitor {

		public LinksysPAP2DeviceMonitor( String name ) {
			mName = name;
			
			// We know there will be two lines for the device, each with
			// two calls.
			mDeviceState = new Device( name, new Line[] {
				new Line( "Line 1", new Call[] { new Call( "Call 1" ), new Call( "Call 2" ) } ),
				new Line( "Line 2", new Call[] { new Call( "Call 1" ), new Call( "Call 2" ) } )
			} );
		}
	    
        public override void Run() {
			int sleepTime = Int32.Parse( GetConfigurationValue( "PollInterval" ) );
			String hostname = GetConfigurationValue( "Hostname" );
			
			WebClient webClient = new WebClient();
			webClient.Headers.Add( "User-Agent", "HVoIPM / LinksysPAP2DeviceStateMonitor" );
			
			if( GetConfigurationValue( "Username" ) != "" || GetConfigurationValue( "Password" ) != "" )
				webClient.Credentials = new NetworkCredential( GetConfigurationValue( "Username" ), GetConfigurationValue( "Password" ) );
			
			while( true ) {
				// Lock to prevent recursion synchronicity problems
				// caused by slow wgetting.
				lock (this) {
					String page;
					try {
						page = new UTF8Encoding().GetString( webClient.DownloadData( hostname ) );
					} catch (WebException e) {
					    if( e.Status == WebExceptionStatus.ProtocolError )
							if( ( (HttpWebResponse) e.Response ).StatusCode == HttpStatusCode.Unauthorized )
					            throw new DeviceAccessUnauthorizedException( "Not authorized to request \"" + hostname + "\"; perhaps username and password are incorrect?", e );
					            
					    throw new DeviceNotRespondingException( "The device \"" + hostname + "\" is not responding to status requests", e );
					}
					
					IList<DevicePropertyChange> deviceChanges = new List<DevicePropertyChange>();
					IList<LinePropertyChange> lineChanges = new List<LinePropertyChange>();
					IList<CallPropertyChange> callChanges = new List<CallPropertyChange>();
				
					AnalyseLineState( page, mDeviceState.Lines[ 0 ], lineChanges, callChanges );	
					AnalyseLineState( page, mDeviceState.Lines[ 1 ], lineChanges, callChanges );
				
					if( deviceChanges.Count > 0 || lineChanges.Count > 0 || callChanges.Count > 0 )
						StateManager.Instance().DeviceStateUpdated( this, deviceChanges, lineChanges, callChanges );
				}			
				
				Thread.Sleep( sleepTime );
			}
        }
        
        protected virtual void AnalyseLineState( String page, Line lineState, IList<LinePropertyChange> lineChanges, IList<CallPropertyChange> callChanges ) {

			String lastCalledNumber = StringHelper.EmptyToNull( StringHelper.ExtractSubstring( page, "Last Called Number:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ) );
			if( lastCalledNumber != lineState.LastCalledNumber )
				lineChanges.Add( new LinePropertyChange( lineState, PROPERTY_LINE_LASTCALLEDNUMBER, lineState.LastCalledNumber, lastCalledNumber ) );
			lineState.LastCalledNumber = lastCalledNumber;
				
			String lastCallerNumber = StringHelper.EmptyToNull( StringHelper.ExtractSubstring( page, "Last Caller Number:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ) );
			if( lastCallerNumber != lineState.LastCallerNumber )
				lineChanges.Add( new LinePropertyChange( lineState, PROPERTY_LINE_LASTCALLERNUMBER, lineState.LastCallerNumber, lastCallerNumber ) );
			lineState.LastCallerNumber = lastCallerNumber;

			RegistrationState registrationState = GetRegistrationState( StringHelper.ExtractSubstring( page, "Registration State:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ) );
			if( registrationState != lineState.RegistrationState )
				lineChanges.Add( new LinePropertyChange( lineState, PROPERTY_LINE_REGISTRATIONSTATE, lineState.RegistrationState.ToString(), registrationState.ToString() ) );
			lineState.RegistrationState = registrationState;

			bool messageWaiting = StringHelper.ExtractSubstring( page, "Message Waiting:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ) != "No";
			if( messageWaiting != lineState.MessageWaiting )
				lineChanges.Add( new LinePropertyChange( lineState, PROPERTY_LINE_MESSAGEWAITING, lineState.MessageWaiting.ToString(), messageWaiting.ToString() ) );
			lineState.MessageWaiting = messageWaiting;

			// Analyse both calls for the line. (We know this device only
			// supports two calls per line.)
			AnalyseCallState( page, lineState.Calls[ 0 ], lineState, callChanges );
			AnalyseCallState( page, lineState.Calls[ 1 ], lineState, callChanges );
        }
        
        protected virtual void AnalyseCallState( String page, Call callState, Line lineState, IList<CallPropertyChange> callChanges ) {
			Activity callActivity = GetActivity( StringHelper.ExtractSubstring( page, callState.Name + " State:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ) );
			if( callActivity != callState.Activity )
				callChanges.Add( new CallPropertyChange( callState, PROPERTY_CALL_ACTIVITY, callState.Activity.ToString(), callActivity.ToString() ) );
			callState.Activity = callActivity;
			
			String duration = StringHelper.EmptyToNull( StringHelper.ExtractSubstring( page, callState.Name + " Duration:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ) );
			if( duration != callState.Duration )
				callChanges.Add( new CallPropertyChange( callState, PROPERTY_CALL_DURATION, callState.Duration, duration ) );
			callState.Duration = duration;

			CallType type = GetCallType( StringHelper.ExtractSubstring( page, callState.Name + " Type:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ) );
			if( type != callState.Type )
				callChanges.Add( new CallPropertyChange( callState, PROPERTY_CALL_TYPE, callState.Type.ToString(), type.ToString() ) );
			callState.Type = type;

			Tone tone = GetTone( StringHelper.ExtractSubstring( page, callState.Name + " Tone:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ) );
			if( tone != callState.Tone )
				callChanges.Add( new CallPropertyChange( callState, PROPERTY_CALL_TONE, callState.Tone.ToString(), tone.ToString() ) );
			callState.Tone = tone;
			
			String encoder = StringHelper.EmptyToNull( StringHelper.ExtractSubstring( page, callState.Name + " Encoder:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ) );
			if( encoder != callState.Encoder )
				callChanges.Add( new CallPropertyChange( callState, PROPERTY_CALL_ENCODER, callState.Encoder, encoder ) );
			callState.Encoder = encoder;
			
			String decoder = StringHelper.EmptyToNull( StringHelper.ExtractSubstring( page, callState.Name + " Decoder:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ) );
			if( decoder != callState.Decoder )
				callChanges.Add( new CallPropertyChange( callState, PROPERTY_CALL_DECODER, callState.Decoder, decoder ) );
			callState.Decoder = decoder;
			
			long bytesSent = ParseInt64( StringHelper.ExtractSubstring( page, callState.Name + " Bytes Sent:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ) );
			if( bytesSent != callState.BytesSent )
				callChanges.Add( new CallPropertyChange( callState, PROPERTY_CALL_BYTESSENT, callState.BytesSent, bytesSent ) );
			callState.BytesSent = bytesSent;

			long bytesReceived = ParseInt64( StringHelper.ExtractSubstring( page, callState.Name + " Bytes Recv:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ) );
			if( bytesReceived != callState.BytesReceived )
				callChanges.Add( new CallPropertyChange( callState, PROPERTY_CALL_BYTESRECEIVED, callState.BytesReceived, bytesReceived ) );
			callState.BytesReceived = bytesReceived;
			
			long packetLoss = ParseInt64( StringHelper.ExtractSubstring( page, callState.Name + " Packets Lost:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ) );
			if( packetLoss != callState.PacketLoss )
				callChanges.Add( new CallPropertyChange( callState, PROPERTY_CALL_PACKETLOSS, callState.PacketLoss, packetLoss ) );
			callState.PacketLoss = packetLoss;
			
			long packetError = ParseInt64( StringHelper.ExtractSubstring( page, callState.Name + " Packet Error:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ) );
			if( packetError != callState.PacketError )
				callChanges.Add( new CallPropertyChange( callState, PROPERTY_CALL_PACKETERROR, callState.PacketError, packetError ) );
			callState.PacketError = packetError;
			
			long jitter = ParseInt64( StringHelper.ExtractSubstring( page, callState.Name + " Jitter:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ).Replace( " ms", "") );
			if( jitter != callState.Jitter )
				callChanges.Add( new CallPropertyChange( callState, PROPERTY_CALL_JITTER, callState.Jitter, jitter ) );
			callState.Jitter = jitter;
			
			long decodeLatency = ParseInt64( StringHelper.ExtractSubstring( page, callState.Name + " Decode Latency:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ).Replace( " ms", "") );
			if( decodeLatency != callState.DecodeLatency )
				callChanges.Add( new CallPropertyChange( callState, PROPERTY_CALL_DECODELATENCY, callState.DecodeLatency, decodeLatency ) );
			callState.DecodeLatency = decodeLatency;
			
			long roundTripDelay = ParseInt64( StringHelper.ExtractSubstring( page, callState.Name + " Round Trip Delay:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ).Replace( " ms", "") );
			if( roundTripDelay != callState.RoundTripDelay )
				callChanges.Add( new CallPropertyChange( callState, PROPERTY_CALL_ROUNDTRIPDELAY, callState.RoundTripDelay, roundTripDelay ) );
			callState.RoundTripDelay = roundTripDelay;
        }
        
        public static long ParseInt64( String parse ) {
			long value = 0L;
			
			try {
				if( parse != null && parse != "" )
					value = Int64.Parse( parse );
			} catch (FormatException) {
				// Safe to do nothing.
			}
			
			return value;
        }

		public virtual CallType GetCallType( String type ) {
			String mapping = ConfigurationManager.AppSettings[ this.GetType().Name + ":CallType:" + type ];
			
			if( mapping != null )
				try {
					return (CallType) Enum.Parse( typeof(CallType), mapping );
				} catch (ArgumentException) {
				}
				
			throw new DeviceConfigurationException( "CallType \"" + type + "\" incorrectly mapped to \"" + mapping + "\". Perhaps a configuration entry is missing?" );			
		}
		
		public virtual Tone GetTone( String tone ) {
			String mapping = ConfigurationManager.AppSettings[ this.GetType().Name + ":Tone:" + tone ];

			if( mapping != null )
				try {
					return (Tone) Enum.Parse( typeof(Tone), mapping );
				} catch (ArgumentException) {
				}
			
			throw new DeviceConfigurationException( "Tone \"" + tone + "\" incorrectly mapped to \"" + mapping + "\". Perhaps a configuration entry is missing?" );			
		}

		public virtual Activity GetActivity( String activity ) {
			String mapping = ConfigurationManager.AppSettings[ this.GetType().Name + ":Activity:" + activity ];
			if( mapping != null )
				try {
					return (Activity) Enum.Parse( typeof(Activity), mapping );
				} catch (ArgumentException) {
				}
			
			throw new DeviceConfigurationException( "Activity \"" + activity + "\" incorrectly mapped to \"" + mapping + "\". Perhaps a configuration entry is missing?" );			
		}
        
        public virtual RegistrationState GetRegistrationState( String state ) {
			String mapping = ConfigurationManager.AppSettings[ this.GetType().Name + ":RegistrationState:" + state ];
			if( mapping != null )
				try {
					return (RegistrationState) Enum.Parse( typeof(RegistrationState), mapping );
				} catch (ArgumentException) {
				}			
			
			throw new DeviceConfigurationException( "Registration state \"" + state + "\" incorrectly mapped to \"" + mapping + "\". Perhaps a configuration entry is missing?" );			
        }
    }
}
