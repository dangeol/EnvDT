using EnvDT.Model.Entity;
using System;

namespace EnvDT.Model.IRepository
{
    public interface IValuationClassRepository : IGenericRepository<ValuationClass>
    {
        public string GetValClassNameNextLevelFromLevel(int level, Guid publicationId);
    }
}