using EnvDT.Model.Entity;
using System;

namespace EnvDT.Model.IRepository
{
    public interface IConfigXlsxRepository : IGenericRepository<ConfigXlsx>
    {
        public ConfigXlsx GetByLaboratoryId(Guid? laboratoryId);
        public ConfigXlsx GetByIdUpdated(Guid? configXlsxId);
    }
}