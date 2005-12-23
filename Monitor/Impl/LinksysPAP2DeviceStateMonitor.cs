using System;
using System.Collections.Generic;
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
	/// </summary>
    public class LinksysPAP2DeviceStateMonitor : DeviceStateMonitor {

		public LinksysPAP2DeviceStateMonitor( String name ) {
			mName = name;
			
			// We know there will be two lines for the device, each with
			// two calls.
			mDeviceState = new DeviceState( name, new LineState[] {
				new LineState( "Line 1", new CallState[] { new CallState( "Call 1" ), new CallState( "Call 2" ) } ),
				new LineState( "Line 2", new CallState[] { new CallState( "Call 1" ), new CallState( "Call 2" ) } )
			} );
		}
	    
        public override void Run() {
			while( true ) {
				// Lock to prevent recursion synchronicity problems
				// caused by slow wgetting.
				lock (this) {
					String page;
					try {
						page = HttpHelper.HttpGet( new Uri( "http://phone" ), "", "", "", 1000 );
					} catch (WebException e) {
						throw new DeviceNotRespondingException( "The device is not responding to status requests", e );
					}
					
					IList<DeviceStateChange> deviceChanges = new List<DeviceStateChange>();
					IList<LineStateChange> lineChanges = new List<LineStateChange>();
					IList<CallStateChange> callChanges = new List<CallStateChange>();
					AnalyseLineState( page, mDeviceState.LineStates[ 0 ], lineChanges, callChanges );	
					AnalyseLineState( page, mDeviceState.LineStates[ 1 ], lineChanges, callChanges );
				
					// FIXME safer to use event here?	
					if( deviceChanges.Count > 0 || lineChanges.Count > 0 || callChanges.Count > 0 )
						StateManager.Instance().DeviceStateUpdated( this, deviceChanges, lineChanges, callChanges );
				}			
				
				Thread.Sleep( 1000 );
			}
        }
        
        protected virtual void AnalyseLineState( String page, LineState lineState, IList<LineStateChange> lineChanges, IList<CallStateChange> callChanges ) {

			String lastCalledNumber = StringHelper.ExtractSubstring( page, "Last Called Number:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" );
			if( lastCalledNumber != lineState.LastCalledNumber )
				lineChanges.Add( new LineStateChange( lineState, "lastCalledNumber", lineState.LastCalledNumber, lastCalledNumber ) );
			lineState.LastCalledNumber = lastCalledNumber;
				
			String lastCallerNumber = StringHelper.ExtractSubstring( page, "Last Caller Number:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" );
			if( lastCallerNumber != lineState.LastCallerNumber )
				lineChanges.Add( new LineStateChange( lineState, "lastCallerNumber", lineState.LastCallerNumber, lastCallerNumber ) );
			lineState.LastCallerNumber = lastCallerNumber;

			RegistrationState registrationState = GetRegistrationState( StringHelper.ExtractSubstring( page, "Registration State:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ) );
			if( registrationState != lineState.RegistrationState )
				lineChanges.Add( new LineStateChange( lineState, "registrationState", lineState.RegistrationState.ToString(), registrationState.ToString() ) );
			lineState.RegistrationState = registrationState;

			bool messageWaiting = StringHelper.ExtractSubstring( page, "Message Waiting:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ) != "No";
			if( messageWaiting != lineState.MessageWaiting )
				lineChanges.Add( new LineStateChange( lineState, "messageWaiting", lineState.MessageWaiting.ToString(), messageWaiting.ToString() ) );
			lineState.MessageWaiting = messageWaiting;

			// Analyse both calls for the line. (We know this device only
			// supports two calls per line.)
			AnalyseCallState( page, lineState.CallStates[ 0 ], lineState, callChanges );
			AnalyseCallState( page, lineState.CallStates[ 1 ], lineState, callChanges );
        }
        
        protected virtual void AnalyseCallState( String page, CallState callState, LineState lineState, IList<CallStateChange> callChanges ) {
			Activity callActivity = GetActivity( StringHelper.ExtractSubstring( page, callState.Name + " State:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ) );
			if( callActivity != callState.Activity )
				callChanges.Add( new CallStateChange( callState, "activity", callState.Activity.ToString(), callActivity.ToString() ) );
			callState.Activity = callActivity;
			
			String duration = StringHelper.ExtractSubstring( page, callState.Name + " Duration:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" );
			if( duration != callState.Duration )
				callChanges.Add( new CallStateChange( callState, "duration", callState.Duration, duration ) );
			callState.Duration = duration;

			CallType type = GetCallType( StringHelper.ExtractSubstring( page, callState.Name + " Type:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ) );
			if( type != callState.Type )
				callChanges.Add( new CallStateChange( callState, "type", callState.Type.ToString(), type.ToString() ) );
			callState.Type = type;

			Tone tone = GetTone( StringHelper.ExtractSubstring( page, callState.Name + " Tone:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ) );
			if( tone != callState.Tone )
				callChanges.Add( new CallStateChange( callState, "tone", callState.Tone.ToString(), tone.ToString() ) );
			callState.Tone = tone;
			
			String encoder = StringHelper.ExtractSubstring( page, callState.Name + " Encoder:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" );
			if( encoder != callState.Encoder )
				callChanges.Add( new CallStateChange( callState, "encoder", callState.Encoder, encoder ) );
			callState.Encoder = encoder;
			
			String decoder = StringHelper.ExtractSubstring( page, callState.Name + " Decoder:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" );
			if( decoder != callState.Decoder )
				callChanges.Add( new CallStateChange( callState, "decoder", callState.Decoder, decoder ) );
			callState.Decoder = decoder;
			
			long bytesSent = ParseInt64( StringHelper.ExtractSubstring( page, callState.Name + " Bytes Sent:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ) );
			if( bytesSent != callState.BytesSent )
				callChanges.Add( new CallStateChange( callState, "bytesSent", callState.BytesSent, bytesSent ) );
			callState.BytesSent = bytesSent;

			long bytesReceived = ParseInt64( StringHelper.ExtractSubstring( page, callState.Name + " Bytes Recv:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ) );
			if( bytesReceived != callState.BytesReceived )
				callChanges.Add( new CallStateChange( callState, "bytesReceived", callState.BytesReceived, bytesReceived ) );
			callState.BytesReceived = bytesReceived;
			
			long packetLoss = ParseInt64( StringHelper.ExtractSubstring( page, callState.Name + " Packets Lost:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ) );
			if( packetLoss != callState.PacketLoss )
				callChanges.Add( new CallStateChange( callState, "packetLoss", callState.PacketLoss, packetLoss ) );
			callState.PacketLoss = packetLoss;
			
			long packetError = ParseInt64( StringHelper.ExtractSubstring( page, callState.Name + " Packet Error:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ) );
			if( packetError != callState.PacketError )
				callChanges.Add( new CallStateChange( callState, "packetError", callState.PacketError, packetError ) );
			callState.PacketError = packetError;
			
			long jitter = ParseInt64( StringHelper.ExtractSubstring( page, callState.Name + " Jitter:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ).Replace( " ms", "") );
			if( jitter != callState.Jitter )
				callChanges.Add( new CallStateChange( callState, "jitter", callState.Jitter, jitter ) );
			callState.Jitter = jitter;
			
			long decodeLatency = ParseInt64( StringHelper.ExtractSubstring( page, callState.Name + " Decode Latency:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ).Replace( " ms", "") );
			if( decodeLatency != callState.DecodeLatency )
				callChanges.Add( new CallStateChange( callState, "decodeLatency", callState.DecodeLatency, decodeLatency ) );
			callState.DecodeLatency = decodeLatency;
			
			long roundTripDelay = ParseInt64( StringHelper.ExtractSubstring( page, callState.Name + " Round Trip Delay:<td><font color=\"darkblue\">", "<", lineState.Name + " Status" ).Replace( " ms", "") );
			if( roundTripDelay != callState.RoundTripDelay )
				callChanges.Add( new CallStateChange( callState, "roundTripDelay", callState.RoundTripDelay, roundTripDelay ) );
			callState.RoundTripDelay = roundTripDelay;
        }
        
        public static long ParseInt64( String parse ) {
			long value = 0L;
			
			try {
				value = Int64.Parse( parse );
			} catch (FormatException) {
				// Safe to do nothing.
			}
			
			return value;
        }

		public virtual CallType GetCallType( String type ) {
			if( type == "Inbound" )
				return CallType.Inbound;
			else if( type == "Outbound" )
				return CallType.Outbound;
			else if( type == "" )
				return CallType.IdleDisconnected;
			throw new ArgumentOutOfRangeException( "Type " + type + " not found." );
		}
		
		public virtual Tone GetTone( String tone ) {
			if( tone == "None" )
				return Tone.None;
			else if( tone == "Dial" )
				return Tone.Dial;
			else if( tone == "Secure Call Indication" )
				return Tone.SecureCall;
			// FIXME does this work?
			else if( tone == "Busy" )
				return Tone.Busy;
			// FIXME does this work?
			else if( tone == "Call" )
				return Tone.Call;
			throw new ArgumentOutOfRangeException( "Tone " + tone + " not found." );
		}

		public virtual Activity GetActivity( String activity ) {
			if( activity == "Idle" )
				return Activity.IdleDisconnected;
			else if( activity == "Connected" )
				return Activity.Connected;
			else if( activity == "Ringing" )
				return Activity.Ringing;
			else if( activity == "Dialing" )
				return Activity.Dialing;
			else if( activity == "Invalid" )
				return Activity.Error;
			throw new ArgumentOutOfRangeException( "Activity " + activity + " not found." );
		}
        
        public virtual RegistrationState GetRegistrationState( String state ) {
			if( state == "Online" )
				return RegistrationState.Online;
			else if( state == "Offline" )
				return RegistrationState.Offline;
			throw new ArgumentOutOfRangeException( "State " + state + " not found." );
        }
    }
}