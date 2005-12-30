using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Xml;

using LothianProductions.Data;
using LothianProductions.Util;
using LothianProductions.Util.Settings;
using LothianProductions.VoIP.Monitor;
using LothianProductions.VoIP.Monitor.Impl;
using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP {
	public delegate void StateUpdateHandler( IDeviceMonitor monitor, StateUpdateEventArgs e );

    public class StateManager {	
		protected static readonly StateManager mInstance = new StateManager();

        protected Dictionary<Call, CallRecord> mCalls = new Dictionary<Call, CallRecord>();
        protected Dictionary<Object, Dictionary<String, PropertyBehaviour>> mStateProperties = new Dictionary<Object, Dictionary<String, PropertyBehaviour>>();
		
		public static StateManager Instance() {
			return mInstance;
		}
		
		public event StateUpdateHandler StateUpdate;
		protected Dictionary<DeviceMonitorControl, Thread> mDeviceMonitorControls = new Dictionary<DeviceMonitorControl, Thread>();
    
		protected StateManager() {
			// Initialize device monitors, have to give each its own thread.
			ReloadDeviceStateMonitors();
		}

		public void ReloadDeviceStateMonitors() {
			lock (mDeviceMonitorControls) {
				// Stop currently running threads.
				foreach( DeviceMonitorControl control in mDeviceMonitorControls.Keys )
					mDeviceMonitorControls[ control ].Abort();
					
				mDeviceMonitorControls.Clear();

				NameValueCollection config = (NameValueCollection) ConfigurationManager.GetSection( "hvoipm/deviceStateMonitors" );

				if( config == null )
					throw new MissingAppSettingsException( "The section \"hvoipm/deviceStateMonitors\" was not present in the application configuration." );

				foreach( String key in config.Keys ) {				
					Type type = Type.GetType( config[ key ], true );

					IDeviceMonitor monitor = (IDeviceMonitor) type.InvokeMember(
						"",
						BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance,
						null,
						null,
						new String[] { key }
					);

					DeviceMonitorControl control = new DeviceMonitorControl( monitor );

					Thread thread = new Thread( new ThreadStart( control.Run ) );
					
					mDeviceMonitorControls.Add( control, thread );
					thread.Start();
                }
                
                Logger.Instance().Log( "Loaded " + mDeviceMonitorControls.Count + " device state monitor(s)" );
			}
		}
		
		public ICollection<DeviceMonitorControl> DeviceMonitorControls {
			get{ return mDeviceMonitorControls.Keys; }
		}

		public void DeviceStateUpdated(	IDeviceMonitor monitor, IList<DevicePropertyChange> deviceChanges,
										IList<LinePropertyChange> lineChanges, IList<CallPropertyChange> callChanges ) {
			// FIXME this is very inefficient
			List<PropertyChange> changes = new List<PropertyChange>();
			foreach( PropertyChange change in deviceChanges )
				changes.Add( change );
			foreach( PropertyChange change in lineChanges )
				changes.Add( change );
			foreach( PropertyChange change in callChanges )
				changes.Add( change );
				
			foreach( PropertyChange change in changes )
				// Note that we log changing to or from any given logging criteria
				if(	LookupPropertyChangeBehaviour( change.Underlying, change.Property, change.ChangedTo ).Log ||
					LookupPropertyChangeBehaviour( change.Underlying, change.Property, change.ChangedFrom ).Log )
					Logger.Instance().Log( change.Underlying.GetType().Name + " property " + change.Property + " has changed from " + change.ChangedFrom + " to " + change.ChangedTo );
						
			// Logging happens here:
			foreach( CallPropertyChange change in callChanges ) {
				if( change.Property == DeviceMonitor.PROPERTY_CALL_ACTIVITY ) {
                    if ( change.ChangedTo == Activity.Connected.ToString() ) {
                        CallRecord call = new CallRecord( monitor.GetDeviceState(), GetLine( change.Call ), change.Call, DateTime.Now, new DateTime() );
						
						// Sanity check in case somehow call is already there
						if ( mCalls.ContainsKey( change.Call ) ) {
							mCalls.Remove( change.Call );
							Logger.Instance().Log("Call #" + change.Call.Name + " had to be removed from the call list - did the previous call fail?");
						}
                        mCalls.Add( change.Call, call );
					} else if ( change.ChangedFrom == Activity.Connected.ToString() ) {
						CallRecord call = mCalls[change.Call];
						call.EndTime = DateTime.Now;
						CallLogger.Instance().Log(call);
						mCalls.Remove( change.Call );
					}
                }

				if (change.Call.Activity != Activity.IdleDisconnected) {
					if ( mCalls.ContainsKey( change.Call ) ) {
						CallRecord call = mCalls[change.Call];
						call.Call = change.Call;
						call.Line = GetLine(change.Call);
						mCalls[change.Call] = call;
					}
				}
			}
			
			if( StateUpdate != null )
				StateUpdate( monitor, new StateUpdateEventArgs( deviceChanges, lineChanges, callChanges ) );
		}
		
		public PropertyChangeBehaviour LookupPropertyChangeBehaviour( Object state, String property, String criteria ) {
			if( LookupPropertyBehaviour( state, property ).PropertyChangeBehaviours.ContainsKey( criteria ) )
				return LookupPropertyBehaviour( state, property ).PropertyChangeBehaviours[ criteria ];
			if( LookupPropertyBehaviour( state, property ).PropertyChangeBehaviours.ContainsKey( "" ) )
				return LookupPropertyBehaviour( state, property ).PropertyChangeBehaviours[ "" ];
				
			throw new ConfigurationErrorsException( "Could not find behaviours suitable for criteria \"" + criteria + "\" for property \"" + property + "\" in application configuration." );
		}
		
		public PropertyBehaviour LookupPropertyBehaviour( Object state, String property ) {		
			// Behaviour not set yet.
			if( ! mStateProperties.ContainsKey( state ) )
				mStateProperties.Add( state, new Dictionary<String, PropertyBehaviour>() );
			
			Dictionary<String, PropertyBehaviour> propertyBehaviours = mStateProperties[ state ];
			
			if( ! propertyBehaviours.ContainsKey( property ) )
				propertyBehaviours.Add( property, GetPropertyBehaviourFromXml( state.GetType().Name, property ) );
			
			return propertyBehaviours[ property ];	
		}
		
		protected PropertyBehaviour GetPropertyBehaviourFromXml( String stateType, String property ) {
			XmlNode node = (XmlNode) ConfigurationManager.GetSection( "hvoipm/behaviours" );
			
			if( node == null )
				throw new ConfigurationErrorsException( "Could not find behaviours section in application configuration." );

			XmlNode propertyNode = node.SelectSingleNode( "property[@stateType='" + stateType + "' and @property='" + property + "']" );
			if( propertyNode == null )
				throw new ConfigurationErrorsException( "Could not find behaviour description for " + stateType + "." + property + "\" in application configuration." );

			Dictionary<String, PropertyChangeBehaviour> changeBehaviours = new Dictionary<String, PropertyChangeBehaviour>();
			PropertyBehaviour behaviour = new PropertyBehaviour(
				propertyNode.Attributes[ "label" ].Value,
				changeBehaviours
			);
			
			foreach( XmlNode behaviourNode in propertyNode.ChildNodes )
				// Just add a single behaviour entry if it's a catch-all criteria
				// or if there's only one.
			    if( behaviourNode.Attributes[ "warningCriteria" ] == null || ! behaviourNode.Attributes[ "warningCriteria" ].Value.Contains( "," ) )
					changeBehaviours.Add(
						( behaviourNode.Attributes[ "warningCriteria" ] == null ? "" : behaviourNode.Attributes[ "warningCriteria" ].Value ),
						new PropertyChangeBehaviour(
							Boolean.Parse( behaviourNode.Attributes[ "showBubble" ].Value ),
							behaviourNode.Attributes[ "bubbleText" ].Value,
							Boolean.Parse( behaviourNode.Attributes[ "systemTrayWarning" ].Value ),
							Boolean.Parse( behaviourNode.Attributes[ "showApplication" ].Value ),
							behaviourNode.Attributes[ "externalProcess" ].Value,
							( behaviourNode.Attributes[ "warningCriteria" ] == null ? "" : behaviourNode.Attributes[ "warningCriteria" ].Value ),
							Boolean.Parse( behaviourNode.Attributes[ "log" ].Value )
						)
					);
				else
					foreach( String criteria in behaviourNode.Attributes[ "warningCriteria" ].Value.Split( ',' ) )
						changeBehaviours.Add(
							criteria,
							new PropertyChangeBehaviour(
								Boolean.Parse( behaviourNode.Attributes[ "showBubble" ].Value ),
								behaviourNode.Attributes[ "bubbleText" ].Value,
								Boolean.Parse( behaviourNode.Attributes[ "systemTrayWarning" ].Value ),
								Boolean.Parse( behaviourNode.Attributes[ "showApplication" ].Value ),
								behaviourNode.Attributes[ "externalProcess" ].Value,
								criteria,
								Boolean.Parse( behaviourNode.Attributes[ "log" ].Value )
							)
						);

			return behaviour;
		}
		
		// Helper functions for linking states.
		public IDeviceMonitor GetMonitor( Device deviceState ) {
			foreach( DeviceMonitorControl control in DeviceMonitorControls )
				if( control.DeviceMonitor.GetDeviceState() == deviceState )
					return control.DeviceMonitor;
			throw new DomainObjectNotFoundException( "Couldn't find a monitor owning the specified device." );
		}

		public IDeviceMonitor GetMonitor( Line lineState ) {
			foreach( DeviceMonitorControl control in DeviceMonitorControls )
				foreach( Line line in control.DeviceMonitor.GetDeviceState().Lines )
					if( line == lineState )
						return control.DeviceMonitor;
			throw new DomainObjectNotFoundException( "Couldn't find a monitor owning the specified line." );
		}

		public IDeviceMonitor GetMonitor( Call callState ) {
			foreach( DeviceMonitorControl control in DeviceMonitorControls )
				foreach( Line line in control.DeviceMonitor.GetDeviceState().Lines )
					foreach( Call call in line.Calls )
						if( call == callState )
							return control.DeviceMonitor;
			throw new DomainObjectNotFoundException( "Couldn't find a monitor owning the specified call." );
		}
		
		public Device GetDevice( Line lineState ) {
			foreach( DeviceMonitorControl control in DeviceMonitorControls )
				foreach( Line line in control.DeviceMonitor.GetDeviceState().Lines )
					if( line == lineState )
						return control.DeviceMonitor.GetDeviceState();
			throw new DomainObjectNotFoundException( "Couldn't find a device owning the specified line." );
		}
		
		public Device GetDevice( Call callState ) {
			foreach( DeviceMonitorControl control in DeviceMonitorControls )
				foreach( Line line in control.DeviceMonitor.GetDeviceState().Lines )
					foreach( Call call in line.Calls )
						if( call == callState )
							return control.DeviceMonitor.GetDeviceState();
			throw new DomainObjectNotFoundException( "Couldn't find a device owning the specified call." );
		}
		
		public Line GetLine( Call callState ) {
			foreach( DeviceMonitorControl control in DeviceMonitorControls )
				foreach( Line line in control.DeviceMonitor.GetDeviceState().Lines )
					foreach( Call call in line.Calls )
						if( call == callState )
							return line;
			throw new DomainObjectNotFoundException( "Couldn't find a line owning the specified call." );
		}
    }
}
