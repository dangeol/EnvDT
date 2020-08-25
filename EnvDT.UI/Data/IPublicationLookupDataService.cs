using EnvDT.Model.Model;
using System.Collections.Generic;

namespace EnvDT.UI.Data
{
    public interface IPublicationLookupDataService
    {
        IEnumerable<LookupItem> GetPublicationLookup();
    }
}