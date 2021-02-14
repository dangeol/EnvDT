using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Event;
using EnvDT.UI.Settings.Localization;
using Microsoft.EntityFrameworkCore;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace EnvDT.UI.Service
{
    public class ImportLabReportService : IImportLabReportService
    {
        private const int _configIdCol = 0;
        private const int _configIdRow = 0;
        private const int _configTypeCol = 1;
        private const int _configTypeRow = 0;

        private IEventAggregator _eventAggregator;
        private IMessageDialogService _messageDialogService;
        private IUnitOfWork _unitOfWork;
        private IReadFileHelper _readFileHelper;
        private List<Sample> _samples = new List<Sample>();
        private TranslationSource _translator = TranslationSource.Instance;

        public ImportLabReportService(IEventAggregator eventAggregator, IMessageDialogService messageDialogService,
            IUnitOfWork unitOfWork, IReadFileHelper readFileHelper)
        {
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;
            _unitOfWork = unitOfWork;
            _readFileHelper = readFileHelper;
        }

        public void RunImport(string file, Guid? projectId)
        {
            DataTable workSheet = _readFileHelper.ReadFile(file);
            if (workSheet != null)
            {
                ConfigBase configBase = new ConfigBase();
                try
                {
                    if (workSheet.Rows[_configIdRow][_configIdCol] != System.DBNull.Value
                        && workSheet.Rows[_configTypeRow][_configTypeCol] != System.DBNull.Value)
                    {
                        var configId = Guid.Parse((workSheet.Rows[_configIdRow][_configIdCol]).ToString());
                        var configType = workSheet.Rows[_configTypeRow][_configTypeCol].ToString();

                        switch (configType)
                        {
                            case "xls(x)":
                                configBase = _unitOfWork.ConfigXlsxs.GetById(configId);
                                break;

                            case "csv":
                                configBase = _unitOfWork.ConfigCsvs.GetById(configId);
                                break;

                            default:
                                _messageDialogService.ShowOkDialog(
                                    _translator["EnvDT.UI.Properties.Strings.ReadFileHelper_DialogTitle_UnknLabRFormat"],
                                    _translator["EnvDT.UI.Properties.Strings.ReadFileHelper_DialogMsg_UnknLabRFormat"]);
                                break;
                        }
                    }
                    else
                    {
                        DisplayReadingCellErrorMessage(nameof(configBase));
                        return;
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    _messageDialogService.ShowOkDialog(
                        _translator["EnvDT.UI.Properties.Strings.VM_DialogTitle_OutOfRangeEx"],
                        string.Format(_translator["EnvDT.UI.Properties.Strings.VM_DialogMsg_OutOfRangeEx"],
                        nameof(configBase), ex.Message));
                    return;
                }
                ImportLabReport(workSheet, configBase, projectId);
            }
        }

        private void ImportLabReport(DataTable workSheet, ConfigBase configBase, Guid? projectId)
        {
            string reportLabIdent;
            try
            {
                if (workSheet.Rows[configBase.ReportLabidentRow][configBase.ReportLabidentCol] != System.DBNull.Value)
                {
                    reportLabIdent = workSheet.Rows[configBase.ReportLabidentRow][configBase.ReportLabidentCol].ToString();
                }
                else
                {
                    DisplayReadingCellErrorMessage(nameof(reportLabIdent));
                    return;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                _messageDialogService.ShowOkDialog(
                    _translator["EnvDT.UI.Properties.Strings.VM_DialogTitle_OutOfRangeEx"],
                    string.Format(_translator["EnvDT.UI.Properties.Strings.VM_DialogMsg_OutOfRangeEx"],
                    "reportLabIdent", ex.Message));
                return;
            }
            if (IsLabReportAlreadyPresent(reportLabIdent))
            {
                return;
            }

            Laboratory laboratory = _unitOfWork.Laboratories.GetById(configBase.LaboratoryId);
            var labCompany = laboratory.LabCompany;
            Guid labReportId = CreateLabReport(reportLabIdent, laboratory.LaboratoryId, projectId).LabReportId;

            int c = configBase.FirstSampleValueCol;
            while (c < workSheet.Columns.Count)
            {
                string sampleLabIdent;
                try
                {
                    if (workSheet.Rows[configBase.SampleLabIdentRow][c] != System.DBNull.Value)
                    {
                        sampleLabIdent = workSheet.Rows[configBase.SampleLabIdentRow][c].ToString();
                    }
                    else
                    {
                        DisplayReadingCellErrorMessage(nameof(sampleLabIdent));
                        return;
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    _messageDialogService.ShowOkDialog(
                        _translator["EnvDT.UI.Properties.Strings.VM_DialogTitle_OutOfRangeEx"],
                        string.Format(_translator["EnvDT.UI.Properties.Strings.VM_DialogMsg_OutOfRangeEx"],
                        "sampleLabIdent", ex.Message));
                    return;
                }
                string sampleName;
                try
                {
                    if (workSheet.Rows[configBase.SampleNameRow][c] != System.DBNull.Value)
                    {
                        sampleName = workSheet.Rows[configBase.SampleNameRow][c].ToString();
                    }
                    else
                    {
                        DisplayReadingCellErrorMessage(nameof(sampleName));
                        return;
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    _messageDialogService.ShowOkDialog(
                        _translator["EnvDT.UI.Properties.Strings.VM_DialogTitle_OutOfRangeEx"],
                        string.Format(_translator["EnvDT.UI.Properties.Strings.VM_DialogMsg_OutOfRangeEx"],
                        "sampleName", ex.Message));
                    return;
                }

                var existingSample = _samples.FirstOrDefault(s => s.SampleName == sampleName);
                if (existingSample != null)
                {
                    existingSample.SampleLabIdent += "_" + sampleLabIdent;
                }
                else 
                { 
                    Sample sample = CreateSample(
                        sampleLabIdent, sampleName, labReportId
                    );
                    _samples.Add(sample);
                }
                c++;
            }
            CreateLabReportParams(workSheet, configBase, labReportId);
          
            try
            {
                _unitOfWork.Save();

                RaiseLabReportImportedEvent(labReportId,
                $"{reportLabIdent} {labCompany}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _messageDialogService.ShowOkDialog(
                    _translator["EnvDT.UI.Properties.Strings.VM_DialogTitle_Error"],
                    string.Format(_translator["EnvDT.UI.Properties.Strings.VM_DialogMsg_Error"],
                    ex.Message));
            }
        }

        public bool IsLabReportAlreadyPresent(string reportLabIdent)
        {
            var foundLabReport = _unitOfWork.LabReports.GetByReportLabIdent(reportLabIdent);
            if (foundLabReport != null) { 
                var result = _messageDialogService.ShowOkDialog(
                    _translator["EnvDT.UI.Properties.Strings.ImportLabReportService_DialogTitle_ImportLabReport"],
                    _translator["EnvDT.UI.Properties.Strings.ImportLabReportService_DialogMsg_ImportLabReport"]);

                if (result == MessageDialogResult.OK)
                {
                }
                return true;
            }
            return false;
        }

        private LabReport CreateLabReport(string reportLabIdent, Guid laboratoryId, Guid? projectId)
        {
            var labReport = new LabReport();
            labReport.ReportLabIdent = reportLabIdent;
            labReport.LaboratoryId = laboratoryId;
            labReport.ProjectId = (Guid)projectId;

            _unitOfWork.LabReports.Create(labReport);

            return labReport;
        }

        private Sample CreateSample(string sampleLabIdent, string sampleName, Guid labReportId)
        {
            var sample = new Sample();
            sample.SampleLabIdent = sampleLabIdent;
            sample.SampleName = sampleName;
            sample.LabReportId = labReportId;

            _unitOfWork.Samples.Create(sample);

            return sample;
        }

        private void CreateLabReportParams(DataTable workSheet, ConfigBase configBase, Guid labReportId)
        {
            int r = configBase.FirstDataRow;
            while (r < workSheet.Rows.Count)
            {
                string labParamName;
                try
                { 
                    if (workSheet.Rows[r][configBase.ParamNameCol] != System.DBNull.Value)
                    {
                        labParamName = workSheet.Rows[r][configBase.ParamNameCol].ToString();
                    }
                    else
                    {
                        DisplayReadingCellErrorMessage(nameof(labParamName));
                        return;
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    _messageDialogService.ShowOkDialog(
                        _translator["EnvDT.UI.Properties.Strings.VM_DialogTitle_OutOfRangeEx"],
                        string.Format(_translator["EnvDT.UI.Properties.Strings.VM_DialogMsg_OutOfRangeEx"],
                        "ParamName", ex.Message));
                    return;
                }
                var labParamUnitName = workSheet.Rows[r][configBase.UnitNameCol].ToString();
                var paramNameVariants = _unitOfWork.ParamNameVariants.GetParamNameVariantsByLabParamName(labParamName);
                var unitId = GetUnitNameVariantByLabParamUnitName(labParamUnitName);

                if (paramNameVariants.Count() > 0)
                {
                    foreach (var paramNameVariant in paramNameVariants)
                    {
                        var parameterId = paramNameVariant.ParameterId;
                        CreateNewLabParam(workSheet, configBase, labParamName, labParamUnitName, 
                            parameterId, unitId, labReportId, r);   
                    }
                }
                else
                {
                    var unknownParameterId = _unitOfWork.Parameters.GetParameterIdOfUnknown();
                    CreateNewLabParam(workSheet, configBase, labParamName, labParamUnitName, 
                        unknownParameterId, unitId, labReportId, r);
                }
                r++;
            }
        }

        private Guid GetUnitNameVariantByLabParamUnitName(string labParamUnitName)
        {
            var unitName = labParamUnitName;
            var unitId = _unitOfWork.UnitNameVariants.GetUnitNameVariantByLabParamUnitName(unitName)?.UnitId
                ?? _unitOfWork.Units.GetUnitIdOfUnknown();

            return unitId;
        }

        // TO DO: refactoring
        private void CreateNewLabParam(DataTable workSheet, ConfigBase configBase, string labReportParamName,
            string labReportUnitName, Guid parameterId, Guid unitId, Guid labReportId, int r)
        {
            var labReportParam = new LabReportParam();
            labReportParam.ParameterId = parameterId;
            labReportParam.LabReportParamName = labReportParamName;
            labReportParam.LabReportUnitName = labReportUnitName;
            labReportParam.UnitId = unitId;
            labReportParam.LabReportId = labReportId;
            labReportParam.DetectionLimit = 0.0;

            try
            {
                if (workSheet.Rows[r][configBase.DetectionLimitCol] != System.DBNull.Value)
                {
                    labReportParam.DetectionLimit = (double)workSheet.Rows[r][configBase.DetectionLimitCol];
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                _messageDialogService.ShowOkDialog(
                    _translator["EnvDT.UI.Properties.Strings.VM_DialogTitle_OutOfRangeEx"],
                    string.Format(_translator["EnvDT.UI.Properties.Strings.VM_DialogMsg_OutOfRangeEx"],
                    "DetectionLimit", ex.Message));
                return;
            }

            labReportParam.Method = "";

            try
            {
                if (workSheet.Rows[r][configBase.MethodCol] != System.DBNull.Value)
                {
                    labReportParam.Method = (string)workSheet.Rows[r][configBase.MethodCol];
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                _messageDialogService.ShowOkDialog(
                    _translator["EnvDT.UI.Properties.Strings.VM_DialogTitle_OutOfRangeEx"],
                    string.Format(_translator["EnvDT.UI.Properties.Strings.VM_DialogMsg_OutOfRangeEx"],
                    "Method", ex.Message));
                return;
            }

            _unitOfWork.LabReportParams.Create(labReportParam);
            CreateNewSampleValue(r, workSheet, configBase, labReportParam);
        }

        private void CreateNewSampleValue(int r, DataTable workSheet, ConfigBase configBase, LabReportParam labReportParam)
        {
            var c = configBase.FirstSampleValueCol;
            while (c < workSheet.Columns.Count)
            {
                try
                {
                    if (workSheet.Rows[r][c] != System.DBNull.Value)
                    {
                        var sValue = 0.0;
                        string sampleName;
                        Guid sampleId = Guid.Empty;

                        try
                        {
                            if (workSheet.Rows[configBase.SampleNameRow][c] != System.DBNull.Value)
                            {
                                sampleName = workSheet.Rows[configBase.SampleNameRow][c].ToString();
                                sampleId = _samples.FirstOrDefault(s => s.SampleName == sampleName).SampleId;
                            }
                        }
                        catch (IndexOutOfRangeException ex)
                        {
                            _messageDialogService.ShowOkDialog(
                                _translator["EnvDT.UI.Properties.Strings.VM_DialogTitle_OutOfRangeEx"],
                                string.Format(_translator["EnvDT.UI.Properties.Strings.VM_DialogMsg_OutOfRangeEx"],
                                "SampleName", ex.Message));
                            return;
                        }

                        double testVar;
                        if (Double.TryParse(workSheet.Rows[r][c].ToString(), out testVar))
                        {
                            sValue = (double)workSheet.Rows[r][c];
                        }
                        var sampleValue = new SampleValue();
                        sampleValue.SValue = sValue;
                        sampleValue.LabReportParamId = labReportParam.LabReportParamId;

                        if (sampleId != Guid.Empty)
                        {
                            sampleValue.SampleId = sampleId;
                            _unitOfWork.SampleValues.Create(sampleValue);
                        }
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    _messageDialogService.ShowOkDialog(
                        _translator["EnvDT.UI.Properties.Strings.VM_DialogTitle_OutOfRangeEx"],
                        string.Format(_translator["EnvDT.UI.Properties.Strings.VM_DialogMsg_OutOfRangeEx"],
                        "FirstSampleValue", ex.Message));
                    return;
                }
                c++;
            }
        }

        private void RaiseLabReportImportedEvent(Guid modelId, string displayMember)
        {
            _eventAggregator.GetEvent<LabReportImportedEvent>()
                .Publish(
                    new LabReportImportedEventArgs
                    {
                        Id = modelId,
                        DisplayMember = displayMember
                    });
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
