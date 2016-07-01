using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace RDPcontrol
{
    public partial class RemoteDeskTopControl : UserControl
    {


        #region out going events and support classes
        public enum DisconnectReason : int
        {
            SocketClosed = 2308,
            ByServer = 3,
            ClientDecompressionError = 3080,
            ConnectionTimedOut = 264,
            DecryptionError = 3078,
            DNSLookupFailed = 260,
            DNSLookupFailed2 = 1288,
            EncryptionError = 2822,
            GetHostByNameFailed = 1540,
            HostNotFound = 520,
            InternalError = 1032,
            InternalSecurityError = 2310,
            InternalSecurityError2 = 2566,
            InvalidEncryption = 1286,
            InvalidIP = 2052,
            InvalidServerSecurityInfo = 1542,
            InvalidSecurityData = 1030,
            InvalidIPAddr = 776,
            LicensingFailed = 2056,
            LicensingTimeout = 2312,
            LocalNotError = 1,
            NoInfo = 0,
            OutOfMemory = 262,
            OutOfMemory2 = 518,
            OutOfMemory3 = 774,
            RemoteByUser = 2,
            ServerCertificateUnpackErr = 1798,
            SocketConnectFailed = 516,
            SocketRecvFailed = 1028,
            TimeoutOccurred = 1796,
            TimerError = 1544,
            WinsockSendFailed = 772,
            DISABLED = 2823,
            EXPIRED = 3591,
            LOCKED_OUT = 3335,
            RESTRICTION = 3079,
            IRED = 6919,
            ON_POLICY = 5639,
            ED_REQUIRED_BY_SERVER = 8455,
            ILURE = 2055,
            NTICATING_AUTHORITY = 6151,
            USER = 2567,
            _EXPIRED = 3847,
            _MUST_CHANGE = 4615,
            TLM_ONLY = 5895,
            D_CARD_BLOCKED = 8711,
            D_WRONG_PIN = 7175
        }

        /// <summary>
        /// class to hold the disconnect reason for the disconnect event
        /// </summary>
        public class DisconnectEventArgs : EventArgs
        {
            public DisconnectReason Reason;
            public DisconnectEventArgs(int reason)
            {
                Reason = (DisconnectReason)reason;
            }             
        }
        /// <summary>
        /// OnDisconnect handler delegate
        /// </summary>
        /// <param name="sender">the object that fired the event</param>
        /// <param name="args">the reason for the disconnect</param>
        public delegate void DisconnectedHandler(object sender, DisconnectEventArgs args);

        /// <summary>
        /// Fired on disconnecting from the remote host
        /// </summary>
        public event DisconnectedHandler OnDisconnected;




        public event EventHandler OnAuthenticationWarningDismissed
        {
            add { axRdpClient.OnAuthenticationWarningDismissed += value; }
            remove { axRdpClient.OnAuthenticationWarningDismissed -= value; }
        }

        public event EventHandler OnAuthenticationWarningDisplayed
        {
            add { axRdpClient.OnAuthenticationWarningDisplayed += value; }
            remove { axRdpClient.OnAuthenticationWarningDisplayed -= value; }
        }

        public event EventHandler OnAutoReconnected
        {
            add { axRdpClient.OnAutoReconnected += value; }
            remove { axRdpClient.OnAutoReconnected -= value; }
        }
        public event IMsTscAxEvents_OnAutoReconnectingEventHandler OnAutoReconnecting;
        public event IMsTscAxEvents_OnAutoReconnecting2EventHandler OnAutoReconnecting2;
        public event IMsTscAxEvents_OnChannelReceivedDataEventHandler OnChannelReceivedData;
        public event IMsTscAxEvents_OnConfirmCloseEventHandler OnConfirmClose;


        public event EventHandler OnConnected
        {
            add { axRdpClient.OnConnected += value; }
            remove { axRdpClient.OnConnected -= value; }
        }
        public event EventHandler OnConnecting
        {
            add { axRdpClient.OnConnecting += value; }
            remove { axRdpClient.OnConnecting -= value; }
        }
        public event EventHandler OnConnectionBarPullDown
        {
            add { axRdpClient.OnConnectionBarPullDown += value; }
            remove { axRdpClient.OnConnectionBarPullDown -= value; }
        }

        public event EventHandler OnDevicesButtonPressed
        {
            add { axRdpClient.OnDevicesButtonPressed += value; }
            remove { axRdpClient.OnDevicesButtonPressed -= value; }
        }

        public event EventHandler OnEnterFullScreenMode
        {
            add { axRdpClient.OnEnterFullScreenMode += value; }
            remove { axRdpClient.OnEnterFullScreenMode -= value; }
        }
        public event IMsTscAxEvents_OnFatalErrorEventHandler OnFatalError;
        public event IMsTscAxEvents_OnFocusReleasedEventHandler OnFocusReleased;

        public event EventHandler OnIdleTimeoutNotification
        {
            add { axRdpClient.OnIdleTimeoutNotification += value; }
            remove { axRdpClient.OnIdleTimeoutNotification -= value; }
        }
        public event EventHandler OnLeaveFullScreenMode
        {
            add { axRdpClient.OnLeaveFullScreenMode += value; }
            remove { axRdpClient.OnLeaveFullScreenMode -= value; }
        }

        public event EventHandler OnLoginComplete
        {
            add { axRdpClient.OnLoginComplete += value; }
            remove { axRdpClient.OnLoginComplete -= value; }
        }

        public event IMsTscAxEvents_OnLogonErrorEventHandler OnLogonError;
        public event IMsTscAxEvents_OnMouseInputModeChangedEventHandler OnMouseInputModeChanged;
        public event IMsTscAxEvents_OnNetworkStatusChangedEventHandler OnNetworkStatusChanged;
        public event IMsTscAxEvents_OnReceivedTSPublicKeyEventHandler OnReceivedTSPublicKey;
        public event IMsTscAxEvents_OnRemoteDesktopSizeChangeEventHandler OnRemoteDesktopSizeChange;
        public event IMsTscAxEvents_OnRemoteProgramDisplayedEventHandler OnRemoteProgramDisplayed;
        public event IMsTscAxEvents_OnRemoteProgramResultEventHandler OnRemoteProgramResult;
        public event IMsTscAxEvents_OnRemoteWindowDisplayedEventHandler OnRemoteWindowDisplayed;
        public event EventHandler OnRequestContainerMinimize
        {
            add { axRdpClient.OnRequestContainerMinimize += value; }
            remove { axRdpClient.OnRequestContainerMinimize -= value; }
        }
        public event EventHandler OnRequestGoFullScreen
        {
            add { axRdpClient.OnRequestGoFullScreen += value; }
            remove { axRdpClient.OnRequestGoFullScreen -= value; }
        }
        public event EventHandler OnRequestLeaveFullScreen
        {
            add { axRdpClient.OnRequestLeaveFullScreen += value; }
            remove { axRdpClient.OnRequestLeaveFullScreen -= value; }
        }
        public event IMsTscAxEvents_OnServiceMessageReceivedEventHandler OnServiceMessageReceived;
        public event IMsTscAxEvents_OnUserNameAcquiredEventHandler OnUserNameAcquired;
        public event IMsTscAxEvents_OnWarningEventHandler OnWarning;





        #endregion

        #region Public properties
        private const short RDP_CLIENT_DISCONNECTED = 0;
        private const short RDP_CLIENT_CONNECTED = 1;
        private const short RDP_CLIENT_CONNECTING = 2; 
        /// <summary>
        /// Test if there is currently a connected session
        /// </summary>
        public bool IsConnected { get { return axRdpClient != null && axRdpClient.Connected == RDP_CLIENT_CONNECTED; } } 

        /// <summary>
        /// Test if a session is currently being connected 
        /// </summary>
        public bool IsConnecting { get { return axRdpClient != null && axRdpClient.Connected == RDP_CLIENT_CONNECTED; } }


        #endregion


        /// <summary>
        /// default constructor
        /// </summary>
        public RemoteDeskTopControl()
        {
            InitializeComponent();

            axRdpClient.OnConnecting += axRdpClient_OnConnecting;
            axRdpClient.OnDisconnected += axRdpClient_OnDisconnected;
            axRdpClient.OnFatalError += AxRdpClient_OnFatalError;
            axRdpClient.OnAutoReconnecting += AxRdpClient_OnAutoReconnecting;
private MSTSCLib.AutoReconnectContinueState AxRdpClient_OnAutoReconnecting(object sender, AxMSTSCLib.IMsTscAxEvents_OnAutoReconnectingEvent e)
        {
            throw new NotImplementedException();
        }

        public event IMsTscAxEvents_OnAutoReconnecting2EventHandler OnAutoReconnecting2;
            public event IMsTscAxEvents_OnChannelReceivedDataEventHandler OnChannelReceivedData;
            public event IMsTscAxEvents_OnConfirmCloseEventHandler OnConfirmClose;


    }

    private void AxRdpClient_OnFatalError(object sender, AxMSTSCLib.IMsTscAxEvents_OnFatalErrorEvent e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initiates a connection using the properties currently set on the control.
        /// </summary>
        public void Connect()
        {
            axRdpClient.Connect();
        }

        /// <summary>
        /// Reconnects to the remote session with the new desktop width and height.
        /// </summary>
        /// <param name="width">The new desktop width, in pixels. </param>
        /// <param name="height">The new desktop height, in pixels. </param>
        public void Reconnect(uint width, uint height)
        {
            axRdpClient.Reconnect(width, height);
        }

        /// <summary>
        /// Requests a graceful shutdown of the Remote Desktop ActiveX control.
        /// </summary>
        /// <returns>
        /// true if the control has shutdown gracefully.
        /// false if you need to wait for a 
        /// </returns>
        public bool RequestClose()
        {

            MSTSCLib.ControlCloseStatus status = axRdpClient.RequestClose();
            return status == MSTSCLib.ControlCloseStatus.controlCloseCanProceed;
            {

            }
        }
        
        /// <summary>
        /// Disconnects the active connection.
        /// </summary>
        /// <remarks>Will throw if there is not currently a connection</remarks>
        public void Disconnect()
        {
            axRdpClient.Disconnect();
        }

        /// <summary>
        /// Retrieves the error description for the session disconnect events
        /// </summary>
        /// <param name="disconnectReason">The disconnect reason.</param>
        /// <param name="extendedDisconnectReason">Provides detailed information about why a disconnect was initiated.</param>
        /// <returns></returns>
        public string GetErrorDescription(uint disconnectReason, uint extendedDisconnectReason)
        {
            return axRdpClient.GetErrorDescription(disconnectReason, extendedDisconnectReason);
        }

        /// <summary>
        /// Retrieves the status text for the specified status code.
        /// </summary>
        /// <param name="statusCode">A unsigned int that specifies the status code to retrieve the text for.</param>
        /// <returns></returns>
        public string GetStatusText(uint statusCode)
        {
            return axRdpClient.GetStatusText(statusCode);
        }


        // Note: Virtiual channels will not be supported by this control

        // Creates a client-side virtual channel object for each specified virtual channel name.
        //    public void CreateVirtualChannels(string newVChannels)


        // Retrieves the options set for a virtual channel.
        //   GetVirtualChannelOptions
        // Sends data to the RD Session Host server over a virtual channel that was created previously by using the CreateVirtualChannels method.
        //   SendOnVirtualChannel

        // Causes an action to be performed in the remote session.
        //   SendRemoteAction

        // Sets the virtual channel options for the Remote Desktop ActiveX control.
        //   SetVirtualChannelOptions




        #region RDP client event handlers
        /// <summary>
        /// Event handler for OnConnecting events from the RDP client control
        /// </summary>
        /// <param name="sender">The RDP client control</param>
        /// <param name="e"> unused parameter</param>
        private void axRdpClient_OnConnecting(object sender, EventArgs e)
        {
            Debug.WriteLine("RemoteDeskTopControl.OnConnecting: EvnetArgs type {0}", e.ToString());
        }

        /// <summary>
        /// OnDisconnected event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>This is needed to translate the parameter</remarks>
        private void axRdpClient_OnDisconnected(object sender, AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEvent e)
        {
            Debug.WriteLine("OnDisconnected event");
            OnDisconnected(this, new DisconnectEventArgs(e.discReason));
        }
        #endregion

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            
            if(IsConnected)
            {


            }
        }

        private void axRdpClient_OnConnecting_1(object sender, EventArgs e)
        {

        }
    }
}
