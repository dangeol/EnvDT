using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace EnvDT.DataAccess.Repository
{
    public class ConfigXlsxRepository : GenericRepository<ConfigXlsx, EnvDTDbContext>,
        IConfigXlsxRepository
    {
        public ConfigXlsxRepository(EnvDTDbContext context)
            :base(context)
        {
        }

        public ConfigXlsx GetByLaboratoryId(Guid? laboratoryId)
        {
            return Context.ConfigXlsxs
                .SingleOrDefault(c => c.LaboratoryId == laboratoryId);
        }

        public ConfigXlsx GetByIdUpdated(Guid? configXlsxId)
        {
            return Context.ConfigXlsxs.AsNoTracking()
                .SingleOrDefault(c => c.ConfigXlsxId == configXlsxId);
        }
    }
}