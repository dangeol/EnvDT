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

                var laboratoryName = "Agrolab Bruckberg";
                Guid labReportId = CreateLabReport(reportLabIdent, laboratoryName, projectId).LabReportId;

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
                    $"{reportLabIdent} {laboratoryName}");
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

        private LabReport CreateLabReport(string reportLabIdent, string laboratoryName, Guid? projectId)
        {
            Guid laboratoryId = _unitOfWork.LabReports.GetLabIdByLabName(laboratoryName).LaboratoryId;

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
                double sValue = 0.0;
                double testVar;
                if (workSheet.Rows[r][c] != System.DBNull.Value 
                    && Double.TryParse(workSheet.Rows[r][c].ToString(), out testVar))
                {
                    sValue = (double)workSheet.Rows[r][c];
                }
                double detectionLimit = 0.0;
                if (workSheet.Rows[r][2] != System.DBNull.Value)
                {
                    detectionLimit = (double)workSheet.Rows[r][2];
                }
                var labParamName = workSheet.Rows[r][0].ToString();
                var paramLabs = _unitOfWork.SampleValues.GetParamLabsByLabParamName(labParamName);
                var unitName = workSheet.Rows[r][1].ToString();
                var unitId = _unitOfWork.SampleValues.GetUnitIdByName(unitName)?.UnitId ?? Guid.Empty;


                //TO DO: this code below is due to trouble with reading the 'µ' char. Need to change this.
                if (unitName == "µg/l")
                {
                    unitId = Guid.Parse("E78E1C38-7177-45BA-B093-637143F4C568");
                } 
                else if (unitName == "µS/cm")
                {
                    unitId = Guid.Parse("9D821E03-02E7-482D-A409-57221F92CC28");
                }

                if (paramLabs.Count() > 0)
                { 
                    foreach (var paramLab in paramLabs)
                    {
                        var sampleValue = new SampleValue();
                        sampleValue.SValue = sValue;
                        sampleValue.DetectionLimit = detectionLimit;
                        sampleValue.SampleId = sampleId;
                        sampleValue.ParameterId = paramLab.ParameterId;
                        sampleValue.UnitId = unitId;

                        _unitOfWork.SampleValues.Create(sampleValue);
                    }
                }
                r++;
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
