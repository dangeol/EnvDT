using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Event;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace EnvDT.UI.Service
{
    public class ImportLabReportService : IImportLabReportService
    {
        private IEventAggregator _eventAggregator;
        private IMessageDialogService _messageDialogService;
        private IUnitOfWork _unitOfWork;
        private IReadFileHelper _readFileHelper;
        private List<Guid> _sampleIdList = new List<Guid>();

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
            if (workSheet.Rows[2][4] != System.DBNull.Value)
            { 
                reportLabIdent = workSheet.Rows[2][4].ToString();
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
            if (workSheet.Rows[0][0] != System.DBNull.Value)
            {
                labName = workSheet.Rows[0][0].ToString();
            }
            else
            {
                DisplayReadingCellErrorMessage(nameof(labName));
                return;
            }
            var labCompany = _unitOfWork.LabReports.GetLabByLabName(labName).LabCompany;
            Guid labReportId = CreateLabReport(reportLabIdent, labName, projectId).LabReportId;

            int c = 4;
            while (c < workSheet.Columns.Count)
            {
                string sampleLabIdent;
                if (workSheet.Rows[3][c] != System.DBNull.Value)
                {
                    sampleLabIdent = workSheet.Rows[3][c].ToString();
                }
                else
                {
                    DisplayReadingCellErrorMessage(nameof(sampleLabIdent));
                    return;
                }
                string sampleName;
                if (workSheet.Rows[4][c] != System.DBNull.Value)
                {
                    sampleName = workSheet.Rows[4][c].ToString();
                }
                else
                {
                    DisplayReadingCellErrorMessage(nameof(sampleName));
                    return;
                }

                Guid sampleId = CreateSample(
                    sampleLabIdent, sampleName, labReportId
                ).SampleId;
                _sampleIdList.Add(sampleId);

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
                var result = _messageDialogService.ShowOkDialog("Import LabReport",
                    $"This LabReport has already been imported. Please chose another file or" +
                    $" delete the LabReport first.");
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
            int r = 7;
            while (r < workSheet.Rows.Count)
            {
                var labParamName = "";
                if (workSheet.Rows[r][0] != System.DBNull.Value)
                {
                    labParamName = workSheet.Rows[r][0].ToString();
                }
                else
                {
                    DisplayReadingCellErrorMessage(nameof(labParamName));
                    return;
                }
                var labParamUnitName = workSheet.Rows[r][1].ToString();
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

            //TO DO: this code below is due to trouble with reading the 'µ' char. Need to change this.
            if (unitName == "µg/l")
            {
                unitId = Guid.Parse("E78E1C38-7177-45BA-B093-637143F4C568");
            }
            else if (unitName == "µS/cm")
            {
                unitId = Guid.Parse("9D821E03-02E7-482D-A409-57221F92CC28");
            }
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
            if (workSheet.Rows[r][2] != System.DBNull.Value)
            {
                labReportParam.DetectionLimit = (double)workSheet.Rows[r][2];
            }
            labReportParam.Method = "";
            if (workSheet.Rows[r][3] != System.DBNull.Value)
            {
                labReportParam.Method = (string)workSheet.Rows[r][3];
            }

            _unitOfWork.LabReportParams.Create(labReportParam);
            CreateNewSampleValue(r, workSheet, labReportParam);
        }

        private void CreateNewSampleValue(int r, DataTable workSheet, LabReportParam labReportParam)
        {
            var c_init = 4;
            for (int i = 0; i < _sampleIdList.Count; i++)
            {
                var c = c_init + i;
                if (workSheet.Rows[r][c] != System.DBNull.Value)
                {
                    var sValue = 0.0;
                    double testVar;
                    if (Double.TryParse(workSheet.Rows[r][c].ToString(), out testVar))
                    {
                        sValue = (double)workSheet.Rows[r][c];
                    }
                    var sampleValue = new SampleValue();
                    sampleValue.SValue = sValue;
                    sampleValue.LabReportParamId = labReportParam.LabReportParamId;
                    sampleValue.SampleId = _sampleIdList[i];

                    _unitOfWork.SampleValues.Create(sampleValue);
                }
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
            _messageDialogService.ShowOkDialog("Cell value error", 
                "The value of the following key could not be read: " + variableName);
        }
    }
}
