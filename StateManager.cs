using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Reflection;

using LothianProductions.Data;
using LothianProductions.Util.Settings;
using LothianProductions.VoIP.Monitor;
using LothianProductions.VoIP.Monitor.Impl;
using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP {

	public class StateUpdateEventArgs : EventArgs {
		public StateUpdateEventArgs(	IList<DeviceChange> deviceChanges, IList<LineChange> lineChanges,
										IList<CallChange> callChanges ) {
			mDeviceStateChanges = deviceChanges;
			mLineStateChanges = lineChanges;
			mCallStateChanges = callChanges;
		}

		protected IList<DeviceChange> mDeviceStateChanges;
		public IList<DeviceChange> DeviceStateChanges {
			get{ return mDeviceStateChanges; }
		}

		protected IList<LineChange> mLineStateChanges;
		public IList<LineChange> LineStateChanges {
			get{ return mLineStateChanges; }
		}

		protected IList<CallChange> mCallStateChanges;
		public IList<CallChange> CallStateChanges {
			get{ return mCallStateChanges; }
		}
	}
	
	public delegate void StateUpdateHandler( IDeviceStateMonitor monitor, StateUpdateEventArgs e );

	/// <summary>
	/// Doesn't handle multiple or custom device monitors yet.
	/// </summary>
    public class StateManager {	
		protected static readonly StateManager mInstance = new StateManager();

        protected Dictionary<Call, CallRecord> mCalls = new Dictionary<Call, CallRecord>();
		
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
			if( StateUpdate != null )
				StateUpdate( monitor, new StateUpdateEventArgs( deviceChanges, lineChanges, callChanges ) );

			// Clever logging stuff here.
            foreach( CallChange change in callChanges ) {
                if ( change.Property == "activity" ) {
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
					} else if ( change.Call.Activity != Activity.IdleDisconnected ) {
						CallRecord call = mCalls[change.Call];
						call.Call = change.Call;
						call.Line = GetLine(change.Call);
					}
                }
            }
			foreach (DeviceChange change in deviceChanges) {
				Logger.Instance().Log( "Device property " + change.Property + " has changed from " + change.ChangedFrom + " to " + change.ChangedTo );
			}
			foreach (LineChange change in lineChanges) {
				Logger.Instance().Log( "Line property " + change.Property + " has changed from " + change.ChangedFrom + " to " + change.ChangedTo );
			}
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
