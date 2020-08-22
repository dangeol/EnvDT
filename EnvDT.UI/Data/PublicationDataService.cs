using EnvDT.DataAccess;
using EnvDT.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.UI.Data
{
    public class PublicationDataService : IPublicationDataService
    {
        public IEnumerable<Publication> GetAll()
        {
            using (var ctx = new EnvDTDbContext())
            {
                return ctx.Publications.AsNoTracking().ToList();
            }
        }
    }
}
