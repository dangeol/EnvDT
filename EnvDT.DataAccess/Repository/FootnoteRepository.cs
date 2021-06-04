using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;

namespace EnvDT.DataAccess.Repository
{
    public class FootnoteRepository : GenericRepository<Footnote, EnvDTDbContext>,
        IFootnoteRepository
    {
        public FootnoteRepository(EnvDTDbContext context)
            :base(context)
        {
        }
    }
}