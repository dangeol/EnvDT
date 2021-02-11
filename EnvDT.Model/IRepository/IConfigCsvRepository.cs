using EnvDT.Model.Entity;
using System;

namespace EnvDT.Model.IRepository
{
    public interface IConfigCsvRepository : IGenericRepository<ConfigCsv>
    {
        public ConfigCsv GetByLaboratoryId(Guid? laboratoryId);
        public ConfigCsv GetByIdUpdated(Guid? configCsvId);
    }
}