using EnvDT.DataAccess;
using EnvDT.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.UI.Data
{
    public class PublicationDataService : IPublicationDataService
    {
        private Func<EnvDTDbContext> _contextCreator;

        public PublicationDataService(Func<EnvDTDbContext> contextCreator)
        {
            _contextCreator = contextCreator;
        }
        public Publication GetById(Guid publicationId)
        {
            using (var ctx = _contextCreator())
            {
                return ctx.Publications.AsNoTracking().Single(p => p.PublicationId == publicationId);
            }
        }

        public void Save(Publication publication)
        {
            using (var ctx = _contextCreator())
            {
                ctx.Publications.Attach(publication);
                ctx.Entry(publication).State = EntityState.Modified;
                ctx.SaveChanges();
            }
        }
    }
}
