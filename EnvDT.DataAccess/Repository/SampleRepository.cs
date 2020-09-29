using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;

namespace EnvDT.DataAccess.Repository
{
    public class SampleRepository : GenericRepository<Sample, EnvDTDbContext>,
        ISampleRepository
    {
        public SampleRepository(EnvDTDbContext context)
            :base(context)
        {
        }
    }
}