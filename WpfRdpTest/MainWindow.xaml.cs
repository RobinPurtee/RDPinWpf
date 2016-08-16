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
using RemoteDesktop;

namespace WpfRdpTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


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

        private ConnectionStatusEnum currentState = ConnectionStatusEnum.Disconnected;
        public ConnectionStatusEnum ConnectionState
        {
            get { return currentState; }
            set
            {
                currentState = value;
                switch(currentState)
                {
                    case ConnectionStatusEnum.Connected:
                        ConnectBtn.Visibility = Visibility.Hidden;
                        RestoreBtn.Visibility = Visibility.Visible;
                        CancelBtn.Visibility = Visibility.Hidden;
                        IsConnected = true;
                        break;
                    case ConnectionStatusEnum.Connecting:
                        ConnectBtn.Visibility = Visibility.Hidden;
                        RestoreBtn.Visibility = Visibility.Hidden;
                        CancelBtn.Visibility = Visibility.Visible;
                        IsConnected = true;
                        break;
                    case ConnectionStatusEnum.Disconnected:
                    default:
                        ConnectBtn.Visibility = Visibility.Visible;
                        RestoreBtn.Visibility = Visibility.Hidden;
                        CancelBtn.Visibility = Visibility.Hidden;
                        IsConnected = false;
                        break;
                }
            }
        }

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

        private void PreSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            PreSetDialog dialog = new PreSetDialog();
            dialog.Owner = this;
            dialog.ShowDialog();
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(Computer))
            {
                MessageBox.Show((string)FindResource("NoComputerName"), (string)FindResource("ErrorTitle"));
            }
            else
            {
                ConnectHost();
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (host != null)
            {
                host.Disconnect();
            }
        }

        private void RestoreBtn_Click(object sender, RoutedEventArgs e)
        {
            if (host != null)
            {
                host.GoFullScreen();
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            DisconnectHost();
            Close();
        }

        private void Host_OnDisconnected(object sender, RemoteDesktopControl.DisconnectEventArgs args)
        {
        }

        private void Host_OnConnected(object sender, EventArgs e)
        {
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

            ConnectionState = ConnectionStatusEnum.Connecting;

            host = new RDCHost(Computer, User); 
            host.Owner = this;
            host.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            host.OnConnected += Host_OnConnected;
            host.OnDisconnected += Host_OnDisconnected;
            host.StartWaiting += DisplaySpinner;
            host.StopWaiting += CloseSpinner;

            host.Closed += Host_Closed;
            host.Connect();

        }

        private void Host_Closed(object sender, EventArgs e)
        {
            if(ConnectionState != ConnectionStatusEnum.Disconnected)
            {
                ConnectionState = ConnectionStatusEnum.Disconnected;
            }
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
                host = null;
            }
        }


        private void DisplaySpinner()
        {
            Spinner.Visibility = Visibility.Visible;
            Spinner.Start();
        }

        private void CloseSpinner()
        {
            if (Spinner.IsVisible)
            {
                Spinner.Stop();
                Spinner.Visibility = Visibility.Hidden;
            }
        }

    }
}
