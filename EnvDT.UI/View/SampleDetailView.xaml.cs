﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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
            if (!e.Column.Header.ToString().Equals("Sample") &&
                !e.Column.Header.ToString().Equals("Probe"))
            {
                e.Column.HeaderStyle = Application.Current.FindResource("ColumnHeaderRotateStyle") as Style;
            }
        }

        private void DataGrid_AutoGeneratingColumn_PublAndFootnotes(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString().Equals("Key"))
            {
                e.Column.Width = 10; 
            }
        }

        public void CopyToClipboard(object obj, RoutedEventArgs e)
        {
            EvalResultsDataGrid.SelectAllCells();
            EvalResultsDataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, EvalResultsDataGrid);
            EvalResultsDataGrid.UnselectAllCells();
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
