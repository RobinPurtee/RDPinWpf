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

            // Assign the MaskedTextBox control as the host control's child.
            host.Child = rdpControl;

            // Add the interop host control to the Grid
            // control's collection of child controls.
            this.HostGrid.Children.Add(host);

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            rdpControl.Dispose();
        }
    }
}
