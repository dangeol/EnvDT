using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Event;
using EnvDT.UI.Settings.Localization;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace EnvDT.UI.Service
{
    public class ImportLabReportService : IImportLabReportService
    {
        private const string _worksheetName = "Datenblatt";
        private const int _labNameCol = 0;
        private const int _labNameRow = 0;
        private const int _reportLabidentCol = 4;
        private const int _reportLabidentRow = 2;
        private const int _firstSampleValueCol = 4;
        private const int _sampleLabIdentRow = 3;
        private const int _sampleNameRow = 4;
        private const int _firstDataRow = 7;
        private const int _paramNameCol = 0;
        private const int _unitNameCol = 1;
        private const int _detectionLimitCol = 2;
        private const int _methodCol = 3;

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
                ImportLabReport(workSheet, projectId);
            }
        }

        private void ImportLabReport(DataTable workSheet, Guid? projectId)
        {
            string reportLabIdent;
            if (workSheet.Rows[_reportLabidentRow][_reportLabidentCol] != System.DBNull.Value)
            { 
                reportLabIdent = workSheet.Rows[_reportLabidentRow][_reportLabidentCol].ToString();
            }
            else
            {
                DisplayReadingCellErrorMessage(nameof(reportLabIdent));
                return;
            }
            if (IsLabReportAlreadyPresent(reportLabIdent))
            {
                return;
            }

            string labName;
            if (workSheet.Rows[_labNameRow][_labNameCol] != System.DBNull.Value)
            {
                labName = workSheet.Rows[_labNameRow][_labNameCol].ToString();
            }
            else
            {
                DisplayReadingCellErrorMessage(nameof(labName));
                return;
            }
            var labCompany = _unitOfWork.LabReports.GetLabByLabName(labName).LabCompany;
            Guid labReportId = CreateLabReport(reportLabIdent, labName, projectId).LabReportId;

            int c = _firstSampleValueCol;
            while (c < workSheet.Columns.Count)
            {
                string sampleLabIdent;
                if (workSheet.Rows[_sampleLabIdentRow][c] != System.DBNull.Value)
                {
                    sampleLabIdent = workSheet.Rows[_sampleLabIdentRow][c].ToString();
                }
                else
                {
                    DisplayReadingCellErrorMessage(nameof(sampleLabIdent));
                    return;
                }
                string sampleName;
                if (workSheet.Rows[_sampleNameRow][c] != System.DBNull.Value)
                {
                    sampleName = workSheet.Rows[_sampleNameRow][c].ToString();
                }
                else
                {
                    DisplayReadingCellErrorMessage(nameof(sampleName));
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
            CreateLabReportParams(workSheet, labReportId);

            _unitOfWork.Save();

            RaiseLabReportImportedEvent(labReportId,
                $"{reportLabIdent} {labCompany}");
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

        private LabReport CreateLabReport(string reportLabIdent, string labName, Guid? projectId)
        {
            Guid laboratoryId = _unitOfWork.LabReports.GetLabByLabName(labName).LaboratoryId;

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

        private void CreateLabReportParams(DataTable workSheet, Guid labReportId)
        {
            int r = _firstDataRow;
            while (r < workSheet.Rows.Count)
            {
                string labParamName;
                if (workSheet.Rows[r][_paramNameCol] != System.DBNull.Value)
                {
                    labParamName = workSheet.Rows[r][_paramNameCol].ToString();
                }
                else
                {
                    DisplayReadingCellErrorMessage(nameof(labParamName));
                    return;
                }
                var labParamUnitName = workSheet.Rows[r][_unitNameCol].ToString();
                var paramNameVariants = _unitOfWork.ParamNameVariants.GetParamNameVariantsByLabParamName(labParamName);
                var unitId = GetUnitNameVariantByLabParamUnitName(labParamUnitName);

                if (paramNameVariants.Count() > 0)
                {
                    foreach (var paramNameVariant in paramNameVariants)
                    {
                        var parameterId = paramNameVariant.ParameterId;
                        CreateNewLabParam(workSheet, labParamName, labParamUnitName, 
                            parameterId, unitId, labReportId, r);   
                    }
                }
                else
                {
                    var unknownParameterId = _unitOfWork.Parameters.GetParameterIdOfUnknown();
                    CreateNewLabParam(workSheet, labParamName, labParamUnitName, 
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
        private void CreateNewLabParam(DataTable workSheet, string labReportParamName, string labReportUnitName,
            Guid parameterId, Guid unitId, Guid labReportId, int r)
        {
            var labReportParam = new LabReportParam();
            labReportParam.ParameterId = parameterId;
            labReportParam.LabReportParamName = labReportParamName;
            labReportParam.LabReportUnitName = labReportUnitName;
            labReportParam.UnitId = unitId;
            labReportParam.LabReportId = labReportId;
            labReportParam.DetectionLimit = 0.0;
            if (workSheet.Rows[r][_detectionLimitCol] != System.DBNull.Value)
            {
                labReportParam.DetectionLimit = (double)workSheet.Rows[r][_detectionLimitCol];
            }
            labReportParam.Method = "";
            if (workSheet.Rows[r][_methodCol] != System.DBNull.Value)
            {
                labReportParam.Method = (string)workSheet.Rows[r][_methodCol];
            }

            _unitOfWork.LabReportParams.Create(labReportParam);
            CreateNewSampleValue(r, workSheet, labReportParam);
        }

        private void CreateNewSampleValue(int r, DataTable workSheet, LabReportParam labReportParam)
        {
            var c = _firstSampleValueCol;
            while (c < workSheet.Columns.Count)
            {
                if (workSheet.Rows[r][c] != System.DBNull.Value)
                {
                    var sValue = 0.0;
                    string sampleName;
                    Guid sampleId = Guid.Empty;
                    if (workSheet.Rows[_sampleNameRow][c] != System.DBNull.Value)
                    {
                        sampleName = workSheet.Rows[_sampleNameRow][c].ToString();
                        sampleId = _samples.FirstOrDefault(s => s.SampleName == sampleName).SampleId;
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
