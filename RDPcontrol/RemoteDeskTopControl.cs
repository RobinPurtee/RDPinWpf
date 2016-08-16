using System;
using System.Windows.Forms;
using System.Diagnostics;
using AxMSTSCLib;

namespace RemoteDesktop
{
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
        D_WRONG_PIN = 7175,
        ConnectionCanceled = 7943
    }

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

    public enum WarningType
    {
        bitmapCorrupt = 1      //Bitmap cache is corrupt
    }

    public enum ConnectionStatusEnum : short
    {
        Disconnected = 0,
        Connected = 1,
        Connecting = 2
    }
    public partial class RemoteDesktopControl : UserControl
    {

        #region out going events and support classes
        #region Disconnected
        public class DisconnectEventArgs : EventArgs
        {
            public DisconnectReason reason;
            public DisconnectEventArgs(IMsTscAxEvents_OnDisconnectedEvent args)
            {
                reason = (DisconnectReason)args.discReason;
            }             
        }
        public event EventHandler<DisconnectEventArgs> OnDisconnected;
        private void AxRdpClient_OnDisconnected(object sender, IMsTscAxEvents_OnDisconnectedEvent e)
        {
            if(OnDisconnected != null)
                OnDisconnected(sender, new DisconnectEventArgs(e));
        }
        #endregion
        public event EventHandler OnAuthenticationWarningDismissed
        {
            add { rdpClient.OnAuthenticationWarningDismissed += value; }
            remove { rdpClient.OnAuthenticationWarningDismissed -= value; }
        }
        public event EventHandler OnAuthenticationWarningDisplayed
        {
            add { rdpClient.OnAuthenticationWarningDisplayed += value; }
            remove { rdpClient.OnAuthenticationWarningDisplayed -= value; }
        }
        public event EventHandler OnAutoReconnected
        {
            add { rdpClient.OnAutoReconnected += value; }
            remove { rdpClient.OnAutoReconnected -= value; }
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
        public event EventHandler<AutoReconnectingEventArgs> OnAutoReconnecting;
        private void AxRdpClient_OnAutoReconnecting(object sender, IMsTscAxEvents_OnAutoReconnecting2Event args)
        {
            if(OnAutoReconnecting != null)
                OnAutoReconnecting(sender, new AutoReconnectingEventArgs(args));
        }
        #endregion
        public event EventHandler OnConfirmClose;
        private bool AxRdpClient_OnConfirmClose(object sender, IMsTscAxEvents_OnConfirmCloseEvent e)
        {
            if(OnConfirmClose != null)
                OnConfirmClose(sender, new EventArgs());

            //TODO: figure system to implement a valid response to this
            // You would prompt the user to confirm disconnecton.  
            //      currently assumne disconnect
            return true;
        }
        public event EventHandler OnConnected
        {
            add { rdpClient.OnConnected += value; }
            remove { rdpClient.OnConnected -= value; }
        }
        public event EventHandler OnConnecting
        {
            add { rdpClient.OnConnecting += value; }
            remove { rdpClient.OnConnecting -= value; }
        }
        public event EventHandler OnConnectionBarPullDown
        {
            add { rdpClient.OnConnectionBarPullDown += value; }
            remove { rdpClient.OnConnectionBarPullDown -= value; }
        }
        public event EventHandler OnDevicesButtonPressed
        {
            add { rdpClient.OnDevicesButtonPressed += value; }
            remove { rdpClient.OnDevicesButtonPressed -= value; }
        }
        public event EventHandler OnEnterFullScreenMode
        {
            add { rdpClient.OnEnterFullScreenMode += value; }
            remove { rdpClient.OnEnterFullScreenMode -= value; }
        }
        #region OnFatalError
        public class FatalErrorEventArgs : EventArgs
        {
            public FatalErrorType error;

            public FatalErrorEventArgs(IMsTscAxEvents_OnFatalErrorEvent args)
            {
                error = (FatalErrorType)args.errorCode;
            }
        }
        public event EventHandler<FatalErrorEventArgs> OnFatalError;
        private void AxRdpClient_OnFatalError(object sender, IMsTscAxEvents_OnFatalErrorEvent e)
        {
            if(OnFatalError != null)
                OnFatalError(sender, new FatalErrorEventArgs(e));
        }
        #endregion
        //This event is use to move focus away from the control which we do not want, so do not expose it
        //public event IMsTscAxEvents_OnFocusReleasedEventHandler OnFocusReleased;
        public event EventHandler OnIdleTimeoutNotification
        {
            add { rdpClient.OnIdleTimeoutNotification += value; }
            remove { rdpClient.OnIdleTimeoutNotification -= value; }
        }
        public event EventHandler OnLeaveFullScreenMode
        {
            add { rdpClient.OnLeaveFullScreenMode += value; }
            remove { rdpClient.OnLeaveFullScreenMode -= value; }
        }
        public event EventHandler OnLoginComplete
        {
            add { rdpClient.OnLoginComplete += value; }
            remove { rdpClient.OnLoginComplete -= value; }
        }
        #region Logon Error
        public class LogonErrorEventArgs : EventArgs
        {
            public LogonErrorType error;

            public LogonErrorEventArgs(IMsTscAxEvents_OnLogonErrorEvent code)
            {
                error = (LogonErrorType)code.lError;
            }
        }
        public event EventHandler<LogonErrorEventArgs> OnLogonError;
        private void AxRdpClient_OnLogonError(object sender, IMsTscAxEvents_OnLogonErrorEvent e)
        {
            if(OnLogonError != null)
                OnLogonError(sender, new LogonErrorEventArgs(e));
        }
        #endregion
        //public event IMsTscAxEvents_OnMouseInputModeChangedEventHandler OnMouseInputModeChanged;
        //public event IMsTscAxEvents_OnNetworkStatusChangedEventHandler OnNetworkStatusChanged;
        //public event IMsTscAxEvents_OnReceivedTSPublicKeyEventHandler OnReceivedTSPublicKey;
        #region RemoteDesktopSizeChanged
        public class RemoteDesktopSizeChangeEventArgs : EventArgs
        {
            public int height;
            public int width;

            public RemoteDesktopSizeChangeEventArgs(IMsTscAxEvents_OnRemoteDesktopSizeChangeEvent args)
            {
                height = args.height;
                width = args.width;
            }
        }
        public event EventHandler<RemoteDesktopSizeChangeEventArgs> OnRemoteDesktopSizeChange;
        private void AxRdpClient_OnRemoteDesktopSizeChange(object sender, IMsTscAxEvents_OnRemoteDesktopSizeChangeEvent e)
        {
            if(OnRemoteDesktopSizeChange != null)
                OnRemoteDesktopSizeChange(sender, new RemoteDesktopSizeChangeEventArgs(e));
        }
        #endregion
        // running remote apps is not supported
        //public event IMsTscAxEvents_OnRemoteProgramDisplayedEventHandler OnRemoteProgramDisplayed;
        //public event IMsTscAxEvents_OnRemoteProgramResultEventHandler OnRemoteProgramResult;
        //public event IMsTscAxEvents_OnRemoteWindowDisplayedEventHandler OnRemoteWindowDisplayed;
        public event EventHandler OnRequestContainerMinimize
        {
            add { rdpClient.OnRequestContainerMinimize += value; }
            remove { rdpClient.OnRequestContainerMinimize -= value; }
        }
        public event EventHandler OnRequestGoFullScreen
        {
            add { rdpClient.OnRequestGoFullScreen += value; }
            remove { rdpClient.OnRequestGoFullScreen -= value; }
        }
        public event EventHandler OnRequestLeaveFullScreen
        {
            add { rdpClient.OnRequestLeaveFullScreen += value; }
            remove { rdpClient.OnRequestLeaveFullScreen -= value; }
        }
        // server messages are not supported
        //public event IMsTscAxEvents_OnServiceMessageReceivedEventHandler OnServiceMessageReceived;
        #region UserName Acquired event
        public class UserNameAcquiredEventArgs : EventArgs
        {
            public string userName;

            public UserNameAcquiredEventArgs(IMsTscAxEvents_OnUserNameAcquiredEvent e)
            {
                userName = e.bstrUserName;
            }
        }
        public event EventHandler<UserNameAcquiredEventArgs> OnUserNameAcquired;
        private void AxRdpClient_OnUserNameAcquired(object sender, IMsTscAxEvents_OnUserNameAcquiredEvent e)
        {
            if(OnUserNameAcquired != null)
                OnUserNameAcquired(sender, new UserNameAcquiredEventArgs(e));
        }
        #endregion
        #region Warning event
        public class WarningEventArgs : EventArgs
        {
            public WarningType warning;
            public WarningEventArgs(IMsTscAxEvents_OnWarningEvent arg)
            {
                warning = (WarningType)arg.warningCode;
            }
        }
        public event EventHandler<WarningEventArgs> OnWarning;
        private void AxRdpClient_OnWarning(object sender, IMsTscAxEvents_OnWarningEvent e)
        {
            if(OnWarning != null)
                OnWarning(sender, new WarningEventArgs(e));
        }
        #endregion
        #endregion

        #region Public properties
        /// <summary>
        /// Test if there is currently a connected session
        /// </summary>
        public bool IsConnected
        {
            get
            {
                bool bRet = rdpClient != null;
                if (bRet)
                {
                    try
                    {
                        bRet = (ConnectionStatusEnum)rdpClient.Connected == ConnectionStatusEnum.Connected;
                    }
                    catch (System.Windows.Forms.AxHost.InvalidActiveXStateException e)
                    {
                        Trace.WriteLine(string.Format("Error while checking connection State: {0}", e.Message));
                        bRet = false;
                    }
                }
                return bRet;
            }
        } 

        /// <summary>
        /// Test if a session is currently being connected 
        /// </summary>
        public bool IsConnecting
        {
            get
            {
                return rdpClient != null && (ConnectionStatusEnum)rdpClient.Connected == ConnectionStatusEnum.Connecting;
            }
        }

        /// <summary>
        /// Test if the control is currently in full screen mode.
        /// </summary>
        public bool FullScreen
        {
            get { return IsConnected ? false : rdpClient.FullScreen; }
            set
            {
                if (IsConnected)
                    rdpClient.FullScreen = value;
            }
        }
        #endregion


        /// <summary>
        /// default constructor
        /// </summary>
        public RemoteDesktopControl()
        {
            InitializeComponent();

            rdpClient.AdvancedSettings9.AuthenticationLevel = 2;
            rdpClient.AdvancedSettings9.EnableCredSspSupport = true;
            rdpClient.AdvancedSettings9.RedirectDrives = false;
            rdpClient.AdvancedSettings9.RedirectPrinters = false;
            rdpClient.AdvancedSettings9.RedirectPrinters = false;
            rdpClient.AdvancedSettings9.RedirectSmartCards = false;

            rdpClient.ColorDepth = 24; // int value can be 8, 15, 16, or 24
            rdpClient.DesktopWidth = 1920; // int value 
            rdpClient.DesktopHeight = 1080; // int value 
            rdpClient.Width = 1920;
            rdpClient.Height = 1080;
            rdpClient.FullScreen = true; // boolean value that can be True or False


            rdpClient.OnDisconnected += AxRdpClient_OnDisconnected;
            rdpClient.OnConfirmClose += AxRdpClient_OnConfirmClose;
            rdpClient.OnFatalError += AxRdpClient_OnFatalError;
            rdpClient.OnAutoReconnecting2 += AxRdpClient_OnAutoReconnecting;
            rdpClient.OnLogonError += AxRdpClient_OnLogonError;
            rdpClient.OnRemoteDesktopSizeChange += AxRdpClient_OnRemoteDesktopSizeChange;
            rdpClient.OnUserNameAcquired += AxRdpClient_OnUserNameAcquired;
            rdpClient.OnWarning += AxRdpClient_OnWarning;
        }

        /// <summary>
        /// Initiates a connection using the properties currently set on the control.
        /// </summary>
        public void Connect(string server, string user, string password)
        {
            rdpClient.Server = server;
            if(!string.IsNullOrEmpty(user))
                rdpClient.UserName = user;
            if(!string.IsNullOrEmpty(password))
                rdpClient.AdvancedSettings9.ClearTextPassword = password;

            rdpClient.Connect();
        }

        /// <summary>
        /// Reconnects to the remote session with the new desktop width and height.
        /// </summary>
        /// <param name="width">The new desktop width, in pixels. </param>
        /// <param name="height">The new desktop height, in pixels. </param>
        public void Reconnect(uint width, uint height)
        {
            rdpClient.Reconnect(width, height);
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

            MSTSCLib.ControlCloseStatus status = rdpClient.RequestClose();

            return status == MSTSCLib.ControlCloseStatus.controlCloseCanProceed;
        }

        /// <summary>
        /// This is experamental because of undocumented API
        /// </summary>
        /// <param name="ulDesktopWidth"></param>
        /// <param name="ulDesktopHeight"></param>
        /// <param name="ulPhysicalWidth"></param>
        /// <param name="ulPhysicalHeight"></param>
        /// <param name="ulOrientation"></param>
        /// <param name="ulDesktopScaleFactor"></param>
        /// <param name="ulDeviceScaleFactor"></param>
        public void UpdateSessionDisplaySettings(uint ulDesktopWidth, uint ulDesktopHeight, 
                                                uint ulPhysicalWidth, uint ulPhysicalHeight, 
                                                uint ulOrientation, 
                                                uint ulDesktopScaleFactor, 
                                                uint ulDeviceScaleFactor)
        {

            rdpClient.UpdateSessionDisplaySettings(ulDesktopWidth, ulDesktopHeight,
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
            if((IsConnected || IsConnecting) && !IsDisposed)
                RequestClose();
        }

        /// <summary>
        /// Retrieves the error description for the session disconnect events
        /// </summary>
        /// <param name="disconnectReason">The disconnect reason.</param>
        /// <returns>String of the current disconnection reason or an empty string if the control is not hosting a connection</returns>
        public string GetErrorDescription(uint disconnectReason)
        {
            string ret = string.Empty;
            if(rdpClient != null)
                ret = rdpClient.GetErrorDescription(disconnectReason, (uint)(rdpClient.ExtendedDisconnectReason));
            return ret;
        }

        /// <summary>
        /// Retrieves the status text for the specified status code.
        /// </summary>
        /// <param name="statusCode">A unsigned int that specifies the status code to retrieve the text for.</param>
        /// <returns></returns>
        public string GetStatusText(uint statusCode)
        {
            return rdpClient.GetStatusText(statusCode);
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

        public const Int32 WM_SYSCOMMAND = 0x112;
        public const Int32 SC_MAXIMIZE = 0xF030;
        public const Int32 SC_MINIMIZE = 0xF020;

        protected override void WndProc(ref Message m)
        {
            bool handled = false;
            if (m.Msg == WM_SYSCOMMAND)
            {
                if (m.WParam.ToInt32() == SC_MAXIMIZE)
                {
                    Trace.WriteLine("RemoteDesktopControl.WndProc: Maximize button caught");
                    rdpClient.FullScreen = true;
                    handled = true;
                }
                else if(m.WParam.ToInt32() == SC_MINIMIZE)
                {
                    Trace.WriteLine("RemoteDesktopControl.WndProc: Maximize button caught");
                    rdpClient.FullScreen = false;
                    handled = true;
                }
            }


            if (!handled)
            {
                base.WndProc(ref m);
            }
        }

    }
}
