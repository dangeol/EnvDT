using EnvDT.Model.Guidelines;
using System.Collections.Generic;

namespace EnvDT.UI.Data.Lookups
{
    public interface IPublicationLookupDataService
    {
        IEnumerable<LookupItem> GetPublicationLookup();
    }
}