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
		public StateUpdateEventArgs(	IList<DeviceStateChange> deviceChanges, IList<LineStateChange> lineChanges,
										IList<CallStateChange> callChanges ) {
			mDeviceStateChanges = deviceChanges;
			mLineStateChanges = lineChanges;
			mCallStateChanges = callChanges;
		}

		protected IList<DeviceStateChange> mDeviceStateChanges;
		public IList<DeviceStateChange> DeviceStateChanges {
			get{ return mDeviceStateChanges; }
		}

		protected IList<LineStateChange> mLineStateChanges;
		public IList<LineStateChange> LineStateChanges {
			get{ return mLineStateChanges; }
		}

		protected IList<CallStateChange> mCallStateChanges;
		public IList<CallStateChange> CallStateChanges {
			get{ return mCallStateChanges; }
		}
	}
	
	public delegate void StateUpdateHandler( IDeviceStateMonitor monitor, StateUpdateEventArgs e );

	/// <summary>
	/// Doesn't handle multiple or custom device monitors yet.
	/// </summary>
    public class StateManager {	
		protected static readonly StateManager mInstance = new StateManager();

        protected Dictionary<CallState, Call> mCalls = new Dictionary<CallState, Call>();
		
		public static StateManager Instance() {
			return mInstance;
		}
		
		public event StateUpdateHandler StateUpdate;
		protected Dictionary<IDeviceStateMonitor, Thread> mDeviceStateMonitors = new Dictionary<IDeviceStateMonitor,Thread>();
    
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

		public void DeviceStateUpdated(	IDeviceStateMonitor monitor, IList<DeviceStateChange> deviceChanges,
										IList<LineStateChange> lineChanges, IList<CallStateChange> callChanges ) {
			if( StateUpdate != null )
				StateUpdate( monitor, new StateUpdateEventArgs( deviceChanges, lineChanges, callChanges ) );

			// Clever logging stuff here.
            foreach( CallStateChange change in callChanges ) {
                if ( change.Property == "activity" ) {
                    if ( change.ChangedFrom == Activity.IdleDisconnected.ToString() && change.ChangedTo != Activity.IdleDisconnected.ToString() ) {
                        Call call = new Call(
							monitor.GetDeviceState().Name, change.CallState, change.ChangedTo,
							GetLineState( change.CallState ).LastCalledNumber, DateTime.Now,
							new DateTime(), null
						);
						
						// FIXME add sanity check in case somehow call is already there
                        mCalls.Add( change.CallState, call );
                    } else if ( change.ChangedFrom != Activity.IdleDisconnected.ToString() && change.ChangedTo == Activity.IdleDisconnected.ToString() ) {
						//LineState lineState = GetLineState( change.CallState );
                        Call call = mCalls[ change.CallState ];
                        call.EndTime = DateTime.Now;
                        call.Duration = change.CallState.Duration;
                        CallLogger.Instance().Log( call );
                    }
                }
            }
		}
		
		// Helper functions for linking states.
		public IDeviceStateMonitor GetMonitor( DeviceState deviceState ) {
			foreach( IDeviceStateMonitor monitor in DeviceStateMonitors )
				if( monitor.GetDeviceState() == deviceState )
					return monitor;
			throw new DomainObjectNotFoundException( "Couldn't find a monitor owning the specified device." );
		}

		public IDeviceStateMonitor GetMonitor( LineState lineState ) {
			foreach( IDeviceStateMonitor monitor in DeviceStateMonitors )
				foreach( LineState line in monitor.GetDeviceState().LineStates )
					if( line == lineState )
						return monitor;
			throw new DomainObjectNotFoundException( "Couldn't find a monitor owning the specified line." );
		}

		public IDeviceStateMonitor GetMonitor( CallState callState ) {
			foreach( IDeviceStateMonitor monitor in DeviceStateMonitors )
				foreach( LineState line in monitor.GetDeviceState().LineStates )
					foreach( CallState call in line.CallStates )
						if( call == callState )
							return monitor;
			throw new DomainObjectNotFoundException( "Couldn't find a monitor owning the specified call." );
		}
		
		public DeviceState GetDeviceState( LineState lineState ) {
			foreach( IDeviceStateMonitor monitor in DeviceStateMonitors )
				foreach( LineState line in monitor.GetDeviceState().LineStates )
					if( line == lineState )
						return monitor.GetDeviceState();
			throw new DomainObjectNotFoundException( "Couldn't find a device owning the specified line." );
		}
		
		public DeviceState GetDeviceState( CallState callState ) {
			foreach( IDeviceStateMonitor monitor in DeviceStateMonitors )
				foreach( LineState line in monitor.GetDeviceState().LineStates )
					foreach( CallState call in line.CallStates )
						if( call == callState )
							return monitor.GetDeviceState();
			throw new DomainObjectNotFoundException( "Couldn't find a device owning the specified call." );
		}
		
		public LineState GetLineState( CallState callState ) {
			foreach( IDeviceStateMonitor monitor in DeviceStateMonitors )
				foreach( LineState line in monitor.GetDeviceState().LineStates )
					foreach( CallState call in line.CallStates )
						if( call == callState )
							return line;
			throw new DomainObjectNotFoundException( "Couldn't find a line owning the specified call." );
		}
    }
}
