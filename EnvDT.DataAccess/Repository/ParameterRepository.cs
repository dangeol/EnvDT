using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;

namespace EnvDT.DataAccess.Repository
{
    public class ParameterRepository : GenericRepository<Parameter, EnvDTDbContext>,
        IParameterRepository
    {
        public ParameterRepository(EnvDTDbContext context)
            :base(context)
        {
        }
    }
}