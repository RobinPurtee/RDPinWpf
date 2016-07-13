using System;
using System.Windows.Forms;
using System.Diagnostics;
using AxMSTSCLib;

namespace RDPcontrol
{
    public partial class RemoteDeskTopControl : UserControl
    {

        #region out going events and support classes
        #region Disconnected
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
            public DisconnectReason reason;
            public DisconnectEventArgs(IMsTscAxEvents_OnDisconnectedEvent args)
            {
                reason = (DisconnectReason)args.discReason;
            }             
        }
        /// <summary>
        /// OnDisconnect handler delegate
        /// </summary>
        /// <param name="sender">the object that fired the event</param>
        /// <param name="args">the reason for the disconnect</param>
        public delegate void DisconnectedEventHandler(object sender, DisconnectEventArgs args);

        /// <summary>
        /// Fired on disconnecting from the remote host
        /// </summary>
        public event DisconnectedEventHandler OnDisconnected;

        private void AxRdpClient_OnDisconnected(object sender, IMsTscAxEvents_OnDisconnectedEvent e)
        {
            OnDisconnected(sender, new DisconnectEventArgs(e));
        }
        #endregion

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

        #region AutoReconnectingEvent
        public class AutoReconnectingEventArgs : EventArgs
        {
            public int  attemptCount;
            public int  disconnectReason;
            public int  maxAttemptCount;
            public bool networkAvailable;

            public AutoReconnectingEventArgs(IMsTscAxEvents_OnAutoReconnecting2Event eventArgs)
            {
                attemptCount = eventArgs.attemptCount;
                disconnectReason = eventArgs.disconnectReason;
                maxAttemptCount = eventArgs.maxAttemptCount;
                networkAvailable = eventArgs.networkAvailable;
            }
        }

        public delegate void AutoReconnectingEventHandler(object sender, AutoReconnectingEventArgs args);

        public event AutoReconnectingEventHandler OnAutoReconnecting;

        private void AxRdpClient_OnAutoReconnecting(object sender, IMsTscAxEvents_OnAutoReconnecting2Event args)
        {
            OnAutoReconnecting(sender, new AutoReconnectingEventArgs(args));
        }

        #endregion

        public event EventHandler OnConfirmClose;

        private bool AxRdpClient_OnConfirmClose(object sender, IMsTscAxEvents_OnConfirmCloseEvent e)
        {
            OnConfirmClose(sender, new EventArgs());

            //TODO: figure system to implement a valid response to this
            // You would prompt the useer to confirm disconnecton.  
            //      currently assumne disconnect
            return true;
        }


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

        #region OnFatalError
        public enum FatalErrorType 
        {
            Unknown = 0,
            Internal_1 = 1,
            OutOfMemory = 2,
            WindowCreation = 3,
            Internal_2 = 4,
            Internal_3 = 5,
            Internal_4 = 6,
            Unrecoverable = 7,
            Winsock = 100
        };

        public class FatalErrorEventArgs : EventArgs
        {
            public FatalErrorType error;

            public FatalErrorEventArgs(IMsTscAxEvents_OnFatalErrorEvent args)
            {
                error = (FatalErrorType)args.errorCode;
            }
        }
        public delegate void FatalErrorEventHandler(object sender, FatalErrorEventArgs args);
        public event FatalErrorEventHandler OnFatalError;

        private void AxRdpClient_OnFatalError(object sender, IMsTscAxEvents_OnFatalErrorEvent e)
        {
            OnFatalError(sender, new FatalErrorEventArgs(e));
        }
        #endregion


        //This event is use to move focus away from the control which we do not want, so do not expose it
        //public event IMsTscAxEvents_OnFocusReleasedEventHandler OnFocusReleased;

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
        #region Logon Error
        public enum LogonErrorType
        {

            ARBITRATION_CODE_BUMP_OPTIONS = -5,         //Winlogon is displaying the Session Contention dialog box.
            ARBITRATION_CODE_CONTINUE_LOGON = -2,       //Winlogon is continuing with the logon process.
            ARBITRATION_CODE_CONTINUE_TERMINATE = -3,   //Winlogon is ending silently.
            ARBITRATION_CODE_NOPERM_DIALOG = -6,        //Winlogon is displaying the No Permissions dialog box.
            ARBITRATION_CODE_REFUSED_DIALOG = -7,       //Winlogon is displaying the Disconnect Refused dialog box.
            ARBITRATION_CODE_RECONN_OPTIONS = -4,       //Winlogon is displaying the Reconnect dialog box.
            ERROR_CODE_ACCESS_DENIED = -1,              //The user was denied access.
            LOGON_FAILED_BAD_PASSWORD = 0,              //The logon failed because the logon credentials are not valid.
            LOGON_FAILED_UPDATE_PASSWORD = 1,           //The password is expired. The user must update their password to continue logging on. 
            LOGON_FAILED_OTHER = 2,                     //Another logon or post-logon error occurred. The Remote Desktop client displays a logon screen to the user.
            LOGON_WARNING = 3,                          //The Remote Desktop client displays a dialog box that contains important information for the user.
            STATUS_ACCOUNT_RESTRICTION = -1073741714,   //The user name and authentication information are valid, but authentication was blocked due to restrictions on the user account, such as time-of-day restrictions.
            STATUS_LOGON_FAILURE = -1073741715,         //The attempted logon is not valid. This is due to either an incorrect user name or incorrect authentication information.
            STATUS_PASSWORD_MUST_CHANGE = -1073741276   //The password is expired. The user must update their password to continue logging on.
        }

        public class LogonErrorEvent : EventArgs
        {
            public LogonErrorType error;

            public LogonErrorEvent(IMsTscAxEvents_OnLogonErrorEvent code)
            {
                error = (LogonErrorType)code.lError;
            }
        }

