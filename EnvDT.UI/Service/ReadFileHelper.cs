using System;
using ExcelDataReader;
using System.IO;
using System.Data;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Settings.Localization;
using EnvDT.Model.IRepository;
using EnvDT.Model.IDataService;
using EnvDT.Model.Entity;

namespace EnvDT.UI.Service
{
    public class ReadFileHelper : IReadFileHelper
    {
        private IMessageDialogService _messageDialogService;
        private IUnitOfWork _unitOfWork;
        private ILookupDataService _lookupDataService;
        private Stream stream;
        private TranslationSource _translator = TranslationSource.Instance;

        public ReadFileHelper(IMessageDialogService messageDialogService, IUnitOfWork unitOfWork, 
            ILookupDataService lookupDataService)
        {
            _messageDialogService = messageDialogService;
            _unitOfWork = unitOfWork;
            _lookupDataService = lookupDataService;
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

                DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = false
                    }
                });

                DataTableCollection workSheets = result.Tables;
                reader.Close();

                DataTable workSheet = null;
                var configXlsxs = _lookupDataService.GetAllConfigXlsxs();
                ConfigXlsx configXlsx = null;

                foreach (var configXlsxLookUp in configXlsxs)
                {
                    workSheet = workSheets[configXlsxLookUp.DisplayMember];
                    if (workSheet != null)
                    {                     
                        configXlsx = _unitOfWork.ConfigXlsxs.GetByIdUpdated(configXlsxLookUp.LookupItemId);
                        break;
                    }
                }

                if (!HasLabCheckPassed(workSheet, configXlsx))
                {
                    _messageDialogService.ShowOkDialog(
                        _translator["EnvDT.UI.Properties.Strings.ReadFileHelper_DialogTitle_UnknLabRFormat"],
                        _translator["EnvDT.UI.Properties.Strings.ReadFileHelper_DialogMsg_UnknLabRFormat"]);
                    return null;
                }
                else
                {
                    workSheet.Rows[0][0] = configXlsx.ConfigXlsxId;
                    return workSheet;
                }
            }        
            else
            {
                throw new NotSupportedException("The file could not be read.");
            }
        }

        private bool HasLabCheckPassed(DataTable workSheet, ConfigXlsx configXlsx)
        {
            if (workSheet != null && configXlsx != null
                && workSheet.Rows[configXlsx.IdentWordCol][configXlsx.IdentWordRow] != System.DBNull.Value)
            {
                var identCellContent = workSheet.Rows[configXlsx.IdentWordCol][configXlsx.IdentWordRow].ToString();
                if (identCellContent.IndexOf(configXlsx.IdentWord, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
