using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Event;
using ExcelDataReader;
using Prism.Events;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace EnvDT.UI.Service
{
    public class ImportLabReportService : IImportLabReportService
    {
        private IEventAggregator _eventAggregator;
        private IMessageDialogService _messageDialogService;
        private IUnitOfWork _unitOfWork;

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
                    CreateSampleValues(workSheet, sampleId, c);
                    c++;
                }

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

        private void CreateSampleValues(DataTable workSheet, Guid sampleId, int c)
        {
            int r = 7;
            while (r < workSheet.Rows.Count)
            {
                if (workSheet.Rows[r][c] != System.DBNull.Value)
                {
                    var sampleValTempObj = new SampleValue();
                    sampleValTempObj.SampleId = sampleId;
                    sampleValTempObj.SValue = 0.0;
                    double testVar;
                    if (Double.TryParse(workSheet.Rows[r][c].ToString(), out testVar))
                    {
                        sampleValTempObj.SValue = (double)workSheet.Rows[r][c];
                    }
                    sampleValTempObj.DetectionLimit = 0.0;
                    if (workSheet.Rows[r][2] != System.DBNull.Value)
                    {
                        sampleValTempObj.DetectionLimit = (double)workSheet.Rows[r][2];
                    }
                    sampleValTempObj.Method = "";
                    if (workSheet.Rows[r][3] != System.DBNull.Value)
                    {
                        sampleValTempObj.Method = (string)workSheet.Rows[r][3];
                    }
                    var labParamName = workSheet.Rows[r][0].ToString();
                    var paramLabs = _unitOfWork.SampleValues.GetParamNameVariantsByLabParamName(labParamName);
                    var unitName = workSheet.Rows[r][1].ToString();
                    sampleValTempObj.UnitId = _unitOfWork.SampleValues.GetUnitIdByName(unitName)?.UnitId 
                        ?? _unitOfWork.Units.GetUnitIdOfUnknown();

                    //TO DO: this code below is due to trouble with reading the 'µ' char. Need to change this.
                    if (unitName == "µg/l")
                    {
                        sampleValTempObj.UnitId = Guid.Parse("E78E1C38-7177-45BA-B093-637143F4C568");
                    } 
                    else if (unitName == "µS/cm")
                    {
                        sampleValTempObj.UnitId = Guid.Parse("9D821E03-02E7-482D-A409-57221F92CC28");
                    }

                    if (paramLabs.Count() > 0)
                    { 
                        foreach (var paramLab in paramLabs)
                        {
                            CreateNewSampleValue(sampleValTempObj, paramLab.ParameterId);
                        }
                    } 
                    else
                    {
                        var unknownParameterId = _unitOfWork.Parameters.GetParameterIdOfUnknown();
                        CreateNewSampleValue(sampleValTempObj, unknownParameterId);
                    }
                }
                r++;
            }
        }

        private void CreateNewSampleValue(SampleValue sampleValTempObj, Guid parameterId)
        {
            var sampleValue = new SampleValue();
            sampleValue.SValue = sampleValTempObj.SValue;
            sampleValue.DetectionLimit = sampleValTempObj.DetectionLimit;
            sampleValue.Method = sampleValTempObj.Method;
            sampleValue.SampleId = sampleValTempObj.SampleId;
            sampleValue.ParameterId = parameterId;
            sampleValue.UnitId = sampleValTempObj.UnitId;

            _unitOfWork.SampleValues.Create(sampleValue);
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
