using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;

namespace EnvDT.Model.IRepository
{
    public interface IRefValueRepository : IGenericRepository<RefValue>
    {
        public IEnumerable<RefValue> GetRefValuesByPublicationId(Guid publicationId);
    }
}