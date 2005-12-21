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
			mDeviceState = new DeviceState( new LineState[] {
				new LineState( new CallState[] { new CallState(), new CallState() } ),
				new LineState( new CallState[] { new CallState(), new CallState() } )
			} );
		}
	    
        public override void Run() {
			while( true ) {
				// Lock to prevent recursion synchronicity problems
				// caused by slow wgetting.
				lock (this) {
					String page;
					try {
						page = HttpHelper.HttpGet(new Uri("http://phone"), "", "", "", 1000);
					} catch (WebException e) {
						throw new DeviceNotRespondingException( "The device is not responding to status requests", e );
					}
					
					IList<StateChange> changes = new List<StateChange>();
					AnalyseLineState( page, 0, changes );	
					AnalyseLineState( page, 1, changes );
				
					// FIXME safer to use event here?	
					if( changes.Count > 0 )
						StateManager.Instance().DeviceStateUpdated( this, changes );
				}			
				
				Thread.Sleep( 1000 );
			}
        }
        
        protected virtual void AnalyseLineState( String page, int lineIndex, IList<StateChange> changes ) {
			String lastCalledNumber = StringHelper.ExtractSubstring( page, "Last Called Number:<td><font color=\"darkblue\">", "<", "Line " + ( lineIndex + 1 )+ " Status" );
			if( lastCalledNumber != mDeviceState.LineStates[ lineIndex ].LastCalledNumber )
				changes.Add( new LineStateChange( lineIndex, "LastCalledNumber", mDeviceState.LineStates[ lineIndex ].LastCalledNumber, lastCalledNumber ) );
			mDeviceState.LineStates[ lineIndex ].LastCalledNumber = lastCalledNumber;
				
			String lastCallerNumber = StringHelper.ExtractSubstring( page, "Last Caller Number:<td><font color=\"darkblue\">", "<", "Line " + ( lineIndex + 1 ) + " Status" );
			if( lastCallerNumber != mDeviceState.LineStates[ 0 ].LastCallerNumber )
				changes.Add( new LineStateChange( lineIndex, "LastCallerNumber", mDeviceState.LineStates[ lineIndex ].LastCallerNumber, lastCallerNumber ) );
			mDeviceState.LineStates[ lineIndex ].LastCallerNumber = lastCallerNumber;

			RegistrationState registrationState = GetRegistrationState( StringHelper.ExtractSubstring( page, "Registration State:<td><font color=\"darkblue\">", "<", "Line " + ( lineIndex + 1 ) + " Status" ) );
			if( registrationState != mDeviceState.LineStates[ lineIndex ].RegistrationState )
				changes.Add( new LineStateChange( lineIndex, "RegistrationState", mDeviceState.LineStates[ lineIndex ].RegistrationState.ToString(), registrationState.ToString() ) );
			mDeviceState.LineStates[ lineIndex ].RegistrationState = registrationState;

			// Analyse both calls for the line. (We know this device only
			// supports two calls per line.)
			AnalyseCallState( page, lineIndex, 0, changes );
			AnalyseCallState( page, lineIndex, 1, changes );
        }
        
        protected virtual void AnalyseCallState( String page, int callIndex, int lineIndex, IList<StateChange> changes ) {
			CallActivity callActivity = GetCallActivity( StringHelper.ExtractSubstring( page, "Call " + ( callIndex + 1 ) + " Type:<td><font color=\"darkblue\">", "<", "Line " + ( lineIndex + 1 ) + " Status" ) );
			if( callActivity != mDeviceState.LineStates[ lineIndex ].CallStates[ callIndex ].CallActivity )
				changes.Add( new CallStateChange( callIndex, "callActivity", mDeviceState.LineStates[ lineIndex ].CallStates[ callIndex ].CallActivity.ToString(), callActivity.ToString() ) );
			mDeviceState.LineStates[ lineIndex ].CallStates[ callIndex ].CallActivity = callActivity;
			
			String duration = StringHelper.ExtractSubstring( page, "Call " + ( callIndex + 1 ) + " Duration:<td><font color=\"darkblue\">", "<", "Line " + ( lineIndex + 1 ) + " Status" );
			if( duration != mDeviceState.LineStates[ lineIndex ].CallStates[ callIndex ].Duration )
				changes.Add( new CallStateChange( callIndex, "duration", mDeviceState.LineStates[ lineIndex ].CallStates[ callIndex ].Duration, duration ) );
			mDeviceState.LineStates[ lineIndex ].CallStates[ callIndex ].Duration = duration;
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