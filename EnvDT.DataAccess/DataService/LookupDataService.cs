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

                var labReportParams = ctx.Set<LabReportParam>().AsNoTracking().ToList();
                var nullLabReportParam = new LabReportParam();
                nullLabReportParam.ParameterId = unknownParamNameId;
                nullLabReportParam.LabReportId = labReportId;
                nullLabReportParam.LabReportParamName = "[N/A]";
                labReportParams.Add(nullLabReportParam);

                return labReportParams
                    .Where(lp => lp.ParameterId == unknownParamNameId && lp.LabReportId == labReportId)
                    .OrderBy(lp => lp.LabReportParamName)
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

                var labReportUnits = ctx.Set<LabReportParam>().AsNoTracking().ToList();
                var nullLabReportUnit = new LabReportParam();
                nullLabReportUnit.UnitId = unknownUnitNameId;
                nullLabReportUnit.LabReportId = labReportId;
                nullLabReportUnit.LabReportUnitName = "[N/A]";
                labReportUnits.Add(nullLabReportUnit);

                return labReportUnits
                    .Where(lp => lp.UnitId == unknownUnitNameId && lp.LabReportId == labReportId)
                    .OrderBy(lp => lp.LabReportUnitName)
                    .Select(lp => new LookupItem
                    {
                        LookupItemId = lp.LabReportParamId,
                        DisplayMember = lp.LabReportUnitName
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

        public IEnumerable<LookupItem> GetAllMediumSubTypesLookup()
        {
            using (var ctx = _contextCreator())
            {
                return ctx.Set<MediumSubType>().AsNoTracking().ToList()
                    .Select(l => new LookupItem
                    {
                        LookupItemId = l.MedSubTypeId,
                        DisplayMember = l.MedSubTypeNameEn
                    });
            }
        }

        public IEnumerable<LookupItem> GetAllConditionsLookup()
        {
            using (var ctx = _contextCreator())
            {
                return ctx.Set<Condition>().AsNoTracking().ToList()
                    .Select(l => new LookupItem
                    {
                        LookupItemId = l.ConditionId,
                        DisplayMember = l.ConditionName
                    });
            }
        }
    }
}
