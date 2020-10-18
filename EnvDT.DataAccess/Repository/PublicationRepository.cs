using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;

namespace EnvDT.DataAccess.Repository
{
    public class PublicationRepository : GenericRepository<Publication, EnvDTDbContext>,
        IPublicationRepository
    {
        public PublicationRepository(EnvDTDbContext context)
            :base(context)
        {
        }
    }
}