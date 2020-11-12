using EnvDT.Model.Entity;
using System;

namespace EnvDT.Model.IRepository
{
    public interface IUnitNameVariantRepository : IGenericRepository<UnitNameVariant>
    {
        public UnitNameVariant GetUnitNameVariantByLabParamUnitName(string labParamUnitName);
    }
}