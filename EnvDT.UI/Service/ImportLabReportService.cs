using EnvDT.DataAccess;
using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using ExcelDataReader;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace EnvDT.UI.Service
{
    public class ImportLabReportService : IImportLabReportService
    {
        private Func<EnvDTDbContext> _contextCreator;
        private IProjectRepository _projectRepository;

        public ImportLabReportService(Func<EnvDTDbContext> contextCreator, IProjectRepository projectRepository)
        {
            _contextCreator = contextCreator;
            _projectRepository = projectRepository;
        }

        public void importLabReport(string file)
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
       
                var laboratoryName = "Agrolab Bruckberg";
                Guid labReportId = addLabReportToDb(reportLabIdent, laboratoryName);

                int c = 4;
                while (c < workSheet.Columns.Count)
                {
                    Guid sampleId = addSampleToDb(workSheet.Rows[3][c].ToString(), workSheet.Rows[4][c].ToString(), labReportId);
                    addSampleValuesToDb(workSheet, sampleId, c);
                    c++;
                }

                reader.Close();
            }
            else
            {
                //TO DO: Exception
            }
        }

        private Guid addLabReportToDb(string reportLabIdent, string laboratoryName)
        {
            using (var ctx = _contextCreator())
            {
                var projectId = _projectRepository.GetFirst().ProjectId;

                Guid laboratoryId = ctx.Laboratories
                    .Single(l => l.LaboratoryName == laboratoryName).LaboratoryId;

                var labReportId = Guid.NewGuid();
                ctx.Add(new LabReport
                { 
                    LabReportId = labReportId,
                    ReportLabIdent = reportLabIdent,
                    LaboratoryId = laboratoryId,
                    ProjectId = projectId
                });
                ctx.SaveChanges();
                return labReportId;
            }
        }

        private Guid addSampleToDb(string sampleLabIdent, string sampleName, Guid labReportId)
        {
            using (var ctx = _contextCreator())
            {
                var sampleId = Guid.NewGuid();
                ctx.Add(new Sample
                {
                    SampleId = sampleId,
                    SampleLabIdent = sampleLabIdent,
                    //SampleDate
                    SampleName = sampleName,
                    LabReportId = labReportId
                });
                ctx.SaveChanges();
                return sampleId;
            }
        }

        private void addSampleValuesToDb(DataTable workSheet, Guid sampleId, int c)
        {
            using (var ctx = _contextCreator())
            {
                int r = 7;
                while (r < workSheet.Rows.Count)
                {
                    double sValue = 0.0;
                    double testVar;
                    if (workSheet.Rows[r][c] != System.DBNull.Value && Double.TryParse(workSheet.Rows[r][c].ToString(), out testVar))
                    {
                        sValue = (double)workSheet.Rows[r][c];
                    }
                    double detectionLimit = 0.0;
                    if (workSheet.Rows[r][2] != System.DBNull.Value)
                    {
                        detectionLimit = (double)workSheet.Rows[r][2];
                    }
                    var paramLabs = ctx.ParameterLaboratories
                        .Where(pl => pl.LabParamName == workSheet.Rows[r][0].ToString()); 
                    var unitId = ctx.Units
                        .FirstOrDefault(u => u.UnitName == workSheet.Rows[r][1].ToString())?.UnitId ?? Guid.Empty;

                    if (workSheet.Rows[r][1].ToString() == "µg/l")
                    {
                        unitId = Guid.Parse("E78E1C38-7177-45BA-B093-637143F4C568");
                    } 
                    else if (workSheet.Rows[r][1].ToString() == "µS/cm")
                    {
                        unitId = Guid.Parse("9D821E03-02E7-482D-A409-57221F92CC28");
                    }

                    if (paramLabs.Count() > 0)
                    { 
                        foreach (var paramLab in paramLabs)
                        {
                            var sampleValueId = Guid.NewGuid();

                            ctx.Add(new SampleValue
                            {
                                SampleValueId = sampleValueId,
                                SValue = sValue,
                                DetectionLimit = detectionLimit,
                                SampleId = sampleId,
                                ParameterId = paramLab.ParameterId,
                                UnitId = unitId
                            });
                        }
                    }
                    r++;
                }
                ctx.SaveChanges();
            }
        }
    }
}
