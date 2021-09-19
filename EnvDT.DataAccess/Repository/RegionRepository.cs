using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.DataAccess.Repository
{
    public class RegionRepository : GenericRepository<Region, EnvDTDbContext>,
        IRegionRepository
    {
        public RegionRepository(EnvDTDbContext context)
            :base(context)
        {
        }

        public Region GetRegionByLabreportId(Guid labreportId)
        {
            return
            (
                from p in Context.Projects
                join lr in Context.LabReports on labreportId equals lr.LabReportId
                select p.Region
            )
            .SingleOrDefault();
        }

        public IEnumerable<Region> GetRegionsByFootnoteId(Guid footnoteId)
        {
            return
            (
                from r in Context.Regions
                join fr in Context.FootnoteRegions on r.RegionId equals fr.RegionId
                where fr.FootnoteId == footnoteId
                select r
            )
            .ToList();
        }
    }
}