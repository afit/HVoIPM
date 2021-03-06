using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

using LothianProductions.Util;
using LothianProductions.Util.Http;
using LothianProductions.VoIP.Monitor;
using LothianProductions.VoIP.State;

using Tapi2Lib;

namespace LothianProductions.VoIP.Monitor.Impl
{

    /// <summary>
    /// Implementation of DeviceMonitor for the Linksys PAP2
    /// VoIP adapter device. As there's no neat way to get into
    /// the thing, this implementation works just by HTML scraping.
    /// </summary>
    public class TAPIDeviceMonitor : DeviceMonitor
    {
        private const uint MAXASYNCCALLS = (uint)10;			// TAPI2_ADDITIONS

        /// <summary>
        /// application friendly name to pass to LineInitializeEx
        /// </summary>
        private string m_friendlyAppName;

        /// <summary>
        /// number of devices, returned by LineInitializeEx
        /// </summary>
        private uint m_numberDevices;

        /// <summary>
        /// TAPI Version -- default to 2
        /// </summary>
        private uint m_TapiVersion;

        /// <summary>
        /// Handle of calling application or window
        /// </summary>
        private System.IntPtr m_AppHandle;

        /// <summary>
        /// Tapi Object
        /// </summary>
        private CTapi m_CTapi;

        // holds call handles for valid asynch calls
        private IntPtr[] ma_hCalls = new IntPtr[MAXASYNCCALLS];	// TAPI2_ADDITIONS
        private CCall m_ActiveCall;
        private CLine m_CLine;

        
        public TAPIDeviceMonitor(String name) {
            mName = name;

            // We know there will one line for the device, which may have up to 10 calls. For now we will just support a single call.
            mDeviceState = new Device(name, new Line[] {
				new Line( "1", new Call[] { new Call( "1" ) } ),
			});
        }

        public override void Run() {
            // TAPI2_ADDITIONS
            for (int i = 0; i < MAXASYNCCALLS; i++) {
                ma_hCalls[i] = IntPtr.Zero;
            }
            // TAPI2_ADDITIONS.

            // provide default initialization for Tapi members
            m_friendlyAppName = "CTapi";
            //m_AppHandle = this.Handle;
            m_AppHandle = System.IntPtr.Zero;
            m_TapiVersion = 0x00020000;

            m_CTapi = new CTapi(m_friendlyAppName, m_TapiVersion, m_AppHandle, CTapi.LineInitializeExOptions.LINEINITIALIZEEXOPTION_USEEVENT);

            m_CTapi.AppNewCall += new CTapi.AppNewCallEventHandler(this.MyAppNewCallEventHandler);
            m_CTapi.CallStateEvent += new CTapi.CallStateEventHandler(this.MyCallStateEventHandler);
            m_CTapi.LineReplyEvent += new CTapi.LineReplyEventHandler(this.MyLineReplyEventHandler);
            m_CTapi.LineCallInfoEvent += new CTapi.LineCallInfoEventHandler(this.MyLineCallInfoEventHandler);
            m_CTapi.LineAddressStateEvent += new CTapi.LineAddressStateEventHandler(this.MyLineAddressStateEventHandler);
            m_CTapi.LineDevStateEvent += new CTapi.LineDevStateEventHandler(this.MyLineDevStateEventHandler);
            m_CTapi.LineCloseEvent += new CTapi.LineCloseEventHandler(this.MyLineCloseEventHandler);

            string sLineFilter;
            sLineFilter = GetConfigurationValue( "Provider" );
            if (sLineFilter == null)
                throw new DeviceNotRespondingException("Unable to identify TAPI provider - check config file?", new ApplicationException());
            m_CLine = m_CTapi.GetLineByFilter(sLineFilter, false,
                CTapi.LineCallPrivilege.LINECALLPRIVILEGE_OWNER | CTapi.LineCallPrivilege.LINECALLPRIVILEGE_MONITOR);
            if (m_CLine == null) {
                throw new DeviceNotRespondingException("Unable to identify TAPI line for provider #" + sLineFilter, new ApplicationException());
            }
            while (true) {
                Thread.Sleep(500);
            }
        }

