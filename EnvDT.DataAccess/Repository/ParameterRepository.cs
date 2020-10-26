using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace EnvDT.DataAccess.Repository
{
    public class ParameterRepository : GenericRepository<Parameter, EnvDTDbContext>,
        IParameterRepository
    {
        public ParameterRepository(EnvDTDbContext context)
            :base(context)
        {
        }

        public Guid GetParameterIdOfUnknown()
        {
            return Context.Parameters.AsNoTracking()
                .Single(p => p.ParamNameEn == "[unknown]").ParameterId;
        }

        public Parameter GetParameterByRefValueId(Guid RefValueId)
        {
            return
            (
                from rv in Context.RefValues
                    .Where(rv => rv.RefValueId == RefValueId)
                join pp in Context.PublParams on rv.PublParamId equals pp.PublParamId
                select Context.Parameters.AsNoTracking()
                        .Single(p => p.ParameterId == pp.ParameterId)
            ).First();
        }
    }
}