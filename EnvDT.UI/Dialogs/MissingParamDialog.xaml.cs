using System.Windows;

namespace EnvDT.UI.Dialogs
{
  public partial class MissingParamDialog : Window
  {
    public MissingParamDialog(string title)
    {
      InitializeComponent();
      Title = title;
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
