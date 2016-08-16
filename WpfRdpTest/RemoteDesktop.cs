using System;
using System.Windows;
using System.Diagnostics;
using RemoteDesktop;


namespace WpfRdpTest
{
    public partial class MainWindow
    {

        private RemoteDesktopControl rdpControl;



        #region RDP Conntrol event handlers
        //private void RdpControl_OnAuthenticationWarningDismissed(object sender, EventArgs e)
        //{
        //    Trace.WriteLine("RdpControl_OnAuthenticationWarningDismissed ");
        //}

        private void RdpControl_OnAuthenticationWarningDisplayed(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnAuthenticationWarningDisplayed ");
            CloseSpinner();
        }

        private void RdpControl_OnAutoReconnected(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnAutoReconnected ");
            CloseSpinner();
        }

        private void RdpControl_OnAutoReconnecting(object sender, RemoteDesktopControl.AutoReconnectingEventArgs args)
        {
            Trace.WriteLine("RdpControl_OnAutoReconnecting ");
            DisplaySpinner();
        }

        //private void RdpControl_OnConnectionBarPullDown(object sender, EventArgs e)
        //{
        //    Trace.WriteLine("RdpControl_OnConnectionBarPullDown ");
        //}

        //private void RdpControl_OnConfirmClose(object sender, EventArgs e)
        //{
        //    Trace.WriteLine("RdpControl_OnConfirmClose called");
        //}

        private void RdpControl_OnConnected(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnConnected called");
            CloseSpinner();
            ConnectionState = ConnectionStatusEnum.Connected;

            rdpControl.Show();
            rdpControl.FullScreen = true;
        }

        private void RdpControl_OnConnecting(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnConnecting called");
            ConnectionState = ConnectionStatusEnum.Connecting;
            DisplaySpinner();
        }

        private void RdpControl_OnDisconnected(object sender, RemoteDesktopControl.DisconnectEventArgs args)
        {
            Trace.WriteLine("RdpControl_OnDisconnected with reason: " + args.reason.ToString());
            CloseSpinner();
            ConnectionState = ConnectionStatusEnum.Disconnected;


            switch (args.reason)
            {
                // filter non-error resons for disconnection
                case DisconnectReason.LocalNotError:
                case DisconnectReason.ConnectionCanceled:
                    break;
                default:
                    string errorMesssage = GetErrorDescription(args.reason);
                    if (string.IsNullOrEmpty(errorMesssage))
                    {
                        errorMesssage = (string)FindResource("NotConnectedString");
                    }
                    MessageBox.Show(errorMesssage, (string)FindResource("RdpConnectionError"), MessageBoxButton.OK);
                    break;
            }
            if (rdpControl != null)
            {
                rdpControl.Dispose();
                rdpControl = null;
            }
        }

        private void RdpControl_OnFatalError(object sender, RemoteDesktopControl.FatalErrorEventArgs args)
        {
            Trace.WriteLine("RdpControl_OnFatalError with error:" + args.error.ToString());
            string errorMesssage;
            switch (args.error)
            {
                case FatalErrorType.Winsock:
                    errorMesssage = (string)FindResource("WinsockFatalError");
                    break;
                case FatalErrorType.WindowCreation:
                    errorMesssage = (string)FindResource("CreationFatalError");
                    break;
                case FatalErrorType.OutOfMemory:
                    errorMesssage = (string)FindResource("OutofMemoryFatalError");
                    break;
                default:
                    errorMesssage = (string)FindResource("DefaultFatalError");
                    break;
            }
            if (string.IsNullOrEmpty(errorMesssage))
            {
                errorMesssage = (string)FindResource("NotConnectedString");
            }
            MessageBox.Show(errorMesssage, (string)FindResource("RdpConnectionError"), MessageBoxButton.OK);
            Disconnect();
        }

        //private void RdpControl_OnDevicesButtonPressed(object sender, EventArgs e)
        //{
        //    Trace.WriteLine("RdpControl_OnDevicesButtonPressed ");
        //}

        //private void RdpControl_OnEnterFullScreenMode(object sender, EventArgs e)
        //{
        //    Trace.WriteLine("RdpControl_OnEnterFullScreenMode ");
        //}

        private void RdpControl_OnLeaveFullScreenMode(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnLeaveFullScreenMode ");
            // if not disconnecting display a toast
            rdpControl.FullScreen = true;
        }

        private void RdpControl_OnLoginComplete(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnLoginComplete ");
            rdpControl.FullScreen = true;
        }

