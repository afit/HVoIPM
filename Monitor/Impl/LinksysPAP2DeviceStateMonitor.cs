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
				changes.Add( new LineStateChange( lineState, "LastCalledNumber", lineState.LastCalledNumber, lastCalledNumber ) );
			lineState.LastCalledNumber = lastCalledNumber;
				
			String lastCallerNumber = StringHelper.ExtractSubstring( page, "Last Caller Number:<td><font color=\"darkblue\">", "<", "Line " + lineState.Name + " Status" );
			if( lastCallerNumber != lineState.LastCallerNumber )
				changes.Add( new LineStateChange( lineState, "LastCallerNumber", lineState.LastCallerNumber, lastCallerNumber ) );
			lineState.LastCallerNumber = lastCallerNumber;

			RegistrationState registrationState = GetRegistrationState( StringHelper.ExtractSubstring( page, "Registration State:<td><font color=\"darkblue\">", "<", "Line " + lineState.Name + " Status" ) );
			if( registrationState != lineState.RegistrationState )
				changes.Add( new LineStateChange( lineState, "RegistrationState", lineState.RegistrationState.ToString(), registrationState.ToString() ) );
			lineState.RegistrationState = registrationState;

			// Analyse both calls for the line. (We know this device only
			// supports two calls per line.)
			AnalyseCallState( page, lineState.CallStates[ 0 ], lineState, changes );
			AnalyseCallState( page, lineState.CallStates[ 1 ], lineState, changes );
        }
        
        protected virtual void AnalyseCallState( String page, CallState callState, LineState lineState, IList<StateChange> changes ) {
			CallActivity callActivity = GetCallActivity( StringHelper.ExtractSubstring( page, "Call " + callState.Name + " Type:<td><font color=\"darkblue\">", "<", "Line " + lineState.Name + " Status" ) );
			if( callActivity != callState.CallActivity )
				changes.Add( new CallStateChange( callState, "callActivity", callState.CallActivity.ToString(), callActivity.ToString() ) );
			callState.CallActivity = callActivity;
			
			String duration = StringHelper.ExtractSubstring( page, "Call " + callState.Name + " Duration:<td><font color=\"darkblue\">", "<", "Line " + lineState.Name + " Status" );
			if( duration != callState.Duration )
				changes.Add( new CallStateChange( callState, "duration", callState.Duration, duration ) );
			callState.Duration = duration;
        }

		public virtual CallActivity GetCallActivity( String activity ) {
			if( activity == "" )
				return CallActivity.Idle;
			else if( activity == "Inbound" )
				return CallActivity.Inbound;
			else if( activity == "Outbound" )
				return CallActivity.Outbound;
			else if( activity == "Held" )
				return CallActivity.Held;
			return CallActivity.Other;
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