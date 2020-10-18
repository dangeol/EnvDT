using EnvDT.Model.Entity;
using System;

namespace EnvDT.Model.IRepository
{
    public interface IValuationClassRepository : IGenericRepository<ValuationClass>
    {
        public string getValClassNameNextLevelFromLevel(int level, Guid publicationId);
    }
}