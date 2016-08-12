using System;
using System.Windows;
//using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Forms.Integration;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using RemoteDesktop;


namespace WpfRdpTest
{
    /// <summary>
    /// Interaction logic for RDCHost.xaml
    /// </summary>
    public partial class RDCHost : Window
    {
        public string ComputerName { get; set; }
        public string User { get; set; }


        private RemoteDesktopControl rdpControl;

        public event EventHandler OnConnected;
        public event EventHandler<RemoteDesktopControl.DisconnectEventArgs> OnDisconnected;
        public event EventHandler OnStartWaiting;
        public event EventHandler OnStopWaiting;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public RDCHost()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializing Constructor
        /// </summary>
        /// <param name="computer">The Remote Desktop server</param>
        /// <param name="user">The user name to login with</param>
        public RDCHost(string computer, string user) : this()
        {
            ComputerName = computer;
            User = user;
        }





        #region RDP Conntrol event handlers
        private void RdpControl_OnAuthenticationWarningDismissed(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnAuthenticationWarningDismissed ");
        }

        private void RdpControl_OnAuthenticationWarningDisplayed(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnAuthenticationWarningDisplayed ");
            if (OnStopWaiting != null)
            {
                OnStopWaiting(this, new EventArgs());
            }
        }

        private void RdpControl_OnAutoReconnected(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnAutoReconnected ");
        }

        private void RdpControl_OnAutoReconnecting(object sender, RemoteDesktopControl.AutoReconnectingEventArgs args)
        {
            Trace.WriteLine("RdpControl_OnAutoReconnecting ");
        }

        private void RdpControl_OnConnectionBarPullDown(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnConnectionBarPullDown ");
        }

        private void RdpControl_OnConfirmClose(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnConfirmClose called");
        }

        private void RdpControl_OnConnected(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnConnected called");
            if (OnStopWaiting != null)
            {
                OnStopWaiting(this, new EventArgs());
            }
            if (OnConnected != null)
            {
                OnConnected(this, new EventArgs());
            }
        }

        private void RdpControl_OnConnecting(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnConnecting called");
            if (OnStartWaiting != null)
            {
                OnStartWaiting(this, new EventArgs());
            }
        }

        private void RdpControl_OnDisconnected(object sender, RemoteDesktopControl.DisconnectEventArgs args)
        {
            Trace.WriteLine("RdpControl_OnDisconnected with reason: " + args.reason.ToString());
            if (OnStopWaiting != null)
            {
                OnStopWaiting(this, new EventArgs());
            }
            if (OnDisconnected != null)
            {
                OnDisconnected(this, args);
            }
        }

        private void RdpControl_OnFatalError(object sender, RemoteDesktopControl.FatalErrorEventArgs args)
        {
            Trace.WriteLine("RdpControl_OnFatalError with error:" + args.error.ToString());
        }

        private void RdpControl_OnDevicesButtonPressed(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnDevicesButtonPressed ");
        }

        private void RdpControl_OnEnterFullScreenMode(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnEnterFullScreenMode ");
        }

        private void RdpControl_OnLeaveFullScreenMode(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnLeaveFullScreenMode ");
            // if not disconnecting display a toast
            rdpControl.FullScreen = true;
        }

        private void RdpControl_OnLoginComplete(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnLoginComplete ");
        }

        private void RdpControl_OnLogonError(object sender, RemoteDesktopControl.LogonErrorEvent args)
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
        }


        private void RdpControl_OnUserNameAcquired(object sender, RemoteDesktopControl.UserNameAcquiredEvent args)
        {
            Trace.WriteLine("RdpControl_OnUserNameAcquired Name: " + args.userName);
        }

        private void RdpControl_OnWarning(object sender, RemoteDesktopControl.WarningEvent e)
        {
            Trace.WriteLine(string.Format("RdpControl_OnWarning recieved : {0}", e.warning.ToString()));
        }
        #endregion

        /// <summary>
        /// Window Closing handler
        /// </summary>
        /// <param name="sender">message sender</param>
        /// <param name="e">EventArgs that allows the action to be canceled</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Disconnect();
            if (rdpControl != null)
            {
                if (!rdpControl.IsDisposed)
                {
                    rdpControl.Dispose();
                }
                rdpControl = null;
            }
        }

 

 

        public void Connect()
        {
            try
            {
                // Create the rdp control.
                rdpControl = new RemoteDesktopControl();
                rdpControl.OnAuthenticationWarningDismissed += RdpControl_OnAuthenticationWarningDismissed;
                rdpControl.OnAuthenticationWarningDisplayed += RdpControl_OnAuthenticationWarningDisplayed;
                rdpControl.OnAutoReconnected += RdpControl_OnAutoReconnected;
                rdpControl.OnAutoReconnecting += RdpControl_OnAutoReconnecting;

                rdpControl.OnConfirmClose += RdpControl_OnConfirmClose;
                rdpControl.OnConnected += RdpControl_OnConnected;
                rdpControl.OnConnecting += RdpControl_OnConnecting;
                rdpControl.OnConnectionBarPullDown += RdpControl_OnConnectionBarPullDown;
                rdpControl.OnDevicesButtonPressed += RdpControl_OnDevicesButtonPressed;
                rdpControl.OnDisconnected += RdpControl_OnDisconnected;
                rdpControl.OnEnterFullScreenMode += RdpControl_OnEnterFullScreenMode;
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

                rdpControl.Connect(ComputerName, User, null);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(string.Format("RDP Connect call threw exception: {0}", exception.ToString()));
            }
        }



        /// <summary>
        /// Test if the host is actually connected to a Remote Desktop
        /// </summary>
        public bool IsConnected
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
            if (IsConnected)
            {
                rdpControl.Disconnect();
            }
        }

        public void GoFullScreen()
        {
            if(IsConnected)
            {
                rdpControl.FullScreen = false;
                //WindowState = WindowState.Normal;
                //rdpControl.FullScreen = true;
                //rdpControl.Invalidate();
                //rdpControl.Focus();
                
                //BringWindowToTop(rdpControl.Handle);

                //BringWindowToTop(rdpControl.Handle);
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
