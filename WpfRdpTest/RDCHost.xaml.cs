using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
//using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Forms.Integration;
using RDPcontrol;


namespace WpfRdpTest
{
    /// <summary>
    /// Interaction logic for RDCHost.xaml
    /// </summary>
    public partial class RDCHost : Window
    {
        public string ComputerName { get; set; }
        public string User { get; set; }

        private RemoteDeskTopControl rdpControl;

        public RDCHost()
        {
            InitializeComponent();
        }

        public RDCHost(string computer, string user) : this()
        {
            ComputerName = computer;
            User = user;
        }


        private void HostLoaded(object sender, RoutedEventArgs e)
        {
            // Create the interop host control.
            WindowsFormsHost host = new WindowsFormsHost();

            // Create the rdp control.
            rdpControl = new RemoteDeskTopControl();
            rdpControl.OnLogonError += RdpControl_OnLogonError;
            rdpControl.OnLoginComplete += RdpControl_OnLoginComplete;
            rdpControl.OnUserNameAcquired += RdpControl_OnUserNameAcquired;
            rdpControl.OnConnecting += RdpControl_OnConnecting;
            rdpControl.OnConnected += RdpControl_OnConnected;
            rdpControl.OnConfirmClose += RdpControl_OnConfirmClose;
            rdpControl.OnDisconnected += RdpControl_OnDisconnected;
            rdpControl.OnFatalError += RdpControl_OnFatalError;
            rdpControl.OnWarning += RdpControl_OnWarning;

            // Assign the MaskedTextBox control as the host control's child.
            host.Child = rdpControl;
            // Add the interop host control to the Grid
            // control's collection of child controls.
            this.HostGrid.Children.Add(host);

        }

        private void RdpControl_OnWarning(object sender, RemoteDeskTopControl.WarningEvent e)
        {
            Trace.WriteLine(string.Format("RdpControl_OnWarning recieved : {0}", e.warning.ToString()));
        }

        private void RdpControl_OnUserNameAcquired(object sender, RemoteDeskTopControl.UserNameAcquiredEvent args)
        {
            Trace.WriteLine("RdpControl_OnUserNameAcquired Name: " + args.userName);
        }

        private void RdpControl_OnLoginComplete(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnLoginComplete ");
        }

        private void RdpControl_OnLogonError(object sender, RemoteDeskTopControl.LogonErrorEvent args)
        {
            Trace.WriteLine("RdpControl_OnLogonError with error: " + args.error.ToString());
        }

        private void RdpControl_OnFatalError(object sender, RemoteDeskTopControl.FatalErrorEventArgs args)
        {
            Trace.WriteLine("RdpControl_OnFatalError with error:" + args.error.ToString());
        }

        private void RdpControl_OnDisconnected(object sender, RemoteDeskTopControl.DisconnectEventArgs args)
        {
            Trace.WriteLine("RdpControl_OnDisconnected with reason: " + args.reason.ToString());
        }

        private void RdpControl_OnConnecting(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnConnecting called");
        }

        private void RdpControl_OnConnected(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnConnected called");
        }

        private void RdpControl_OnConfirmClose(object sender, EventArgs e)
        {
            Trace.WriteLine("RdpControl_OnConnecting called");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            rdpControl.RequestClose();
            rdpControl.Dispose();
        }
    }
}
