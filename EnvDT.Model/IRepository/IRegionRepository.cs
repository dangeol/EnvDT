using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;

namespace EnvDT.Model.IRepository
{
    public interface IRegionRepository : IGenericRepository<Region>
    {
        public Region GetRegionByLabreportId(Guid labreportId);
        public IEnumerable<Region> GetRegionsByFootnoteId(Guid footnoteId);
    }
}