        private void RdpControl_OnLogonError(object sender, RemoteDesktopControl.LogonErrorEventArgs args)
        {
            Trace.WriteLine("RdpControl_OnLogonError with error: " + args.error.ToString());
        }

        private void RdpControl_OnRequestContainerMinimize(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnRequestContainerMinimize ");
        }

        private void RdpControl_OnRequestGoFullScreen(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnRequestGoFullScreen ");
        }

        private void RdpControl_OnRequestLeaveFullScreen(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnRequestLeaveFullScreen ");
            if(rdpControl != null)
            {
                rdpControl.FullScreen = true;
            }
        }


        private void RdpControl_OnUserNameAcquired(object sender, RemoteDesktopControl.UserNameAcquiredEventArgs args)
        {
            Trace.WriteLine("RdpControl_OnUserNameAcquired Name: " + args.userName);
        }

        private void RdpControl_OnWarning(object sender, RemoteDesktopControl.WarningEventArgs e)
        {
            Trace.WriteLine(string.Format("RdpControl_OnWarning recieved : {0}", e.warning.ToString()));
        }
        #endregion

        public void Connect()
        {
            if (string.IsNullOrEmpty(Computer))
            {
                throw new ArgumentException((string)FindResource("NoComputerNameError"));
            }
            if(rdpControl != null)
            {
                Disconnect();
            }
            // Create the rdp control.
            rdpControl = new RemoteDesktopControl();
            //rdpControl.OnAuthenticationWarningDismissed += RdpControl_OnAuthenticationWarningDismissed;
            rdpControl.OnAuthenticationWarningDisplayed += RdpControl_OnAuthenticationWarningDisplayed;
            rdpControl.OnAutoReconnected += RdpControl_OnAutoReconnected;
            rdpControl.OnAutoReconnecting += RdpControl_OnAutoReconnecting;

            //rdpControl.OnConfirmClose += RdpControl_OnConfirmClose;
            rdpControl.OnConnected += RdpControl_OnConnected;
            rdpControl.OnConnecting += RdpControl_OnConnecting;
            //rdpControl.OnConnectionBarPullDown += RdpControl_OnConnectionBarPullDown;
            //rdpControl.OnDevicesButtonPressed += RdpControl_OnDevicesButtonPressed;
            rdpControl.OnDisconnected += RdpControl_OnDisconnected;
            //rdpControl.OnEnterFullScreenMode += RdpControl_OnEnterFullScreenMode;
            rdpControl.OnFatalError += RdpControl_OnFatalError;
            rdpControl.OnLeaveFullScreenMode += RdpControl_OnLeaveFullScreenMode;
            rdpControl.OnLoginComplete += RdpControl_OnLoginComplete;
            rdpControl.OnLogonError += RdpControl_OnLogonError;
            rdpControl.OnRequestContainerMinimize += RdpControl_OnRequestContainerMinimize;
            rdpControl.OnRequestGoFullScreen += RdpControl_OnRequestGoFullScreen;
            rdpControl.OnRequestLeaveFullScreen += RdpControl_OnRequestLeaveFullScreen;
            rdpControl.OnUserNameAcquired += RdpControl_OnUserNameAcquired;
            rdpControl.OnWarning += RdpControl_OnWarning;

            RemoteDesktopControlHost.Child = rdpControl;

            rdpControl.Top = 0;
            rdpControl.Left = 0;

            rdpControl.Connect(Computer, User, null);
        }



        /// <summary>
        /// Test if the host is actually connected to a Remote Desktop
        /// </summary>
        private bool IsActuallyConnected
        {
            get
            {
                bool bRet = false;
                if (rdpControl != null)
                {
                    bRet = rdpControl.IsConnected;
                }
                return bRet;
            }
        }

        public void Disconnect()
        {
            if (IsActuallyConnected)
            {
                rdpControl.Disconnect();
            }
            else
            {
                if (rdpControl != null)
                {
                    rdpControl.Dispose();
                }
                rdpControl = null;
            }
        }

        public void GoFullScreen()
        {
            if (IsActuallyConnected)
            {
                // this is all that is need has the fullscreen mode left handler
                // will reset it to full screen.
                rdpControl.FullScreen = false;
            }
        }

        /// <summary>
        /// Retrieves the error description for the session disconnect events
        /// </summary>
        /// <param name="disconnectReason">The disconnect reason.</param>
        /// <returns></returns>
        public string GetErrorDescription(DisconnectReason disconnectReason)
        {
            string ret = string.Empty;
            if (rdpControl != null)
                ret = rdpControl.GetErrorDescription((uint)disconnectReason);
            return ret;
        }


    }
}
