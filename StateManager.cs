using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Xml;

using LothianProductions.Data;
using LothianProductions.Util.Settings;
using LothianProductions.VoIP.Behaviour;
using LothianProductions.VoIP.Monitor;
using LothianProductions.VoIP.Monitor.Impl;
using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP {
	public delegate void StateUpdateHandler( IDeviceStateMonitor monitor, StateUpdateEventArgs e );

    public class StateManager {	
		protected static readonly StateManager mInstance = new StateManager();

        protected Dictionary<Call, CallRecord> mCalls = new Dictionary<Call, CallRecord>();
        protected Dictionary<Object, Dictionary<String, StateChangeBehaviour>> mStateProperties = new Dictionary<Object, Dictionary<String, StateChangeBehaviour>>();
		
		public static StateManager Instance() {
			return mInstance;
		}
		
		public event StateUpdateHandler StateUpdate;
		protected Dictionary<IDeviceStateMonitor, Thread> mDeviceStateMonitors = new Dictionary<IDeviceStateMonitor, Thread>();
    
		protected StateManager() {
			// Initialize device monitors, have to give each its own thread.
			ReloadDeviceStateMonitors();
		}

		public void ReloadDeviceStateMonitors() {
			lock (mDeviceStateMonitors) {
				// Stop currently running threads.
				foreach( IDeviceStateMonitor monitor in mDeviceStateMonitors.Keys )
					mDeviceStateMonitors[ monitor ].Abort();
					
				mDeviceStateMonitors.Clear();

				NameValueCollection config = (NameValueCollection) ConfigurationManager.GetSection( "hvoipm/deviceStateMonitors" );

				if( config == null )
					throw new MissingAppSettingsException( "The section \"hvoipm/deviceStateMonitors\" was not present in the application configuration." );

				foreach( String key in config.Keys ) {				
					Type type = Type.GetType( config[ key ], true );

					IDeviceStateMonitor monitor = (IDeviceStateMonitor) type.InvokeMember(
						"",
						BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance,
						null,
						null,
						new String[] { key }
					);

					Thread thread = new Thread( new ThreadStart( monitor.Run ) );
					
					mDeviceStateMonitors.Add( monitor, thread );
					thread.Start();
                }
                Logger.Instance().Log("Loaded " + mDeviceStateMonitors.Count + " device state monitor(s)");
			}
		}
		
		public ICollection<IDeviceStateMonitor> DeviceStateMonitors {
			get{ return mDeviceStateMonitors.Keys; }
		}

		public void DeviceStateUpdated(	IDeviceStateMonitor monitor, IList<DeviceChange> deviceChanges,
										IList<LineChange> lineChanges, IList<CallChange> callChanges ) {
			// Clever logging stuff here.
			foreach (CallChange change in callChanges) {
				if (change.Property == "activity") {
                    if ( change.ChangedFrom == Activity.IdleDisconnected.ToString() && change.ChangedTo != Activity.IdleDisconnected.ToString() ) {
                        CallRecord call = new CallRecord( monitor.GetDeviceState(), GetLine( change.Call ), change.Call, DateTime.Now, new DateTime() );
						
						// FIXME add sanity check in case somehow call is already there
						if ( mCalls.ContainsKey( change.Call ) ) {
							mCalls.Remove( change.Call );
							Logger.Instance().Log("Call #" + change.Call.Name + " had to be removed from the call list - did the previous call fail?");
						}
                        mCalls.Add( change.Call, call );
					} else if ( change.ChangedFrom != Activity.IdleDisconnected.ToString() && change.ChangedTo == Activity.IdleDisconnected.ToString() ) {
						CallRecord call = mCalls[change.Call];
						call.EndTime = DateTime.Now;
						CallLogger.Instance().Log(call);
						mCalls.Remove( change.Call );
					}
                }

				if (change.Call.Activity != Activity.IdleDisconnected) {
					CallRecord call = mCalls[change.Call];
					call.Call = change.Call;
					call.Line = GetLine(change.Call);
					mCalls[change.Call] = call;
				}

				bool log = false;
				try {
					log = LookupBehaviour(change.Call, change.Property).Log;
				} catch (ConfigurationErrorsException ce) {
				}
				if (log)
					Logger.Instance().Log("Call property " + change.Property + " has changed from " + change.ChangedFrom + " to " + change.ChangedTo);
			}

			foreach (DeviceChange change in deviceChanges) {
				bool log = false;
				try {
					log = LookupBehaviour(change.Device, change.Property).Log;
				} catch (ConfigurationErrorsException ce) {
				}
				if ( log )
					Logger.Instance().Log( "Device property " + change.Property + " has changed from " + change.ChangedFrom + " to " + change.ChangedTo );
			}
			
			foreach (LineChange change in lineChanges) {
				bool log = false;
				try {
					log = LookupBehaviour(change.Line, change.Property).Log;
				} catch (ConfigurationErrorsException ce) {
				}
				if (log)
					Logger.Instance().Log("Line property " + change.Property + " has changed from " + change.ChangedFrom + " to " + change.ChangedTo);
			}

			if (StateUpdate != null)
				StateUpdate(monitor, new StateUpdateEventArgs(deviceChanges, lineChanges, callChanges));

		}
		
		public StateChangeBehaviour LookupBehaviour( Object state, String property ) {		
			// Behaviour not set yet.
			if( ! mStateProperties.ContainsKey( state ) )
				mStateProperties.Add( state, new Dictionary<String, StateChangeBehaviour>() );
			
			Dictionary<String, StateChangeBehaviour> propertyBehaviours = mStateProperties[ state ];
			
			if( ! propertyBehaviours.ContainsKey( property ) )
				propertyBehaviours.Add( property, GetBehaviourFromXml( state.GetType().Name, property ) );
				
			return propertyBehaviours[ property ];			
		}
		
		protected StateChangeBehaviour GetBehaviourFromXml( String stateType, String property ) {
			XmlNode node = (XmlNode) ConfigurationManager.GetSection( "hvoipm/behaviours" );
			
			if( node == null )
				throw new ConfigurationErrorsException( "Could not find behaviours section in application configuration." );

			XmlNode behaviour = node.SelectSingleNode( "behaviour[@stateType='" + stateType + "' and @property='" + property + "']" );
			if( behaviour == null )
				throw new ConfigurationErrorsException( "Could not find behaviour description for " + stateType + "." + property + "\" in application configuration." );

			return new StateChangeBehaviour(
				Boolean.Parse( behaviour.Attributes[ "showBubble" ].Value ),
				behaviour.Attributes[ "bubbleText" ].Value,
				Boolean.Parse( behaviour.Attributes[ "systemTrayWarning" ].Value ),
				Boolean.Parse( behaviour.Attributes[ "showApplication" ].Value ),
				( behaviour.Attributes[ "showCriteria" ] == null ? "" : behaviour.Attributes[ "showCriteria" ].Value ).Split( ',' ),
				Boolean.Parse( behaviour.Attributes[ "log" ].Value )
			);
		}
		
		// Helper functions for linking states.
		public IDeviceStateMonitor GetMonitor( Device deviceState ) {
			foreach( IDeviceStateMonitor monitor in DeviceStateMonitors )
				if( monitor.GetDeviceState() == deviceState )
					return monitor;
			throw new DomainObjectNotFoundException( "Couldn't find a monitor owning the specified device." );
		}

		public IDeviceStateMonitor GetMonitor( Line lineState ) {
			foreach( IDeviceStateMonitor monitor in DeviceStateMonitors )
				foreach( Line line in monitor.GetDeviceState().Lines )
					if( line == lineState )
						return monitor;
			throw new DomainObjectNotFoundException( "Couldn't find a monitor owning the specified line." );
		}

		public IDeviceStateMonitor GetMonitor( Call callState ) {
			foreach( IDeviceStateMonitor monitor in DeviceStateMonitors )
				foreach( Line line in monitor.GetDeviceState().Lines )
					foreach( Call call in line.Calls )
						if( call == callState )
							return monitor;
			throw new DomainObjectNotFoundException( "Couldn't find a monitor owning the specified call." );
		}
		
		public Device GetDevice( Line lineState ) {
			foreach( IDeviceStateMonitor monitor in DeviceStateMonitors )
				foreach( Line line in monitor.GetDeviceState().Lines )
					if( line == lineState )
						return monitor.GetDeviceState();
			throw new DomainObjectNotFoundException( "Couldn't find a device owning the specified line." );
		}
		
		public Device GetDevice( Call callState ) {
			foreach( IDeviceStateMonitor monitor in DeviceStateMonitors )
				foreach( Line line in monitor.GetDeviceState().Lines )
					foreach( Call call in line.Calls )
						if( call == callState )
							return monitor.GetDeviceState();
			throw new DomainObjectNotFoundException( "Couldn't find a device owning the specified call." );
		}
		
		public Line GetLine( Call callState ) {
			foreach( IDeviceStateMonitor monitor in DeviceStateMonitors )
				foreach( Line line in monitor.GetDeviceState().Lines )
					foreach( Call call in line.Calls )
						if( call == callState )
							return line;
			throw new DomainObjectNotFoundException( "Couldn't find a line owning the specified call." );
		}
    }
}
