using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.DataAccess.Repository
{
    public class FootnoteParamRepository : GenericRepository<FootnoteParam, EnvDTDbContext>,
        IFootnoteParamRepository
    {
        public FootnoteParamRepository(EnvDTDbContext context)
            :base(context)
        {
        }

        public override FootnoteParam GetById(Guid? footnoteParamId)
        {
            return Context.FootnoteParams.AsNoTracking()
                .FirstOrDefault(pp => pp.FootnoteParamId == footnoteParamId);
        }

        public IEnumerable<FootnoteParam> GetFootnoteParamsByFootnoteId(Guid? footnoteId)
        {
            return Context.Set<FootnoteParam>().AsNoTracking().ToList()
                .Where(fp => fp.FootnoteId == footnoteId)
                .OrderBy(fp => fp.OrderNo);
        }
    }
}