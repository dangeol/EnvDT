using EnvDT.UI.Dialogs;
using EnvDT.UI.Settings.Localization;
using EnvDT.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace EnvDT.UI.Service
{
    // Some principal ideas and inspiration taken from https://stackoverflow.com/a/12899714
    // The code was developped from scratch to meet the needs of this app
    public class ExcelXmlReader : IExcelXmlReader
    {
        private IMessageDialogService _messageDialogService;
        private TranslationSource _translator = TranslationSource.Instance;
        private List<string> _badDataList = new List<string>();
        private IDispatcher _dispatcher;

        public ExcelXmlReader(IMessageDialogService messageDialogService, IDispatcher dispatcher)
        {
            _messageDialogService = messageDialogService;
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }
            _dispatcher = dispatcher;
        }

        public DataSet ReadExcelXml(Stream stream)
        {
            if (stream.Position > 0)
            {
                stream.Position = 0;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(new XmlTextReader(stream));
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);

            nsmgr.AddNamespace("o", "urn:schemas-microsoft-com:office:office");
            nsmgr.AddNamespace("x", "urn:schemas-microsoft-com:office:excel");
            nsmgr.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet");

            DataSet dataSet = new DataSet();

            foreach (XmlNode node in
                doc.DocumentElement.SelectNodes("//ss:Worksheet", nsmgr))
            {
                DataTable dataTable = new DataTable(node.Attributes["ss:Name"].Value);
                dataSet.Tables.Add(dataTable);
                XmlNodeList? rows = node.SelectNodes("ss:Table/ss:Row", nsmgr);

                if (rows.Count > 0)
                {
                    int startIndex = 0;

                    for (int i = startIndex; i < rows.Count; i++)
                    {
                        DataRow row = dataTable.NewRow();
                        XmlNodeList cells = rows[i].SelectNodes("ss:Cell", nsmgr);
                        int actualCellIndex = 0;
                        for (int cellIndex = 0; cellIndex < cells.Count; cellIndex++)
                        {
                            XmlNode cell = cells[cellIndex];
                            if (cell.Attributes["ss:Index"] != null)
                            {
                                actualCellIndex = int.Parse(cell.Attributes["ss:Index"].Value) - 1;
                            }

                            XmlNode data = cell.SelectSingleNode("ss:Data", nsmgr);

                            if (actualCellIndex >= dataTable.Columns.Count)
                            {
                                for (int j = dataTable.Columns.Count; j <= actualCellIndex; j++)
                                {
                                    if (!dataTable.Columns.Contains("Column" + j.ToString()))
                                    {
                                        dataTable.Columns.Add("Column" + j.ToString());
                                    }
                                }
                            }

                            if (data != null)
                            {
                                WriteCellData(row, actualCellIndex, data);
                            }

                            if (cell.Attributes["ss:MergeAcross"] != null)
                            {
                                actualCellIndex += int.Parse(cell.Attributes["ss:MergeAcross"].Value) + 1;
                            }
                            else
                            {
                                actualCellIndex++;
                            }
                        }

                        // Last row could be empty.
                        if (i < rows.Count - 1 || i == rows.Count - 1
                            && (!row.ItemArray.All(field => field is System.DBNull ||
                                string.Compare((field as string).Trim(), string.Empty) == 0)))
                        {
                            dataTable.Rows.Add(row);
                        }

                    }
                }
            }
            if (_badDataList.Count > 0)
            {
                var badDataListJoined = string.Join(", ", _badDataList);
                _dispatcher.Invoke(() =>
                {
                    _messageDialogService.ShowOkDialog(
                    _translator["EnvDT.UI.Properties.Strings.DialogTitle_FormatError"],
                    string.Format(_translator["EnvDT.UI.Properties.Strings.DialogMsg_FormatError"],
                    badDataListJoined));
                });
            }

            return dataSet;
        }

        private void WriteCellData(DataRow row, int actualCellIndex, XmlNode data)
        {
            if (data.Attributes["ss:Type"] != null)
            {
                switch (data.Attributes["ss:Type"].Value)
                {
                    case "String":
                        row[actualCellIndex] = data.InnerText;
                        break;
                    case "Number":
                        try
                        {
                            row[actualCellIndex] = data.InnerText.Contains(",") && !data.InnerText.Contains(".") ?
                            double.Parse(data.InnerText, new CultureInfo("de-DE")) :
                            double.Parse(data.InnerText, CultureInfo.InvariantCulture);
                        }
                        catch (FormatException)
                        {
                            _badDataList.Add(data.InnerText);
                        }
                        break;
                    default:
                        row[actualCellIndex] = data.InnerText;
                        break;
                }
            }
            else
            {
                row[actualCellIndex] = data.InnerText;
            }
        }
    }
}
