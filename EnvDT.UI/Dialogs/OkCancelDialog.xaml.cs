using System.Windows;

namespace EnvDT.UI.Dialogs
{
  public partial class OkCancelDialog : Window
  {
    public OkCancelDialog(string title, string message )
    {
      InitializeComponent();
      Title = title;
      textBlock.Text = message;
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