        #region Event Handling

        public void MyAppNewCallEventHandler(Object sender, CTapi.AppNewCallEventArgs e) {
            mDeviceState.Lines[0].Calls[0].Name = "Call #" + e.call.CallID.ToString();
            m_ActiveCall = e.call;
        }

        public void MyLineCallInfoEventHandler(Object sender, CTapi.LineCallInfoEventArgs e) {
            IList<DevicePropertyChange> deviceChanges = new List<DevicePropertyChange>();
            IList<LinePropertyChange> lineChanges = new List<LinePropertyChange>();
            IList<CallPropertyChange> callChanges = new List<CallPropertyChange>();
            if (m_ActiveCall != null ) {
                if (((uint)e.LineCallInfoState & (uint)CTapi.LineCallInfoState.LINECALLINFOSTATE_CALLERID) > 1) {
                    m_ActiveCall.GetCallInfo();
                    if (mDeviceState.Lines[0].Calls[0].Type == CallType.Inbound ) {
                        string oldCallerId = mDeviceState.Lines[0].LastCallerNumber;
                        string newCallerId = m_ActiveCall.CallerID.ToString();
                        if (oldCallerId != newCallerId) {
                            lineChanges.Add(new LinePropertyChange(mDeviceState.Lines[0], PROPERTY_LINE_LASTCALLERNUMBER, oldCallerId, newCallerId));
                            StateManager.Instance().DeviceUpdated(this, deviceChanges, lineChanges, callChanges);
                        }
                        mDeviceState.Lines[0].LastCallerNumber = newCallerId;
                    }
                }
            }
			if( deviceChanges.Count > 0 || lineChanges.Count > 0 || callChanges.Count > 0 )
				StateManager.Instance().DeviceUpdated( this, deviceChanges, lineChanges, callChanges );
        }

