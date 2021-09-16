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
                        DisplayMember = l.MedSubTypeNameDe
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

        public IEnumerable<LookupItem> GetAllLaboratoriesLookup()
        {
            using (var ctx = _contextCreator())
            {
                return ctx.Set<Laboratory>().AsNoTracking().ToList()
                    .Select(l => new LookupItem
                    {
                        LookupItemId = l.LaboratoryId,
                        DisplayMember = $"{l.LabCompany} ({l.LabName})"
                    });
            }
        }

        public IEnumerable<LookupItem> GetAllCountriesLookup()
        {
            using (var ctx = _contextCreator())
            {
                return ctx.Set<Country>().AsNoTracking().ToList()
                    .Select(l => new LookupItem
                    {
                        LookupItemId = l.CountryId,
                        DisplayMember = $"{l.CountryNameDe}"
                    });
            }
        }

        public IEnumerable<LookupItem> GetAllRegionsLookupByCountryId(Guid countryId)
        {
            using (var ctx = _contextCreator())
            {
                return ctx.Set<Region>().AsNoTracking().ToList()
                    .Where(r => r.CountryId == countryId)
                    .Select(l => new LookupItem
                    {
                        LookupItemId = l.RegionId,
                        DisplayMember = $"{l.RegionNameDe}"
                    })
                    .OrderBy(l => l.DisplayMember);
            }
        }

        public IEnumerable<LookupItem> GetAllConfigXlsxs()
        {
            using (var ctx = _contextCreator())
            {
                return ctx.Set<ConfigXlsx>().AsNoTracking().ToList()
                    .Select(l => new LookupItem
                    {
                        LookupItemId = l.ConfigXlsxId,
                        DisplayMember = l.WorksheetName
                    });
            }
        }

        public IEnumerable<LookupItem> GetAllWasteCodeEWCsLookup()
        {
            using (var ctx = _contextCreator())
            {
                return ctx.Set<WasteCodeEWC>().AsNoTracking().ToList()
                    .Select(l => new LookupItem
                    {
                        LookupItemId = l.WasteCodeEWCId,
                        DisplayMember = $"{l.WasteCodeNumber}: {l.WasteCodeDescrDeAVV}"
                    })
                    .OrderBy(l => l.DisplayMember);
            }
        }
    }
}
