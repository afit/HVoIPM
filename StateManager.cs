using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using LothianProductions.VoIP.Monitor;
using LothianProductions.VoIP.Monitor.Impl;
using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP {

	public class StateUpdateEventArgs : EventArgs {
		public StateUpdateEventArgs() {
		}        
	}
	
	public delegate void StateUpdateHandler( DeviceMonitor monitor, StateUpdateEventArgs e );

	/// <summary>
	/// Doesn't handle multiple or custom device monitors yet.
	/// </summary>
    public class StateManager {	
		private static readonly StateManager mInstance = new StateManager();
		
		public static StateManager Instance() {
			return mInstance;
		}
		
		public event StateUpdateHandler StateUpdate;
    
		private StateManager() {
			// Initialize device monitors, have to give each its own thread.
			new Thread( new ThreadStart( new LinksysPAP2DeviceMonitor().Run ) ).Start();
		}
				
		public void DeviceStateUpdated( DeviceMonitor monitor ) {
			if( StateUpdate != null )
				StateUpdate( monitor, new StateUpdateEventArgs() );
		}
    }
}
