using EnvDT.DataAccess;
using EnvDT.Model;
using ExcelDataReader;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace EnvDT.UI.Data.Services
{
    public class ImportLabReportService : IImportLabReportService
    {
        private Func<EnvDTDbContext> _contextCreator;
        public ImportLabReportService(Func<EnvDTDbContext> contextCreator)
        {
            _contextCreator = contextCreator;
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

                //DataSet workSheets = reader.AsDataSet();
                DataTable workSheet = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = false
                    }
                }).Tables["Datenblatt"];
                //System.Diagnostics.Debug.WriteLine(workSheet.TableName);
                //foreach (var row in workSheet.Rows)
                //System.Diagnostics.Debug.WriteLine(((DataRow)row)[0]);
                var reportLabIdent = workSheet.Rows[2][4].ToString();
                Guid projectId = Guid.NewGuid();
                var laboratoryName = "Agrolab Bruckberg";
                addLabReportToDb((string)reportLabIdent, projectId, (string)laboratoryName);
                reader.Close();
            }
            else
            {
                //TO DO: Exception
            }
        }

        private void addLabReportToDb(string reportLabIdent, Guid projectId, string laboratoryName)
        {
            using (var ctx = _contextCreator())
            {
                ctx.Add(new Project
                {
                    ProjectId = projectId,
                    ProjectName = "Sample-Project",
                    ProjectAddress = "Sample-Address"
                });

                Guid laboratoryId = ctx.Laboratories
                    .Single(l => l.LaboratoryName == laboratoryName).LaboratoryId;

                Guid labReportId = Guid.NewGuid();
                ctx.Add(new LabReport
                { 
                    LabReportId = labReportId,
                    ReportLabIdent = reportLabIdent,
                    LaboratoryId = laboratoryId,
                    ProjectId = projectId
                });
                ctx.SaveChanges();
            }
        }
    }
}
