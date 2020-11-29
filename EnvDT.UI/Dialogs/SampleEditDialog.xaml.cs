using EnvDT.UI.ViewModel;
using System.Windows;

namespace EnvDT.UI.Dialogs
{
    public partial class SampleEditDialog : Window
    {
        public SampleEditDialog(string title, ISampleEditDialogViewModel sampleEditDialogViewModel)
        {
            Title = title;
            DataContext = sampleEditDialogViewModel;
            InitializeComponent();           
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

    }
}
