using System;
using System.Collections.Generic;
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
    public class LinksysPAP2DeviceMonitor : DeviceMonitor {
	    
		protected DeviceState mDeviceState = new DeviceState( new LineState[] { } );
	    
        public void Run() {
			while( true ) {
				String page = HttpHelper.HttpGet( new Uri( "http://phone" ), "", "", "", 1000 );
				
				// We know there will be two lines for the device, each with
				// two calls.
				DeviceState state = new DeviceState( new LineState[] {
					new LineState( new CallState[] { new CallState(), new CallState() } ),
					new LineState( new CallState[] { new CallState(), new CallState() } )
				} );
				
				state.LineStates[ 0 ].LastCalledNumber = StringHelper.ExtractSubstring( page, "Last Called Number:<td><font color=\"darkblue\">", "<", "Line 1 Status" );
				state.LineStates[ 1 ].LastCalledNumber = StringHelper.ExtractSubstring( page, "Last Called Number:<td><font color=\"darkblue\">", "<", "Line 2 Status" );

				state.LineStates[ 0 ].LastCallerNumber = StringHelper.ExtractSubstring( page, "Last Caller Number:<td><font color=\"darkblue\">", "<", "Line 1 Status" );
				state.LineStates[ 1 ].LastCallerNumber = StringHelper.ExtractSubstring( page, "Last Caller Number:<td><font color=\"darkblue\">", "<", "Line 2 Status" );

				state.LineStates[ 0 ].RegistrationState = GetRegistrationState( StringHelper.ExtractSubstring( page, "Registration State:<td><font color=\"darkblue\">", "<", "Line 1 Status" ) );
				state.LineStates[ 1 ].RegistrationState = GetRegistrationState( StringHelper.ExtractSubstring( page, "Registration State:<td><font color=\"darkblue\">", "<", "Line 2 Status" ) );

				state.LineStates[ 0 ].CallStates[ 0 ].CallActivity = GetCallActivity( StringHelper.ExtractSubstring( page, "Call 1 Type:<td><font color=\"darkblue\">", "<", "Line 1 Status" ) );
				state.LineStates[ 0 ].CallStates[ 1 ].CallActivity = GetCallActivity( StringHelper.ExtractSubstring( page, "Call 2 Type:<td><font color=\"darkblue\">", "<", "Line 1 Status" ) );
				state.LineStates[ 1 ].CallStates[ 0 ].CallActivity = GetCallActivity( StringHelper.ExtractSubstring( page, "Call 1 Type:<td><font color=\"darkblue\">", "<", "Line 2 Status" ) );
				state.LineStates[ 1 ].CallStates[ 1 ].CallActivity = GetCallActivity( StringHelper.ExtractSubstring( page, "Call 2 Type:<td><font color=\"darkblue\">", "<", "Line 2 Status" ) );

				state.LineStates[ 0 ].CallStates[ 0 ].Duration = StringHelper.ExtractSubstring( page, "Call 1 Duration:<td><font color=\"darkblue\">", "<", "Line 1 Status" );
				state.LineStates[ 0 ].CallStates[ 1 ].Duration = StringHelper.ExtractSubstring( page, "Call 2 Duration:<td><font color=\"darkblue\">", "<", "Line 1 Status" );
				state.LineStates[ 1 ].CallStates[ 0 ].Duration = StringHelper.ExtractSubstring( page, "Call 1 Duration:<td><font color=\"darkblue\">", "<", "Line 2 Status" );			
				state.LineStates[ 1 ].CallStates[ 1 ].Duration = StringHelper.ExtractSubstring( page, "Call 2 Duration:<td><font color=\"darkblue\">", "<", "Line 2 Status" );

				mDeviceState = state;
				
				StateManager.Instance().DeviceStateUpdated( this );
				Thread.Sleep( 1000 );
			}
        }

		public static CallActivity GetCallActivity( String activity ) {
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
        
        public static RegistrationState GetRegistrationState( String state ) {
			if( state == "Online" )
				return RegistrationState.Online;
			else if( state == "Offline" )
				return RegistrationState.Offline;
			return RegistrationState.Other;
        }

        public DeviceState GetDeviceState() {
			return mDeviceState;
        }
    }
}