        public delegate void LogonErrorEventHandler(object sender, LogonErrorEvent args);
        public event LogonErrorEventHandler OnLogonError;
        private void AxRdpClient_OnLogonError(object sender, IMsTscAxEvents_OnLogonErrorEvent e)
        {
            OnLogonError(sender, new LogonErrorEvent(e));
        }
        #endregion
        //public event IMsTscAxEvents_OnMouseInputModeChangedEventHandler OnMouseInputModeChanged;
        //public event IMsTscAxEvents_OnNetworkStatusChangedEventHandler OnNetworkStatusChanged;
        //public event IMsTscAxEvents_OnReceivedTSPublicKeyEventHandler OnReceivedTSPublicKey;
        #region RemoteDesktopSizeChanged
        public class RemoteDesktopSizeChangeEvent : EventArgs
        {
            public int height;
            public int width;

            public RemoteDesktopSizeChangeEvent(IMsTscAxEvents_OnRemoteDesktopSizeChangeEvent args)
            {
                height = args.height;
                width = args.width;
            }
        }
        public delegate void RemoteDesktopSizeChangeEventHandler(object sender, RemoteDesktopSizeChangeEvent args);
        public event RemoteDesktopSizeChangeEventHandler OnRemoteDesktopSizeChange;
        private void AxRdpClient_OnRemoteDesktopSizeChange(object sender, IMsTscAxEvents_OnRemoteDesktopSizeChangeEvent e)
        {
            OnRemoteDesktopSizeChange(sender, new RemoteDesktopSizeChangeEvent(e));
        }
        #endregion

        // running remote apps is not supported
        //public event IMsTscAxEvents_OnRemoteProgramDisplayedEventHandler OnRemoteProgramDisplayed;
        //public event IMsTscAxEvents_OnRemoteProgramResultEventHandler OnRemoteProgramResult;
        //public event IMsTscAxEvents_OnRemoteWindowDisplayedEventHandler OnRemoteWindowDisplayed;

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

        // server messages are not supported
        //public event IMsTscAxEvents_OnServiceMessageReceivedEventHandler OnServiceMessageReceived;
        #region UserName Acquired event
        public class UserNameAcquiredEvent : EventArgs
        {
            public string userName;

            public UserNameAcquiredEvent(IMsTscAxEvents_OnUserNameAcquiredEvent e)
            {
                userName = e.bstrUserName;
            }
        }
        public delegate void UserNameAcquiredEventHandler(object sender, UserNameAcquiredEvent args);
        public event UserNameAcquiredEventHandler OnUserNameAcquired;
        private void AxRdpClient_OnUserNameAcquired(object sender, IMsTscAxEvents_OnUserNameAcquiredEvent e)
        {
            OnUserNameAcquired(sender, new UserNameAcquiredEvent(e));
        }
        #endregion
        #region Warning event
        public enum WarningType
        {
            bitmapCorrupt  = 1      //Bitmap cache is corrupt
        }
        public class WarningEvent : EventArgs
        {
            public WarningType warning;
            public WarningEvent(IMsTscAxEvents_OnWarningEvent arg)
            {
                warning = (WarningType)arg.warningCode;
            }
        }
        public delegate void WarningEventHandler(object sender, WarningEvent e);
        public event WarningEventHandler OnWarning;
        private void AxRdpClient_OnWarning(object sender, IMsTscAxEvents_OnWarningEvent e)
        {
            OnWarning(sender, new WarningEvent(e));
        }

        #endregion



        #endregion

        #region Public properties
        private const short RDP_CLIENT_DISCONNECTED = 0;
        private const short RDP_CLIENT_CONNECTED = 1;
        private const short RDP_CLIENT_CONNECTING = 2; 
        /// <summary>
        /// Test if there is currently a connected session
        /// </summary>
        public bool IsConnected
        {
            get
            {
                bool bRet = axRdpClient != null;
                if(bRet)
                {
                    try
                    {
                        bRet = axRdpClient.Connected == RDP_CLIENT_CONNECTED;
                    }
                    catch(System.Windows.Forms.AxHost.InvalidActiveXStateException e)
                    {
                        Debug.WriteLine("Error while checking connection State: {0}", e.Message);
                        bRet = false;
                    }
                }
                return bRet;
            }
        } 

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

            axRdpClient.OnDisconnected += AxRdpClient_OnDisconnected;
            axRdpClient.OnConfirmClose += AxRdpClient_OnConfirmClose;
            axRdpClient.OnFatalError += AxRdpClient_OnFatalError;
            axRdpClient.OnAutoReconnecting2 += AxRdpClient_OnAutoReconnecting;
            axRdpClient.OnLogonError += AxRdpClient_OnLogonError;
            axRdpClient.OnRemoteDesktopSizeChange += AxRdpClient_OnRemoteDesktopSizeChange;
            axRdpClient.OnUserNameAcquired += AxRdpClient_OnUserNameAcquired;
            axRdpClient.OnWarning += AxRdpClient_OnWarning;
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
        }

        public void UpdateSessionDisplaySettings(uint ulDesktopWidth, uint ulDesktopHeight, 
                                                uint ulPhysicalWidth, uint ulPhysicalHeight, 
                                                uint ulOrientation, 
                                                uint ulDesktopScaleFactor, 
                                                uint ulDeviceScaleFactor)
        {

            axRdpClient.UpdateSessionDisplaySettings(ulDesktopWidth, ulDesktopHeight,
                                                     ulPhysicalWidth, ulPhysicalHeight,
                                                     ulOrientation,
                                                     ulDesktopScaleFactor,
                                                     ulDeviceScaleFactor);


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



        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            
            if(IsConnected)
            {


            }
        }

    }
}