        public void MyCallStateEventHandler(Object sender, CTapi.CallStateEventArgs e) {

            IList<DevicePropertyChange> deviceChanges = new List<DevicePropertyChange>();
            IList<LinePropertyChange> lineChanges = new List<LinePropertyChange>();
            IList<CallPropertyChange> callChanges = new List<CallPropertyChange>();
            Activity activity;
            Activity oldActivity;
            Tone tone;
            Tone oldTone;
            switch (e.CallState) {
                case CTapi.LineCallState.LINECALLSTATE_CONNECTED:
                    activity = mDeviceState.Lines[0].Calls[0].Activity;
                    oldActivity = activity;
                    oldTone = mDeviceState.Lines[0].Calls[0].Tone;
                    if (activity == Activity.Ringing || activity == Activity.Dialing) {
                        activity = Activity.Connected;
                        mDeviceState.Lines[0].Calls[0].Activity = activity;
                        mDeviceState.Lines[0].Calls[0].Tone = Tone.Call;
                        tone = mDeviceState.Lines[0].Calls[0].Tone;
                        if ( tone != oldTone ) 
                            callChanges.Add( new CallPropertyChange( mDeviceState.Lines[0].Calls[0], PROPERTY_CALL_TONE, oldTone.ToString(), tone.ToString() ) );
                    }
                    callChanges.Add(new CallPropertyChange(mDeviceState.Lines[0].Calls[0], PROPERTY_CALL_ACTIVITY, oldActivity.ToString(), activity.ToString()));
                    mDeviceState.Lines[0].Calls[0].Duration = "0";
                    break;
                case CTapi.LineCallState.LINECALLSTATE_DISCONNECTED:
                    oldActivity = mDeviceState.Lines[0].Calls[0].Activity;
                    mDeviceState.Lines[0].Calls[0].Activity = Activity.IdleDisconnected;
                    mDeviceState.Lines[0].Calls[0].Tone = Tone.None;
                    oldTone = mDeviceState.Lines[0].Calls[0].Tone;
                    tone = mDeviceState.Lines[0].Calls[0].Tone;
                    callChanges.Add(new CallPropertyChange(mDeviceState.Lines[0].Calls[0], PROPERTY_CALL_ACTIVITY, oldActivity.ToString(), "IdleDisconnected"));
                    if (tone != oldTone)
                        callChanges.Add(new CallPropertyChange(mDeviceState.Lines[0].Calls[0], PROPERTY_CALL_TONE, oldTone.ToString(), tone.ToString()));
                    break;
                case CTapi.LineCallState.LINECALLSTATE_ONHOLD:
                    oldActivity = mDeviceState.Lines[0].Calls[0].Activity;
                    mDeviceState.Lines[0].Calls[0].Activity = Activity.Held;
                    callChanges.Add(new CallPropertyChange(mDeviceState.Lines[0].Calls[0], PROPERTY_CALL_ACTIVITY, oldActivity.ToString(), "Held"));
                    break;
                case CTapi.LineCallState.LINECALLSTATE_IDLE:
                    oldActivity = mDeviceState.Lines[0].Calls[0].Activity;
                    oldTone = mDeviceState.Lines[0].Calls[0].Tone;
                    mDeviceState.Lines[0].Calls[0].Activity = Activity.IdleDisconnected;
                    mDeviceState.Lines[0].Calls[0].Tone = Tone.None;
                    tone = mDeviceState.Lines[0].Calls[0].Tone;
                    callChanges.Add(new CallPropertyChange(mDeviceState.Lines[0].Calls[0], PROPERTY_CALL_ACTIVITY, oldActivity.ToString(), "IdleDisconnected"));
                    if (tone != oldTone)
                        callChanges.Add(new CallPropertyChange(mDeviceState.Lines[0].Calls[0], PROPERTY_CALL_TONE, oldTone.ToString(), tone.ToString()));
                    break;
                case CTapi.LineCallState.LINECALLSTATE_PROCEEDING:
                    CCall call = e.call;
                    oldActivity = mDeviceState.Lines[0].Calls[0].Activity;
                    mDeviceState.Lines[0].Calls[0].Activity = Activity.Dialing;
                    mDeviceState.Lines[0].Calls[0].Tone = Tone.Dial;
                    mDeviceState.Lines[0].Calls[0].Type = CallType.Outbound;
                    mDeviceState.Lines[0].LastCalledNumber = call.CalledID.ToString();
                    callChanges.Add(new CallPropertyChange(mDeviceState.Lines[0].Calls[0], PROPERTY_CALL_ACTIVITY, "IdleDisconnected", "Dialing"));
                    callChanges.Add(new CallPropertyChange(mDeviceState.Lines[0].Calls[0], PROPERTY_CALL_TYPE, "IdleDisconnected", "Outbound"));
                    lineChanges.Add(new LinePropertyChange(mDeviceState.Lines[0], PROPERTY_LINE_LASTCALLEDNUMBER, "", call.CalledID.ToString() ));
                    break;
                case CTapi.LineCallState.LINECALLSTATE_OFFERING:
                    mDeviceState.Lines[0].Calls[0].Activity = Activity.Ringing;
                    mDeviceState.Lines[0].Calls[0].Tone = Tone.None;
                    mDeviceState.Lines[0].Calls[0].Type = CallType.Inbound;
                    callChanges.Add(new CallPropertyChange(mDeviceState.Lines[0].Calls[0], PROPERTY_CALL_ACTIVITY, "IdleDisconnected", "Ringing"));
                    callChanges.Add(new CallPropertyChange(mDeviceState.Lines[0].Calls[0], PROPERTY_CALL_TYPE, "IdleDisconnected", "Inbound"));
                    break;
                case CTapi.LineCallState.LINECALLSTATE_ACCEPTED:
                    if (mDeviceState.Lines[0].Calls[0].Activity != Activity.Ringing) {
                        oldActivity = mDeviceState.Lines[0].Calls[0].Activity;
                        mDeviceState.Lines[0].Calls[0].Activity = Activity.Ringing;
                        callChanges.Add(new CallPropertyChange(mDeviceState.Lines[0].Calls[0], PROPERTY_CALL_ACTIVITY, oldActivity.ToString(), "Ringing"));
                    }
                    break;
                case CTapi.LineCallState.LINECALLSTATE_DIALING:
                    oldActivity = mDeviceState.Lines[0].Calls[0].Activity;
                    oldTone = mDeviceState.Lines[0].Calls[0].Tone;
                    mDeviceState.Lines[0].Calls[0].Tone = Tone.Dial;
                    tone = mDeviceState.Lines[0].Calls[0].Tone;
                    mDeviceState.Lines[0].Calls[0].Activity = Activity.Dialing;
                    callChanges.Add(new CallPropertyChange(mDeviceState.Lines[0].Calls[0], PROPERTY_CALL_ACTIVITY, oldActivity.ToString(), "Dialing"));
                    if ( tone != oldTone )
                        callChanges.Add(new CallPropertyChange(mDeviceState.Lines[0].Calls[0], PROPERTY_CALL_TONE, oldTone.ToString(), tone.ToString()));
                    break;
                case CTapi.LineCallState.LINECALLSTATE_RINGBACK:
                    oldActivity = mDeviceState.Lines[0].Calls[0].Activity;
                    mDeviceState.Lines[0].Calls[0].Activity = Activity.Ringing;
                    callChanges.Add(new CallPropertyChange(mDeviceState.Lines[0].Calls[0], PROPERTY_CALL_ACTIVITY, oldActivity.ToString(), "Ringing"));
                    break;
                case CTapi.LineCallState.LINECALLSTATE_BUSY:
                    oldTone = mDeviceState.Lines[0].Calls[0].Tone;
                    mDeviceState.Lines[0].Calls[0].Tone = Tone.Busy;
                    tone = mDeviceState.Lines[0].Calls[0].Tone;
                    if ( tone != oldTone ) 
                        callChanges.Add(new CallPropertyChange(mDeviceState.Lines[0].Calls[0], PROPERTY_CALL_TONE, oldTone.ToString(), tone.ToString() ));
                    break;
                case CTapi.LineCallState.LINECALLSTATE_SPECIALINFO:
                    break;
            }
            if (deviceChanges.Count > 0 || lineChanges.Count > 0 || callChanges.Count > 0)
                StateManager.Instance().DeviceUpdated(this, deviceChanges, lineChanges, callChanges);
        }

        public void MyLineReplyEventHandler(Object sender, CTapi.LineReplyEventArgs e) {
        }

        public void MyLineAddressStateEventHandler(Object sender, CTapi.LineAddressStateEventArgs e) {
        }

        public void MyLineDevStateEventHandler(Object sender, CTapi.LineDevStateEventArgs e) {
        }

        public void MyLineCloseEventHandler(Object sender, CTapi.LineCloseEventArgs e) {
            IList<DevicePropertyChange> deviceChanges = new List<DevicePropertyChange>();
            IList<LinePropertyChange> lineChanges = new List<LinePropertyChange>();
            IList<CallPropertyChange> callChanges = new List<CallPropertyChange>();
            mDeviceState.Lines[0].RegistrationState = RegistrationState.Offline;
            lineChanges.Add(new LinePropertyChange(mDeviceState.Lines[0], PROPERTY_LINE_REGISTRATIONSTATE, "Online", "Offline"));
            if (deviceChanges.Count > 0 || lineChanges.Count > 0 || callChanges.Count > 0)
                StateManager.Instance().DeviceUpdated(this, deviceChanges, lineChanges, callChanges);
        }

        #endregion // Event Handling
    
    }
}
