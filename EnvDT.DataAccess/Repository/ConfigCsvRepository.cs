using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace EnvDT.DataAccess.Repository
{
    public class ConfigCsvRepository : GenericRepository<ConfigCsv, EnvDTDbContext>,
        IConfigCsvRepository
    {
        public ConfigCsvRepository(EnvDTDbContext context)
            :base(context)
        {
        }

        public ConfigCsv GetByLaboratoryId(Guid? laboratoryId)
        {
            return Context.ConfigCsvs
                .SingleOrDefault(c => c.LaboratoryId == laboratoryId);
        }

        public ConfigCsv GetByIdUpdated(Guid? configCsvId)
        {
            return Context.ConfigCsvs.AsNoTracking()
                .SingleOrDefault(c => c.ConfigCsvId == configCsvId);
        }
    }
}