using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Event;
using ExcelDataReader;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Documents;

namespace EnvDT.UI.Service
{
    public class ImportLabReportService : IImportLabReportService
    {
        private IEventAggregator _eventAggregator;
        private IMessageDialogService _messageDialogService;
        private IUnitOfWork _unitOfWork;
        private List<Guid> _sampleIdList = new List<Guid>();

        public ImportLabReportService(IEventAggregator eventAggregator, IMessageDialogService messageDialogService,
            IUnitOfWork unitOfWork)
        {
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;
            _unitOfWork = unitOfWork;
        }

        public void ImportLabReport(string file, Guid? projectId)
        {
            if (file != null && file.Length > 0)
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                FileStream stream = File.OpenRead(file);
                IExcelDataReader reader = null;

                if (file.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (file.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                else
                {
                    throw new NotSupportedException("Wrong file extension");
                }

                DataTable workSheet = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = false
                    }
                }).Tables["Datenblatt"];

                var reportLabIdent = workSheet.Rows[2][4].ToString();
                if (IsLabReportAlreadyPresent(reportLabIdent))
                {
                    return;
                }

                var labName = "Agrolab Bruckberg";
                var labCompany = _unitOfWork.LabReports.GetLabByLabName(labName).LabCompany;
                Guid labReportId = CreateLabReport(reportLabIdent, labName, projectId).LabReportId;

                int c = 4;
                while (c < workSheet.Columns.Count)
                {
                    Guid sampleId = CreateSample(
                        workSheet.Rows[3][c].ToString(), 
                        workSheet.Rows[4][c].ToString(), 
                        labReportId
                    ).SampleId;
                    _sampleIdList.Add(sampleId);
                    c++;
                }
                CreateLabReportParams(workSheet, labReportId);

                reader.Close();

                _unitOfWork.Save();

                RaiseLabReportImportedEvent(labReportId,
                    $"{reportLabIdent} {labCompany}");
            }
            else
            {
                //TO DO: Exception
            }
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
                var labParamName = workSheet.Rows[r][0].ToString();
                var paramNameVariants = _unitOfWork.ParamNameVariants.GetParamNameVariantsByLabParamName(labParamName);

                if (paramNameVariants.Count() > 0)
                {
                    foreach (var paramNameVariant in paramNameVariants)
                    {
                        var parameterId = paramNameVariant.ParameterId;
                        CreateNewLabParam(workSheet, parameterId, labReportId, r);   
                    }
                }
                else
                {
                    var unknownParameterId = _unitOfWork.Parameters.GetParameterIdOfUnknown();
                    CreateNewLabParam(workSheet, unknownParameterId, labReportId, r);
                }
                r++;
            }
        }

        private void CreateNewLabParam(DataTable workSheet, Guid parameterId, Guid labReportId, int r)
        {
            var labReportParam = new LabReportParam();
            labReportParam.ParameterId = parameterId;
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
            var unitName = workSheet.Rows[r][1].ToString();
            labReportParam.UnitId = _unitOfWork.Units.GetUnitIdByName(unitName)?.UnitId
                ?? _unitOfWork.Units.GetUnitIdOfUnknown();

            //TO DO: this code below is due to trouble with reading the 'µ' char. Need to change this.
            if (unitName == "µg/l")
            {
                labReportParam.UnitId = Guid.Parse("E78E1C38-7177-45BA-B093-637143F4C568");
            }
            else if (unitName == "µS/cm")
            {
                labReportParam.UnitId = Guid.Parse("9D821E03-02E7-482D-A409-57221F92CC28");
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
    }
}
