using System;
using ExcelDataReader;
using System.IO;
using System.Data;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Settings.Localization;
using EnvDT.Model.IRepository;
using EnvDT.Model.IDataService;
using EnvDT.Model.Entity;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;

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
                if (file.EndsWith(".xls") || file.EndsWith(".xlsx"))
                { 
                    return GetDataTableFromXlsx(file);
                }
                else if (file.EndsWith(".csv"))
                {
                    return GetDataTableFromCsv(file);
                }
                else
                {
                    throw new NotSupportedException("Wrong file extension.");
                }
            }
            else
            {
                throw new NotSupportedException("The file could not be read.");
            }
        }

        private DataTable GetDataTableFromXlsx(string file)
        {
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

            string identCellContent = "";
            if (workSheet != null && configXlsx != null
                && workSheet.Rows[configXlsx.IdentWordCol][configXlsx.IdentWordRow] != System.DBNull.Value)
            {
                identCellContent = workSheet.Rows[configXlsx.IdentWordCol][configXlsx.IdentWordRow].ToString();
            }

            if (!HasLabCheckPassed(identCellContent, configXlsx.IdentWord))
            {
                _messageDialogService.ShowOkDialog(
                    _translator["EnvDT.UI.Properties.Strings.ReadFileHelper_DialogTitle_UnknLabRFormat"],
                    _translator["EnvDT.UI.Properties.Strings.ReadFileHelper_DialogMsg_UnknLabRFormat"]);
                return null;
            }
            else
            {
                workSheet.Rows[0][0] = configXlsx.ConfigXlsxId;
                workSheet.Rows[0][1] = "xls(x)";
                return workSheet;
            }
        }

        private DataTable GetDataTableFromCsv(string file)
        {
            var reader = new StreamReader(file);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                NewLine = Environment.NewLine,
                MissingFieldFound = null
            };
            using var csvReader = new CsvReader(reader, config);
            using var dr = new CsvDataReader(csvReader);

            var workSheet = new DataTable();
            workSheet.Load(dr);

            var configCsvs = _lookupDataService.GetAllConfigCsvs();
            Guid configCsvId = Guid.Empty;

            string identCellContent = "";

            foreach (var configCsvLookUp in configCsvs)
            {
                if (workSheet != null 
                    && workSheet.Rows[configCsvLookUp.IdentWordCol][configCsvLookUp.IdentWordRow] != System.DBNull.Value)
                {
                    identCellContent = workSheet.Rows[configCsvLookUp.IdentWordCol][configCsvLookUp.IdentWordRow].ToString();
                }
                if (HasLabCheckPassed(identCellContent, configCsvLookUp.IdentWord))
                {
                    configCsvId = _unitOfWork.ConfigCsvs.GetByIdUpdated(configCsvLookUp.LookupItemId).ConfigCsvId;
                    break;
                }
            }

            if (Guid.Equals(configCsvId, Guid.Empty))
            {
                _messageDialogService.ShowOkDialog(
                    _translator["EnvDT.UI.Properties.Strings.ReadFileHelper_DialogTitle_UnknLabRFormat"],
                    _translator["EnvDT.UI.Properties.Strings.ReadFileHelper_DialogMsg_UnknLabRFormat"]);
                return null;
            }
            else 
            { 
                workSheet.Rows[0][0] = configCsvId;
                workSheet.Rows[0][1] = "csv";
                return workSheet;
            }                         
        }

        private bool HasLabCheckPassed(string identCellContent, string identWord)
        {
            if (identCellContent.IndexOf(identWord, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            return false;
        }
    }
}
