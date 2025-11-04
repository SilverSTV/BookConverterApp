using System.Windows;


namespace WpfBookConverter.Views
{
    public enum OutputFormat { PDF, FB2 }


    public partial class FormatDialog : Window
    {
        public OutputFormat SelectedFormat { get; private set; } = OutputFormat.PDF;
        public FormatDialog()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
        }


        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            SelectedFormat = PdfRb.IsChecked == true ? OutputFormat.PDF : OutputFormat.FB2;
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
