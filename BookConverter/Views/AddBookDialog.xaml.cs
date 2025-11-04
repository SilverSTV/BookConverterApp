using System.Windows;


namespace WpfBookConverter.Views
{
    public partial class InputDialog : Window
    {
        public string Value => ValueTextBox.Text;
        public InputDialog(string prompt)
        {
            InitializeComponent();
            Title = prompt;
            Owner = Application.Current.MainWindow;
        }


        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
