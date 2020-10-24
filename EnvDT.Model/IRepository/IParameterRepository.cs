using EnvDT.Model.Entity;
using System;

namespace EnvDT.Model.IRepository
{
    public interface IParameterRepository : IGenericRepository<Parameter>
    {
        public Guid GetParameterIdOfUnknown();
    }
}