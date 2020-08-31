using EnvDT.DataAccess;
using EnvDT.Model.Guidelines;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.UI.Data.Lookups
{
    public class LookupDataService : IPublicationLookupDataService
    {
        private Func<EnvDTDbContext> _contextCreator;

        public LookupDataService(Func<EnvDTDbContext> contextCreator)
        {
            _contextCreator = contextCreator;
        }
        public IEnumerable<LookupItem> GetPublicationLookup()
        {
            using (var ctx = _contextCreator())
            {
                return ctx.Publications.AsNoTracking()
                    .Select(p =>
                    new LookupItem
                    {
                        LookupItemId = p.PublicationId,
                        DisplayMember = p.Abbreviation + " " + p.Year
                    })
                    .ToList();
            }
        }
    }
}
