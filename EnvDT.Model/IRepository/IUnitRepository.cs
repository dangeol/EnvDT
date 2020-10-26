using EnvDT.Model.Entity;
using System;
using System.Linq;

namespace EnvDT.Model.IRepository
{
    public interface IUnitRepository : IGenericRepository<Unit>
    {
        public Guid GetUnitIdOfUnknown();
        public Unit GetUnitByRefValueId(Guid RefValueId);
    }
}