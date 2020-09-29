﻿using EnvDT.Model.Entity;
using EnvDT.Model.IDataService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.DataAccess.DataService
{
    public class LookupDataService : IProjectDataService, ILabReportDataService
    {
        private Func<EnvDTDbContext> _contextCreator;
        public LookupDataService(Func<EnvDTDbContext> contextCreator)
        {
            _contextCreator = contextCreator;
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
                        DisplayMember = $"{l.ReportLabIdent} {l.Laboratory.LaboratoryName}"
                    });
            }
        }
    }
}
