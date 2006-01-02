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

namespace LothianProductions.VoIP.Monitor {

    public abstract class AbstractWebDeviceMonitor : DeviceMonitor {

        public override void Run() {
			int sleepTime = Int32.Parse( GetConfigurationValue( "PollInterval" ) );
			String hostname = GetConfigurationValue( "Hostname" );
			
			WebClient webClient = new WebClient();
			webClient.Headers.Add( "User-Agent", "HVoIPM / " + this.GetType().Name );
			
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
					
					AnalyseDevice( page, deviceChanges, lineChanges, callChanges );
					
					if( deviceChanges.Count > 0 || lineChanges.Count > 0 || callChanges.Count > 0 )
						StateManager.Instance().DeviceUpdated( this, deviceChanges, lineChanges, callChanges );
				}			
				
				Thread.Sleep( sleepTime );
			}
        }

		protected abstract void AnalyseDevice( String page, IList<DevicePropertyChange> deviceChanges, IList<LinePropertyChange> lineChanges, IList<CallPropertyChange> callChanges );
        protected abstract void AnalyseLine( String page, Line line, IList<LinePropertyChange> lineChanges, IList<CallPropertyChange> callChanges );
		protected abstract void AnalyseCall( String page, Call call, Line line, IList<CallPropertyChange> callChanges );
		
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
