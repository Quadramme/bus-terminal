using System.Windows;
using System.Windows.Controls;

namespace UserApp.View.Windows
{

    public partial class ConfirmationWindow : Window
    {

        public ConfirmationWindow(Window parent, string text)
        {
            Owner = parent;
            InitializeComponent();
            TextBlock.Text = text;
        }
        public bool Success { get; private set; }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            Success = true;
            Close();
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            Success = false;
            Close();
        }
    }

}
