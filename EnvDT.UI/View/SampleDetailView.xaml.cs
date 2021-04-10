using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace EnvDT.UI.View
{
    public partial class SampleDetailView : UserControl
    {
        public SampleDetailView()
        {
            InitializeComponent();
        }

        private void DataGrid_AutoGeneratingColumn_SampleDataView(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString().Equals("Sample") ||
                e.Column.Header.ToString().Equals("Probe"))
            {
                e.Column.CellStyle = Application.Current.FindResource("WhiteDataGridCellStyle") as Style;
            }
            else
            {              
                e.Column.HeaderStyle = Application.Current.FindResource("ColumnHeaderRotateStyle") as Style;
            }
        }

        private void DataGrid_AutoGeneratingColumn_EvalResultDataView(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            // The following has been taken without modification from https://stackoverflow.com/a/55400647

            var columnName = (string)e.Column.Header;
          
            var bindingBuilder = new StringBuilder(columnName.Length * 2 + 2);

            bindingBuilder.Append('[');
            foreach (var c in columnName)
            {
                bindingBuilder.Append('^');
                bindingBuilder.Append(c);
            }
            bindingBuilder.Append(']');

            e.Column = new DataGridTextColumn
            {
                Binding = new Binding(bindingBuilder.ToString()),
                Header = e.Column.Header,
            };

            if (e.Column.Header.ToString().Equals("SortCol"))
            {
                e.Column.Visibility = Visibility.Collapsed;
            }
        }

        private void DataGrid_AutoGeneratingColumn_PublAndFootnotes(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString().Equals("Key"))
            {
                e.Column.Width = 10; 
            }

            //TO DO: fix this - text wrapping does not work here
            var col = e.Column as DataGridTextColumn;
            if (col != null)
            {
                var style = new Style(typeof(TextBlock));
                style.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.WrapWithOverflow));
                style.Setters.Add(new Setter(VerticalAlignmentProperty, VerticalAlignment.Top));

                col.ElementStyle = style;
            }
        }

        public void CopyToClipboard_SelectedPublsDataGrid(object obj, RoutedEventArgs e)
        {
            SelectedPublsDataGrid.SelectAllCells();
            SelectedPublsDataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
            ApplicationCommands.Copy.Execute(null, SelectedPublsDataGrid);
            SelectedPublsDataGrid.UnselectAllCells();
        }

        public void CopyToClipboard_EvalResultsDataGrid(object obj, RoutedEventArgs e)
        {
            EvalResultsDataGrid.SelectAllCells();
            EvalResultsDataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, EvalResultsDataGrid);
            EvalResultsDataGrid.UnselectAllCells();
        }

        public void CopyToClipboard_FootnotesDataGrid(object obj, RoutedEventArgs e)
        {
            FootnotesDataGrid.SelectAllCells();
            FootnotesDataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
            ApplicationCommands.Copy.Execute(null, FootnotesDataGrid);
            FootnotesDataGrid.UnselectAllCells();
        }

        // Taken from the following source and slightly modified:
        // http://wpfthoughts.blogspot.com/2018/04/put-cell-into-edit-mode-when-clicked.html
        private void DataGrid_Selected(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                DataGrid g = (DataGrid)sender;
                g.BeginEdit(e);
                DataGridCell Cell = e.OriginalSource as DataGridCell;
                List<CheckBox> cb = FindChildrenByType<CheckBox>(Cell);
                if (cb.Count > 0)
                {
                    cb[0].Focus();
                    cb[0].IsChecked = !cb[0].IsChecked;
                }
            }
        }

        public static List<T> FindChildrenByType<T>(DependencyObject depObj) where T : DependencyObject
        {
            List<T> Children = new List<T>();
            if (depObj != null)
            {
                for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(depObj) - 1; i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        Children.Add((T)child);
                    }
                    Children.AddRange(FindChildrenByType<T>(child));
                }
            }
            return Children;
        }
    }
}
