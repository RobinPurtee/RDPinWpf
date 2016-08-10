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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfRdpTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RDCHost host;

        public string Computer
        {
            get { return (string)GetValue(ComputerProperty); }
            set { SetValue(ComputerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Computer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ComputerProperty =
            DependencyProperty.Register("Computer", typeof(string), typeof(MainWindow), new PropertyMetadata(string.Empty));



        public string User
        {
            get { return (string)GetValue(UserProperty); }
            set { SetValue(UserProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserProperty =
            DependencyProperty.Register("User", typeof(string), typeof(MainWindow), new PropertyMetadata(string.Empty));




        public bool IsConnected
        {
            get { return (bool)GetValue(IsConnectedProperty); }
            set { SetValue(IsConnectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsConnected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsConnectedProperty =
            DependencyProperty.Register("IsConnected", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));



        public MainWindow()
        {
            InitializeComponent();
            Computer = "MININT-7SQTTC7";
            User = "v-ripurt";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;

        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            DisconnectHost();
            Close();
        }

        private void PreSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            PreSetDialog dialog = new PreSetDialog();
            dialog.Owner = this;
            dialog.ShowDialog();
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {

            if (IsConnected)
            {
                host.GoFullScreen();
                
            }
            else
            {
                if (string.IsNullOrEmpty(Computer))
                {

                }
                else
                {
                    ConnectHost();
                }
            }
        }

        private void Host_OnDisconnected(object sender, EventArgs e)
        {
            IsConnected = false;
            IsEnabled = true;
            ConnectBtnText.Text = "Connect";
            if (host != null)
            {
                host.Close();
            }
        }

        private void Host_OnConnected(object sender, EventArgs e)
        {
            IsConnected = true;
            IsEnabled = true;
            ConnectBtnText.Text = "Restore";
            if (host != null)
            {
                host.Show();
            }
        }

        /// <summary>
        /// Create a new host controll and open the connection
        /// </summary>
        private void ConnectHost()
        {
            if(host != null)
            {
                DisconnectHost();
            }

            host = new RDCHost(Computer, User);
            host.Owner = this;
            host.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            host.OnConnected += Host_OnConnected;
            host.OnDisconnected += Host_OnDisconnected;

            host.Connect();
            host.Show();
            IsEnabled = false;
        }

        /// <summary>
        /// Disconnect and close the current host control
        /// </summary>
        private void DisconnectHost()
        {
            if (host != null)
            {
                //host.Disconnect();
                host.Close();
            }
        }

    }
}
