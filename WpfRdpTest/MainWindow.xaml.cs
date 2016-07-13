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



        public MainWindow()
        {
            InitializeComponent();
            Computer = "MININT-7QTTC7";
            User = "v-ripurt";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;

        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
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
            RDCHost host = new RDCHost(Computer, User);
            host.Owner = this;
            host.ShowDialog();
        }
    }
}
