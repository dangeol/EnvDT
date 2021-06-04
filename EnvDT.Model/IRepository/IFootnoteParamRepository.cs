using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;

namespace EnvDT.Model.IRepository
{
    public interface IFootnoteParamRepository : IGenericRepository<FootnoteParam>
    {
        public IEnumerable<FootnoteParam> GetFootnoteParamsByFootnoteId(Guid? footnoteId);
    }
}