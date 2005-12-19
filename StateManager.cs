using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Reflection;

using LothianProductions.Util.Settings;
using LothianProductions.VoIP.Monitor;
using LothianProductions.VoIP.Monitor.Impl;
using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP {

	public class StateUpdateEventArgs : EventArgs {
		public StateUpdateEventArgs() {
		}        
	}
	
	public delegate void StateUpdateHandler( DeviceStateMonitor monitor, StateUpdateEventArgs e );

	/// <summary>
	/// Doesn't handle multiple or custom device monitors yet.
	/// </summary>
    public class StateManager {	
		protected static readonly StateManager mInstance = new StateManager();
		
		public static StateManager Instance() {
			return mInstance;
		}
		
		public event StateUpdateHandler StateUpdate;
		protected Dictionary<DeviceStateMonitor, Thread> mDeviceStateMonitors = new Dictionary<DeviceStateMonitor,Thread>();
    
		protected StateManager() {
			// Initialize device monitors, have to give each its own thread.
			ReloadDeviceStateMonitors();
		}

		public void ReloadDeviceStateMonitors() {
			lock (mDeviceStateMonitors) {
				// Stop currently running threads.
				foreach( DeviceStateMonitor monitor in mDeviceStateMonitors.Keys )
					mDeviceStateMonitors[ monitor ].Abort();
					
				mDeviceStateMonitors.Clear();

				NameValueCollection config = (NameValueCollection) ConfigurationManager.GetSection( "deviceStateMonitors" );

				if( config == null )
					throw new MissingAppSettingsException( "The section \"deviceStateMonitors\" was not present in the application configuration." );

				foreach( String key in config.Keys ) {				
					Type type = Type.GetType( config[ key ], true );

					DeviceStateMonitor monitor = (DeviceStateMonitor) type.InvokeMember(
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
			}
		}
		
		public IList<DeviceStateMonitor> DeviceStateMonitors {
			// FIXME this is very inefficient
			get{ return new List<DeviceStateMonitor>( mDeviceStateMonitors.Keys ); }
		}

		public void DeviceStateUpdated( DeviceStateMonitor monitor, IList<IStateChange> changes ) {
			if( StateUpdate != null )
				StateUpdate( monitor, new StateUpdateEventArgs() );
				
			// Clever logging stuff here.
		}
    }
}
