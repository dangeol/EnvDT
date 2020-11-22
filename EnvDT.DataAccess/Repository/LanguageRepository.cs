using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;

namespace EnvDT.DataAccess.Repository
{
    public class LanguageRepository : GenericRepository<Language, EnvDTDbContext>,
        ILanguageRepository
    {
        public LanguageRepository(EnvDTDbContext context)
            :base(context)
        {
        }
    }
}