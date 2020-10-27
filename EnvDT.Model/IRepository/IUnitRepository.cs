using EnvDT.Model.Entity;
using System;

namespace EnvDT.Model.IRepository
{
    public interface IUnitRepository : IGenericRepository<Unit>
    {
        public Guid GetUnitIdOfUnknown();
    }
}