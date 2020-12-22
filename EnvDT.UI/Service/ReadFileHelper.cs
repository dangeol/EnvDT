using System;
using ExcelDataReader;
using System.IO;
using System.Data;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Settings.Localization;

namespace EnvDT.UI.Service
{
    public class ReadFileHelper : IReadFileHelper
    {
        private IMessageDialogService _messageDialogService;
        private Stream stream;
        private TranslationSource _translator = TranslationSource.Instance;

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
                    _messageDialogService.ShowOkDialog(
                        _translator["EnvDT.UI.Properties.Strings.ReadFileHelper_DialogTitle_FileError"],
                        string.Format(_translator["EnvDT.UI.Properties.Strings.ReadFileHelper_DialogMsg_FileError"],
                        ex.Message));

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
                        _messageDialogService.ShowOkDialog(
                            _translator["EnvDT.UI.Properties.Strings.ReadFileHelper_DialogTitle_CorruptExcel"],
                            string.Format(_translator["EnvDT.UI.Properties.Strings.ReadFileHelper_DialogMsg_CorruptExcel"],
                            ex.Message));

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
                        _messageDialogService.ShowOkDialog(
                            _translator["EnvDT.UI.Properties.Strings.ReadFileHelper_DialogTitle_CorruptExcel"],
                            string.Format(_translator["EnvDT.UI.Properties.Strings.ReadFileHelper_DialogMsg_CorruptExcel"],
                            ex.Message));

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
                    _messageDialogService.ShowOkDialog(
                        _translator["EnvDT.UI.Properties.Strings.ReadFileHelper_DialogTitle_UnknLabRFormat"],
                        _translator["EnvDT.UI.Properties.Strings.ReadFileHelper_DialogMsg_UnknLabRFormat"]);
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
