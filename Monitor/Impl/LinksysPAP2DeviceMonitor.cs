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
    public class LinksysPAP2DeviceMonitor : AbstractWebDeviceMonitor {

		public LinksysPAP2DeviceMonitor( String name ) {
			mName = name;
			
			// We know there will be two lines for the device, each with
			// two calls.
			mDeviceState = new Device( name, new Line[] {
				new Line( "Line 1", new Call[] { new Call( "Call 1" ), new Call( "Call 2" ) } ),
				new Line( "Line 2", new Call[] { new Call( "Call 1" ), new Call( "Call 2" ) } )
			} );
		}
        
        protected override void AnalyseDevice( String page, IList<DevicePropertyChange> deviceChanges, IList<LinePropertyChange> lineChanges, IList<CallPropertyChange> callChanges ) {
			foreach( Line line in mDeviceState.Lines )
				AnalyseLine( page, line, lineChanges, callChanges );	
		}		
        
        protected override void AnalyseLine( String page, Line lineState, IList<LinePropertyChange> lineChanges, IList<CallPropertyChange> callChanges ) {

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

			// Analyse all calls for the line.
			foreach( Call call in lineState.Calls )
				AnalyseCall( page, call, lineState, callChanges );
        }
        
        protected override void AnalyseCall( String page, Call callState, Line lineState, IList<CallPropertyChange> callChanges ) {
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
	}
}
