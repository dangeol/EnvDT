using EnvDT.Model.Entity;
using System;

namespace EnvDT.Model.IRepository
{
    public interface IPublParamRepository : IGenericRepository<PublParam>
    {
        public PublParam GetByPublIdParameterNameDeAndUnitName(Guid publicationId, string paramNameDe, string unitName);
    }
}