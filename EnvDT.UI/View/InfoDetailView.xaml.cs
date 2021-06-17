using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace EnvDT.UI.View
{
    public partial class InfoDetailView : UserControl
    {
        public InfoDetailView()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
