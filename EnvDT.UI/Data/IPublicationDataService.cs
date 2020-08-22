using EnvDT.Model;
using System.Collections.Generic;

namespace EnvDT.UI.Data
{
    public interface IPublicationDataService
    {
        IEnumerable<Publication> GetAll();
    }
}