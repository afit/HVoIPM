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
    public class TAPIDeviceStateMonitor : DeviceStateMonitor
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

        public TAPIDeviceStateMonitor(String name) {
            mName = name;

            // We know there will one line for the device, which may have up to 10 calls. For now we will just support a single call.
            mDeviceState = new DeviceState(name, new LineState[] {
				new LineState( "1", new CallState[] { new CallState( "1" ) } ),
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

            m_CTapi = new CTapi(m_friendlyAppName, m_TapiVersion, m_AppHandle, CTapi.LineInitializeExOptions.LINEINITIALIZEEXOPTION_USEHIDDENWINDOW);

            m_CTapi.AppNewCall += new CTapi.AppNewCallEventHandler(MyAppNewCallEventHandler);
            m_CTapi.CallStateEvent += new CTapi.CallStateEventHandler(MyCallStateEventHandler);
            m_CTapi.LineReplyEvent += new CTapi.LineReplyEventHandler(MyLineReplyEventHandler);
            m_CTapi.LineCallInfoEvent += new CTapi.LineCallInfoEventHandler(MyLineCallInfoEventHandler);
            m_CTapi.LineAddressStateEvent += new CTapi.LineAddressStateEventHandler(MyLineAddressStateEventHandler);
            m_CTapi.LineDevStateEvent += new CTapi.LineDevStateEventHandler(MyLineDevStateEventHandler);
            m_CTapi.LineCloseEvent += new CTapi.LineCloseEventHandler(MyLineCloseEventHandler);

            string sLineFilter;
            sLineFilter = "Nortel";
            m_CLine = m_CTapi.GetLineByFilter(sLineFilter, false,
                CTapi.LineCallPrivilege.LINECALLPRIVILEGE_OWNER | CTapi.LineCallPrivilege.LINECALLPRIVILEGE_MONITOR);
            while (true) {
                Thread.Sleep(50);
            }
        }

        protected virtual void AnalyseLineState(String page, LineState lineState, IList<StateChange> changes) {

        }

        protected virtual void AnalyseCallState(String page, CallState callState, LineState lineState, IList<StateChange> changes) {
        }

        #region Event Handling

        public void MyAppNewCallEventHandler(Object sender, CTapi.AppNewCallEventArgs e) {
            CCall call = e.call;
            m_ActiveCall = call;
            m_ActiveCall.GetCallInfo();
            mDeviceState.LineStates[0].CallStates[0].Activity = Activity.InboundRinging;
        }

        public void MyLineCallInfoEventHandler(Object sender, CTapi.LineCallInfoEventArgs e) {
            if (m_ActiveCall != null) {
                if (((uint)e.LineCallInfoState & (uint)CTapi.LineCallInfoState.LINECALLINFOSTATE_CALLERID) > 1) {
                    m_ActiveCall.GetCallInfo();
                    if (mDeviceState.LineStates[0].CallStates[0].Activity == Activity.InboundRinging) {
                        mDeviceState.LineStates[0].LastCallerNumber = m_ActiveCall.CallerID.ToString();
                        IList<StateChange> changes = new List<StateChange>();
                        changes.Add(new CallStateChange(mDeviceState.LineStates[0].CallStates[0], "lastCallerNumber", "", m_ActiveCall.CallerID.ToString()));
                        // FIXME uncomment this StateManager.Instance().DeviceStateUpdated(this, changes);
                    }
                }
            }
        }

        public void MyCallStateEventHandler(Object sender, CTapi.CallStateEventArgs e) {

            switch (e.CallState) {
                case CTapi.LineCallState.LINECALLSTATE_CONNECTED:
                    Activity activity = mDeviceState.LineStates[0].CallStates[0].Activity;
                    if (activity == Activity.InboundRinging) {
                        activity = Activity.Inbound;
                    } else if ((activity == Activity.Dialing) || (activity == Activity.OutboundRinging)) {
                        activity = Activity.Outbound;
                    }
                    mDeviceState.LineStates[0].CallStates[0].Duration = "0";
                    break;
                case CTapi.LineCallState.LINECALLSTATE_DISCONNECTED:
                    mDeviceState.LineStates[0].CallStates[0].Activity = Activity.IdleDisconnected;
                    break;
                case CTapi.LineCallState.LINECALLSTATE_ONHOLD:
                    mDeviceState.LineStates[0].CallStates[0].Activity = Activity.Held;
                    break;
                case CTapi.LineCallState.LINECALLSTATE_IDLE:
                    mDeviceState.LineStates[0].CallStates[0].Activity = Activity.IdleDisconnected;
                    break;
                case CTapi.LineCallState.LINECALLSTATE_PROCEEDING:
                    break;
                case CTapi.LineCallState.LINECALLSTATE_OFFERING:
                    break;
                case CTapi.LineCallState.LINECALLSTATE_ACCEPTED:
                    if (mDeviceState.LineStates[0].CallStates[0].Activity != Activity.InboundRinging) {
                        mDeviceState.LineStates[0].CallStates[0].Activity = Activity.InboundRinging;
                    }
                    break;
                case CTapi.LineCallState.LINECALLSTATE_DIALING:
                    mDeviceState.LineStates[0].CallStates[0].Activity = Activity.Dialing;
                    break;
                case CTapi.LineCallState.LINECALLSTATE_RINGBACK:
                    mDeviceState.LineStates[0].CallStates[0].Activity = Activity.OutboundRinging;
                    break;
                case CTapi.LineCallState.LINECALLSTATE_BUSY:
                    mDeviceState.LineStates[0].CallStates[0].Activity = Activity.Busy;
                    break;
                case CTapi.LineCallState.LINECALLSTATE_SPECIALINFO:
                    break;
            }
        }

        public void MyLineReplyEventHandler(Object sender, CTapi.LineReplyEventArgs e) {
        }

        public void MyLineAddressStateEventHandler(Object sender, CTapi.LineAddressStateEventArgs e) {
        }

        public void MyLineDevStateEventHandler(Object sender, CTapi.LineDevStateEventArgs e) {
        }

        public void MyLineCloseEventHandler(Object sender, CTapi.LineCloseEventArgs e) {
            mDeviceState.LineStates[0].RegistrationState = RegistrationState.Offline;
        }

        #endregion // Event Handling
    
    }
}
