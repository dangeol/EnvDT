using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EnvDT.UI.View
{
    public partial class SampleDetailView : UserControl
    {
        public SampleDetailView()
        {
            InitializeComponent();
        }

        public void CopyToClipboard(object obj, RoutedEventArgs e)
        {
            EvalResultsDataGrid.SelectAllCells();
            EvalResultsDataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, EvalResultsDataGrid);
            EvalResultsDataGrid.UnselectAllCells();
        }
    }
}
