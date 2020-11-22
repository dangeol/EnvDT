using EnvDT.Model.Entity;
using EnvDT.Model.IDataService;
using EnvDT.Model.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.DataAccess.DataService
{
    public class LookupDataService : ILookupDataService
    {
        private Func<EnvDTDbContext> _contextCreator;
        private ILabReportRepository _labReportRepository;

        public LookupDataService(Func<EnvDTDbContext> contextCreator, ILabReportRepository labReportRepository)
        {
            _contextCreator = contextCreator;
            _labReportRepository = labReportRepository;
        }
        public IEnumerable<LookupItem> GetAllProjectsLookup()
        {
            using (var ctx = _contextCreator())
            {
                return ctx.Set<Project>().AsNoTracking().ToList()
                    .Select(p => new LookupItem
                    {
                        LookupItemId = p.ProjectId,
                        DisplayMember = $"{p.ProjectNumber} {p.ProjectName}"
                    });
            }
        }

        public IEnumerable<LookupItem> GetAllLabReportsLookupByProjectId(Guid? projectId)
        {
            using (var ctx = _contextCreator())
            {
                return ctx.Set<LabReport>().AsNoTracking().ToList()
                    .Where(l => l.ProjectId == projectId)
                    .Select(l => new LookupItem
                    {
                        LookupItemId = l.LabReportId,
                        DisplayMember = $"{l.ReportLabIdent} " +
                        $"{_labReportRepository.GetLabByLabId(l.LaboratoryId).LabCompany}"
                    });
            }
        }

        public IEnumerable<LookupItem> GetLabReportUnknownParamNamesLookupByLabReportId(Guid labReportId)
        {
            using (var ctx = _contextCreator())
            {
                var unknownParamNameId = ctx.Parameters.AsNoTracking()
                    .Single(p => p.ParamNameEn == "[unknown]").ParameterId;

                return ctx.Set<LabReportParam>().AsNoTracking().ToList()
                    .Where(lp => lp.ParameterId == unknownParamNameId && lp.LabReportId == labReportId)
                    .Select(lp => new LookupItem
                    {
                        LookupItemId = lp.LabReportParamId,
                        DisplayMember = lp.LabReportParamName
                    });
            }
        }

        public IEnumerable<LookupItem> GetLabReportUnknownUnitNamesLookupByLabReportId(Guid labReportId)
        {
            using (var ctx = _contextCreator())
            {
                var unknownUnitNameId = ctx.Units.AsNoTracking()
                    .Single(u => u.UnitName == "[unknown]").UnitId;

                return ctx.Set<LabReportParam>().AsNoTracking().ToList()
                    .Where(u => u.UnitId == unknownUnitNameId && u.LabReportId == labReportId)
                    .Select(u => new LookupItem
                    {
                        LookupItemId = u.LabReportParamId,
                        DisplayMember = u.LabReportUnitName
                    });
            }
        }

        public IEnumerable<LookupItem> GetAllLanguagesLookup()
        {
            using (var ctx = _contextCreator())
            {
                return ctx.Set<Language>().AsNoTracking().ToList()
                    .Select(l => new LookupItem
                    {
                        LookupItemId = l.LanguageId,
                        DisplayMember = l.LangName
                    });
            }
        }
    }
}
