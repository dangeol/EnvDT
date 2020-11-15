using System;
using ExcelDataReader;
using System.IO;
using System.Data;
using EnvDT.UI.Dialogs;

namespace EnvDT.UI.Service
{
    public class ReadFileHelper : IReadFileHelper
    {
        private IMessageDialogService _messageDialogService;
        private Stream stream;

        public ReadFileHelper(IMessageDialogService messageDialogService)
        {
            _messageDialogService = messageDialogService;
        }

        public DataTable ReadFile(string file)
        {
            if (file != null && file.Length > 0)
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                try 
                { 
                    stream = File.OpenRead(file);
                }
                catch (Exception ex)
                {
                    _messageDialogService.ShowOkDialog("File error", 
                    "An error has occured. Details: " + ex.Message);
                    return null;
                }
                IExcelDataReader reader = null;

                if (file.EndsWith(".xls"))
                {
                    try 
                    { 
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    catch (Exception ex)
                    {
                        _messageDialogService.ShowOkDialog("Corrupt Excel file", "The Excel file is probably corrupt. " +
                            "Please save the file again in Excel by overwriting the same file. Details: " + ex.Message);
                        return null;
                    }
                }
                else if (file.EndsWith(".xlsx"))
                {
                    try
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }
                    catch (Exception ex)
                    {
                        _messageDialogService.ShowOkDialog("Corrupt Excel file", "The Excel file is probably corrupt. " +
                            "Please save the file again in Excel by overwriting the same file. Details: " + ex.Message);
                        return null;
                    }
                }
                else
                {
                    throw new NotSupportedException("Wrong file extension.");
                }

                DataTable workSheet = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = false
                    }
                }).Tables["Datenblatt"];

                reader.Close();
                if (workSheet == null)
                {
                    _messageDialogService.ShowOkDialog("Unknown LabReport format", "The laboratory could not be identified. " +
                        "Please check the LabReport configurator settings.");
                }
                else
                {
                    // For testing only. TO DO: implement dictionary which contains table names from 
                    // labreport configurator, which needs to be implemented.
                    // Do LabReport identification logic here.
                    var labName = "Agrolab Bruckberg";
                    workSheet.Rows[0][0] = labName;
                }

                return workSheet;
            }        
            else
            {
                throw new NotSupportedException("The file could not be read.");
            }
        }
    }
}
