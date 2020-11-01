using EnvDT.Model.Entity;
using System.Collections.Generic;

namespace EnvDT.Model.IRepository
{
    public interface IParamNameVariantRepository : IGenericRepository<ParamNameVariant>
    {
        public IEnumerable<ParamNameVariant> GetParamNameVariantsByLabParamName(string labParamName);
    }
}