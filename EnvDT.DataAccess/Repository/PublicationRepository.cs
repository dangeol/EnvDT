using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.DataAccess.Repository
{
    public class PublicationRepository : GenericRepository<Publication, EnvDTDbContext>,
        IPublicationRepository
    {
        public PublicationRepository(EnvDTDbContext context)
            :base(context)
        {
        }

        public override Publication GetById(Guid publicationId)
        {
            return Context.Publications
                .Include(p => p.PublParams)
                .FirstOrDefault(p => p.PublicationId == publicationId);
        }
    }
}