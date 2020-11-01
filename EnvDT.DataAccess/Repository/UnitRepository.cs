using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace EnvDT.DataAccess.Repository
{
    public class UnitRepository : GenericRepository<Unit, EnvDTDbContext>,
        IUnitRepository
    {
        public UnitRepository(EnvDTDbContext context)
            :base(context)
        {
        }

        public Guid GetUnitIdOfUnknown()
        {
            return Context.Units.AsNoTracking()
                .Single(u => u.UnitName == "[unknown]").UnitId;
        }

        public Unit GetUnitIdByName(string unitName)
        {
            return Context.Units.AsNoTracking()
                .FirstOrDefault(u => u.UnitName == unitName);
        }
    }
}