using System.Windows;

namespace EnvDT.UI.Dialogs
{
  public partial class OkDialog : Window
  {
    public OkDialog(string title, string message )
    {
      InitializeComponent();
      Title = title;
      textBlock.Text = message;
    }

    private void ButtonOk_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
    }
  }
}
