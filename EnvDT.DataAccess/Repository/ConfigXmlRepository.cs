using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;

namespace EnvDT.DataAccess.Repository
{
    public class ConfigXmlRepository : GenericRepository<ConfigXml, EnvDTDbContext>,
        IConfigXmlRepository
    {
        public ConfigXmlRepository(EnvDTDbContext context)
            :base(context)
        {
        }

    }
}