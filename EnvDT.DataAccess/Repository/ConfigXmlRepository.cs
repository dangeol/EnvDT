using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using System;
using System.Linq;

namespace EnvDT.DataAccess.Repository
{
    public class ConfigXmlRepository : GenericRepository<ConfigXml, EnvDTDbContext>,
        IConfigXmlRepository
    {
        public ConfigXmlRepository(EnvDTDbContext context)
            :base(context)
        {
        }

        public ConfigXml GetByLaboratoryId(Guid? laboratoryId)
        {
            return Context.ConfigXmls
                .SingleOrDefault(c => c.LaboratoryId == laboratoryId);
        }
    }
}