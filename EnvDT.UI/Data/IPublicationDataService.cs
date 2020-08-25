using EnvDT.Model;
using System;

namespace EnvDT.UI.Data
{
    public interface IPublicationDataService
    {
        Publication GetById(Guid publicationId);
        void Save(Publication publication);
    }
}