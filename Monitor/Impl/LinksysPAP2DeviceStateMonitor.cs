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
				new LineState( "1", new CallState[] { new CallState( "1" ), new CallState( "2" ) } ),
				new LineState( "2", new CallState[] { new CallState( "1" ), new CallState( "2" ) } )
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
					
					IList<StateChange> changes = new List<StateChange>();
					AnalyseLineState( page, mDeviceState.LineStates[ 0 ], changes );	
					AnalyseLineState( page, mDeviceState.LineStates[ 1 ], changes );
				
					// FIXME safer to use event here?	
					if( changes.Count > 0 )
						StateManager.Instance().DeviceStateUpdated( this, changes );
				}			
				
				Thread.Sleep( 1000 );
			}
        }
        
        protected virtual void AnalyseLineState( String page, LineState lineState, IList<StateChange> changes ) {

			String lastCalledNumber = StringHelper.ExtractSubstring( page, "Last Called Number:<td><font color=\"darkblue\">", "<", "Line " + lineState.Name + " Status" );
			if( lastCalledNumber != lineState.LastCalledNumber )
				changes.Add( new LineStateChange( lineState, "lastCalledNumber", lineState.LastCalledNumber, lastCalledNumber ) );
			lineState.LastCalledNumber = lastCalledNumber;
				
			String lastCallerNumber = StringHelper.ExtractSubstring( page, "Last Caller Number:<td><font color=\"darkblue\">", "<", "Line " + lineState.Name + " Status" );
			if( lastCallerNumber != lineState.LastCallerNumber )
				changes.Add( new LineStateChange( lineState, "lastCallerNumber", lineState.LastCallerNumber, lastCallerNumber ) );
			lineState.LastCallerNumber = lastCallerNumber;

			RegistrationState registrationState = GetRegistrationState( StringHelper.ExtractSubstring( page, "Registration State:<td><font color=\"darkblue\">", "<", "Line " + lineState.Name + " Status" ) );
			if( registrationState != lineState.RegistrationState )
				changes.Add( new LineStateChange( lineState, "registrationState", lineState.RegistrationState.ToString(), registrationState.ToString() ) );
			lineState.RegistrationState = registrationState;

			bool messageWaiting = StringHelper.ExtractSubstring( page, "Message Waiting:<td><font color=\"darkblue\">", "<", "Line " + lineState.Name + " Status" ) == "No";
			if( messageWaiting != lineState.MessageWaiting )
				changes.Add( new LineStateChange( lineState, "messageWaiting", lineState.MessageWaiting.ToString(), messageWaiting.ToString() ) );
			lineState.MessageWaiting = messageWaiting;

			// Analyse both calls for the line. (We know this device only
			// supports two calls per line.)
			AnalyseCallState( page, lineState.CallStates[ 0 ], lineState, changes );
			AnalyseCallState( page, lineState.CallStates[ 1 ], lineState, changes );
        }
        
        protected virtual void AnalyseCallState( String page, CallState callState, LineState lineState, IList<StateChange> changes ) {
			Activity callActivity = GetActivity( StringHelper.ExtractSubstring( page, "Call " + callState.Name + " Type:<td><font color=\"darkblue\">", "<", "Line " + lineState.Name + " Status" ) );
			if( callActivity != callState.Activity )
				changes.Add( new CallStateChange( callState, "activity", callState.Activity.ToString(), callActivity.ToString() ) );
			callState.Activity = callActivity;
			
			String duration = StringHelper.ExtractSubstring( page, "Call " + callState.Name + " Duration:<td><font color=\"darkblue\">", "<", "Line " + lineState.Name + " Status" );
			if( duration != callState.Duration )
				changes.Add( new CallStateChange( callState, "duration", callState.Duration, duration ) );
			callState.Duration = duration;

			CallType type = GetCallType( StringHelper.ExtractSubstring( page, "Call " + callState.Name + " Type:<td><font color=\"darkblue\">", "<", "Line " + lineState.Name + " Status" ) );
			if( type != callState.Type )
				changes.Add( new CallStateChange( callState, "type", callState.Type.ToString(), type.ToString() ) );
			callState.Type = type;

			Tone tone = GetTone( StringHelper.ExtractSubstring( page, "Call " + callState.Name + " Tone:<td><font color=\"darkblue\">", "<", "Line " + lineState.Name + " Status" ) );
			if( tone != callState.Tone )
				changes.Add( new CallStateChange( callState, "tone", callState.Tone.ToString(), tone.ToString() ) );
			callState.Tone = tone;
			
			String encoder = StringHelper.ExtractSubstring( page, "Call " + callState.Name + " Encoder:<td><font color=\"darkblue\">", "<", "Line " + lineState.Name + " Status" );
			if( encoder != callState.Encoder )
				changes.Add( new CallStateChange( callState, "encoder", callState.Encoder, encoder ) );
			callState.Encoder = encoder;
			
			String decoder = StringHelper.ExtractSubstring( page, "Call " + callState.Name + " Decoder:<td><font color=\"darkblue\">", "<", "Line " + lineState.Name + " Status" );
			if( decoder != callState.Decoder )
				changes.Add( new CallStateChange( callState, "decoder", callState.Decoder, decoder ) );
			callState.Decoder = decoder;
			
			long bytesSent = ParseInt64( StringHelper.ExtractSubstring( page, "Call " + callState.Name + " BytesSent:<td><font color=\"darkblue\">", "<", "Line " + lineState.Name + " Status" ) );
			if( bytesSent != callState.BytesSent )
				changes.Add( new CallStateChange( callState, "bytesSent", callState.BytesSent, bytesSent ) );
			callState.BytesSent = bytesSent;

			long bytesReceived = ParseInt64( StringHelper.ExtractSubstring( page, "Call " + callState.Name + " BytesReceived:<td><font color=\"darkblue\">", "<", "Line " + lineState.Name + " Status" ) );
			if( bytesReceived != callState.BytesReceived )
				changes.Add( new CallStateChange( callState, "bytesReceived", callState.BytesReceived, bytesReceived ) );
			callState.BytesReceived = bytesReceived;
			
			long packetLoss = ParseInt64( StringHelper.ExtractSubstring( page, "Call " + callState.Name + " PacketLoss:<td><font color=\"darkblue\">", "<", "Line " + lineState.Name + " Status" ) );
			if( packetLoss != callState.PacketLoss )
				changes.Add( new CallStateChange( callState, "packetLoss", callState.PacketLoss, packetLoss ) );
			callState.PacketLoss = packetLoss;
			
			long packetError = ParseInt64( StringHelper.ExtractSubstring( page, "Call " + callState.Name + " PacketError:<td><font color=\"darkblue\">", "<", "Line " + lineState.Name + " Status" ) );
			if( packetError != callState.PacketError )
				changes.Add( new CallStateChange( callState, "packetError", callState.PacketError, packetError ) );
			callState.PacketError = packetError;
			
			long jitter = ParseInt64( StringHelper.ExtractSubstring( page, "Call " + callState.Name + " Jitter:<td><font color=\"darkblue\">", "<", "Line " + lineState.Name + " Status" ).Replace( " ms", "") );
			if( jitter != callState.Jitter )
				changes.Add( new CallStateChange( callState, "jitter", callState.Jitter, jitter ) );
			callState.Jitter = jitter;
			
			long decodeLatency = ParseInt64( StringHelper.ExtractSubstring( page, "Call " + callState.Name + " DecodeLatency:<td><font color=\"darkblue\">", "<", "Line " + lineState.Name + " Status" ).Replace( " ms", "") );
			if( decodeLatency != callState.DecodeLatency )
				changes.Add( new CallStateChange( callState, "decodeLatency", callState.DecodeLatency, decodeLatency ) );
			callState.DecodeLatency = decodeLatency;
			
			long roundTripDelay = ParseInt64( StringHelper.ExtractSubstring( page, "Call " + callState.Name + " RoundTripDelay:<td><font color=\"darkblue\">", "<", "Line " + lineState.Name + " Status" ).Replace( " ms", "") );
			if( roundTripDelay != callState.RoundTripDelay )
				changes.Add( new CallStateChange( callState, "roundTripDelay", callState.RoundTripDelay, roundTripDelay ) );
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
			//if( type == "" )
				return CallType.IdleDisconnected;
		}
		
		public virtual Tone GetTone( String tone ) {
			//if( type == "" )
				return Tone.None;
		}

		public virtual Activity GetActivity( String activity ) {
			if( activity == "" )
				return Activity.IdleDisconnected;
			else if( activity == "Inbound" )
				return Activity.Inbound;
			else if( activity == "Outbound" )
				return Activity.Outbound;
			else if( activity == "Held" )
				return Activity.Held;
			return Activity.Other;
		}
        
        public virtual RegistrationState GetRegistrationState( String state ) {
			if( state == "Online" )
				return RegistrationState.Online;
			else if( state == "Offline" )
				return RegistrationState.Offline;
			return RegistrationState.Other;
        }
    }
}