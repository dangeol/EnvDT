using EnvDT.DataAccess;
using EnvDT.Model;
using System;
using System.Linq;

namespace EnvDT.UI.Data.Repositories
{
    public class PublicationRepository : IPublicationRepository
    {
        private EnvDTDbContext _context;

        public PublicationRepository(EnvDTDbContext context)
        {
            _context = context;
        }
        public Publication GetById(Guid publicationId)
        {
                return _context.Publications.Single(p => p.PublicationId == publicationId);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
