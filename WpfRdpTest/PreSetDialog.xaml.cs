using System.Windows;

namespace WpfRdpTest
{
    /// <summary>
    /// Interaction logic for PreSetDialog.xaml
    /// </summary>
    public partial class PreSetDialog : Window
    {
        public PreSetDialog()
        {
            InitializeComponent();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
