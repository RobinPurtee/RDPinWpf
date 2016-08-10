using System;
using System.Windows;
//using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Forms.Integration;
using System.Runtime.InteropServices;
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
        public event EventHandler OnDisconnected;

        public RDCHost()
        {
            InitializeComponent();
        }

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
            CloseSpinner();
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
            if (OnConnected != null)
            {
                OnConnected(this, new EventArgs());
            }
            CloseSpinner();
        }

        private void RdpControl_OnConnecting(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnConnecting called");
            DisplaySpinner();
        }

        private void RdpControl_OnFatalError(object sender, RemoteDesktopControl.FatalErrorEventArgs args)
        {
            Trace.WriteLine("RdpControl_OnFatalError with error:" + args.error.ToString());
        }

        private void RdpControl_OnDisconnected(object sender, RemoteDesktopControl.DisconnectEventArgs args)
        {
            Trace.WriteLine("RdpControl_OnDisconnected with reason: " + args.reason.ToString());
            CloseSpinner();

            switch(args.reason)
            {
                // filter non-error resons for disconnection
                case RemoteDesktopControl.DisconnectReason.LocalNotError:
                case RemoteDesktopControl.DisconnectReason.ConnectionCanceled:
                    break;
                default:
                    string errorMesssage = rdpControl.GetErrorDescription((uint)(args.reason));
                    MessageBox.Show(errorMesssage, (string)FindResource("RdpConnectionError"), MessageBoxButton.OK);
                    break;
            }
            // once the connection is closed, delete the control
            if(!rdpControl.IsDisposed)
            {
                rdpControl.Dispose();
                rdpControl = null;
            }

            if (OnDisconnected != null)
            {
                OnDisconnected(this, new EventArgs());
            }
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

        private void FormHost_ChildChanged(object sender, ChildChangedEventArgs e)
        {
            Trace.WriteLine("FormHost_ChildChanged called ");
            if (e.PreviousChild != null)
            {
                Trace.WriteLine("FormHost_ChildChanged args: " + e.PreviousChild.ToString());
            }
        }


        private void DisplaySpinner()
        {
            Visibility = Visibility.Visible;
            Spinner.Visibility = Visibility.Visible;
            Spinner.Start();
        }

        private void CloseSpinner()
        {
            if(Spinner.IsVisible)
            {
                Spinner.Stop();
                Spinner.Visibility = Visibility.Hidden;
            }
            Visibility = Visibility.Hidden;
            FormHost.Focus();
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

                FormHost.Child = rdpControl;
 
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
                rdpControl.Show();
                rdpControl.Select();
                rdpControl.FullScreen = true;

                Activate();
            }
        }

        
        
        
        // nCmdShow values:
        const int SW_HIDE = 0;
        const int SW_HSHOWNORMAL = 1;
        const int SW_SHOWMINIMIZED = 2;
        const int SW_MAXIMIZE = 3;
        const int SW_SHOWNOACTIVATE = 4;
        const int SW_SHOW = 5;
        const int SW_MINIMIZE = 6;
        const int SW_SHOWMINNOACTIVE = 7;
        const int SW_SHOWNA = 8;
        const int SW_RESTORE = 9;
        const int SW_SHOWDEFAULT = 10;
        const int SW_FORCEMINIMIZE = 11;
        [DllImport("user32.dll")]
        static extern int ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        static extern int ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        static extern int BringWindowToTop(IntPtr hWnd);


        const uint SWP_NOSIZE = 0x0001; // Retains the current size (ignores the cx and cy parameters).
        const uint SWP_NOMOVE = 0x0002; // Retains the current position (ignores the x and y parameters).
        const uint SWP_NOZORDER = 0x0004; // Retains the current Z order (ignores the hWndInsertAfter parameter).
        const uint SWP_NOREDRAW = 0x0008; // Does not redraw changes.If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved.When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
        const uint SWP_NOACTIVATE = 0x0010; // Does not activate the window.If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
        const uint SWP_FRAMECHANGED = 0x0020; // Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
        const uint SWP_DRAWFRAME = SWP_FRAMECHANGED; // Draws a frame(defined in the window's class description) around the window.
        const uint SWP_SHOWWINDOW = 0x0040; // Displays the window
        const uint SWP_HIDEWINDOW = 0x0080; // Hides the window.
        const uint SWP_NOCOPYBITS = 0x0100; // Discards the entire contents of the client area.If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
        const uint SWP_NOOWNERZORDER = 0x0200; // Does not change the owner window's position in the Z order.
        const uint SWP_NOREPOSITION = 0x0200; // Same as the SWP_NOOWNERZORDER flag.
        const uint SWP_NOSENDCHANGING = 0x0400; // Prevents the window from receiving the WM_WINDOWPOSCHANGING message.


        enum HWND : int
        {
            NOTOPMOST = -2,     //Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window.
            TOPMOST = -1,       //Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
            TOP = 0,            //Places the window at the top of the Z order.
            BOTTOM = 1,         //Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.
        }

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowPos(IntPtr hWnd, IntPtr hwndInserAfter, int x, int y, int cx, int cy, uint uFlags);



    }
}
