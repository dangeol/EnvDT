using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.DataAccess.Repository
{
    public class SampleValueRepository : GenericRepository<SampleValue, EnvDTDbContext>,
        ISampleValueRepository
    {
        public SampleValueRepository(EnvDTDbContext context)
            :base(context)
        {
        }

        public IEnumerable<ParameterLaboratory> GetParamLabsByLabParamName(string labParamName)
        {
            return Context.Set<ParameterLaboratory>().AsNoTracking().ToList()
                .Where(pl => pl.LabParamName == labParamName);
        }

        public Unit GetUnitIdByName(string unitName)
        {
            return Context.Units.AsNoTracking()
                .FirstOrDefault(u => u.UnitName == unitName);
        }
    }
}