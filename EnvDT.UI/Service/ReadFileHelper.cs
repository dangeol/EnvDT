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
using EnvDT.Model.Core.HelperEntity;

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

        public ImportedFileData ReadFile(string filePath)
        {
            if (filePath != null && filePath.Length > 0)
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                try
                {
                    stream = File.OpenRead(filePath);
                }
                catch (Exception ex)
                {
                    _messageDialogService.ShowOkDialog(
                        _translator["EnvDT.UI.Properties.Strings.ReadFileHelper_DialogTitle_FileError"],
                        string.Format(_translator["EnvDT.UI.Properties.Strings.ReadFileHelper_DialogMsg_FileError"],
                        ex.Message));

                    return null;
                }
                if (filePath.EndsWith(".xls") || filePath.EndsWith(".xlsx"))
                { 
                    return GetDataTableFromXlsx(filePath);
                }
                else if (filePath.EndsWith(".csv"))
                {
                    return GetDataTableFromCsv(filePath);
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

        private ImportedFileData GetDataTableFromXlsx(string filePath)
        {
            IExcelDataReader reader = null;

            if (filePath.EndsWith(".xls"))
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
            else if (filePath.EndsWith(".xlsx"))
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
            var configXlsxLookups = _lookupDataService.GetAllConfigXlsxs();
            ConfigXlsx configXlsx = null;

            foreach (var configXlsxLookUp in configXlsxLookups)
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
                && workSheet.Rows[configXlsx.IdentWordRow][configXlsx.IdentWordCol] != System.DBNull.Value)
            {
                identCellContent = workSheet.Rows[configXlsx.IdentWordRow][configXlsx.IdentWordCol].ToString();
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
                string reportLabIdent;
                try
                {
                    if (workSheet.Rows[configXlsx.ReportLabidentRow][configXlsx.ReportLabidentCol] != System.DBNull.Value)
                    {
                        reportLabIdent = workSheet.Rows[configXlsx.ReportLabidentRow][configXlsx.ReportLabidentCol].ToString();
                    }
                    else
                    {
                        DisplayReadingCellErrorMessage(nameof(reportLabIdent));
                        return null;
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    _messageDialogService.ShowOkDialog(
                        _translator["EnvDT.UI.Properties.Strings.VM_DialogTitle_OutOfRangeEx"],
                        string.Format(_translator["EnvDT.UI.Properties.Strings.VM_DialogMsg_OutOfRangeEx"],
                        "reportLabIdent", ex.Message));
                    return null;
                }

                ImportedFileData data = new ImportedFileData();
                data.WorkSheet = workSheet;
                data.ConfigId = configXlsx.ConfigXlsxId;
                data.ConfigType = "xls(x)";
                data.ReportLabIdent = reportLabIdent;
                return data;
            }
        }

        private ImportedFileData GetDataTableFromCsv(string filePath)
        {
            string reportLabIdent = "";

            using (FileStream fs1 = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (FileStream fs2 = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs1 = new BufferedStream(fs1))
            using (BufferedStream bs2 = new BufferedStream(fs2))
            using (var stream = new MemoryStream())
            using (StreamReader sr1 = new StreamReader(bs1))
            using (StreamReader sr2 = new StreamReader(bs2))
            using (StreamWriter sw = new StreamWriter(stream))
            {
                ConfigCsv configCsv = GetConfigCsvId(sr1);

                if (configCsv == null)
                {
                    _messageDialogService.ShowOkDialog(
                        _translator["EnvDT.UI.Properties.Strings.ReadFileHelper_DialogTitle_UnknLabRFormat"],
                        _translator["EnvDT.UI.Properties.Strings.ReadFileHelper_DialogMsg_UnknLabRFormat"]);
                    return null;
                }

                int headerRow = ++configCsv.HeaderRow;
                int dataRow = ++configCsv.FirstDataRow;

                string line;
                int i = 0;
                while ((line = sr2.ReadLine()) != null)
                {
                    System.Diagnostics.Debug.WriteLine("line" + i + ": " + line);
                    if (i == configCsv.ReportLabidentRow && i < headerRow)
                    {
                        reportLabIdent = line;
                    }
                    i++;
                    if (i < headerRow) 
                        continue;
                    if (i > headerRow && i < dataRow)
                        continue;

                    {                     
                        sw.WriteLine(line);
                        if (i == headerRow)
                        {
                            i++;
                            sw.WriteLine(line);
                        }
                        sw.Flush();
                    }
                }

                sw.Flush();
                stream.Position = 0;

                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = configCsv.DelimiterChar,
                    IgnoreBlankLines = false,
                    DetectColumnCountChanges = false,
                    MissingFieldFound = null
                };
                using (var reader = new StreamReader(stream))
                using (var csv = new CsvReader(reader, config))
                {
                    using (var dr = new CsvDataReader(csv))
                    {
                        var workSheet = new DataTable();
                        workSheet.Load(dr);
                        headerRow--;

                        if (configCsv.ReportLabidentRow >= headerRow)
                        {
                            configCsv.ReportLabidentRow -= headerRow;
                        }
                        if (configCsv.SampleLabIdentRow >= headerRow)
                        {
                            configCsv.SampleLabIdentRow -= headerRow;
                        }
                        if (configCsv.SampleNameRow >= headerRow)
                        {
                            configCsv.SampleNameRow -= headerRow;
                        }
                        if (configCsv.FirstDataRow >= headerRow)
                        {
                            configCsv.FirstDataRow -= headerRow;
                        }

                        if (reportLabIdent.Equals(""))
                        { 
                            try
                            {
                                if (workSheet.Rows[configCsv.ReportLabidentRow][configCsv.ReportLabidentCol] != System.DBNull.Value)
                                {
                                    reportLabIdent = workSheet.Rows[configCsv.ReportLabidentRow][configCsv.ReportLabidentCol].ToString();
                                }
                                else
                                {
                                    DisplayReadingCellErrorMessage(nameof(reportLabIdent));
                                    return null;
                                }
                            }
                            catch (IndexOutOfRangeException ex)
                            {
                                _messageDialogService.ShowOkDialog(
                                    _translator["EnvDT.UI.Properties.Strings.VM_DialogTitle_OutOfRangeEx"],
                                    string.Format(_translator["EnvDT.UI.Properties.Strings.VM_DialogMsg_OutOfRangeEx"],
                                    "reportLabIdent", ex.Message));
                                return null;
                            }
                        }

                        ImportedFileData data = new ImportedFileData();
                        data.WorkSheet = workSheet;
                        data.ConfigId = configCsv.ConfigCsvId;
                        data.ConfigCsv = configCsv;
                        data.ConfigType = "csv";
                        data.ReportLabIdent = reportLabIdent;
                        return data;
                    }
                }
            }
        }

        private ConfigCsv GetConfigCsvId(StreamReader sr)
        {
            var configCsvs = _unitOfWork.ConfigCsvs.GetAll();

            foreach (var configCsv in configCsvs)
            {
                var identWord = configCsv.IdentWord;
                var identWordRow = configCsv.IdentWordRow;
                string line;
                int i = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    if (i == identWordRow && HasLabCheckPassed(line, identWord))
                    {
                        return configCsv;
                    }
                    i++;
                }
            }

            return null;
        }

        private bool HasLabCheckPassed(string identCellContent, string identWord)
        {
            if (identCellContent.IndexOf(identWord, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            return false;
        }

        private void DisplayReadingCellErrorMessage(string variableName)
        {
            _messageDialogService.ShowOkDialog(
                _translator["EnvDT.UI.Properties.Strings.ImportLabReportService_DialogTitle_CellError"],
                string.Format(_translator["EnvDT.UI.Properties.Strings.ImportLabReportService_DialogMsg_CellError"],
                variableName));
        }
    }
}
