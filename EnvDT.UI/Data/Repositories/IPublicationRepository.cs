using EnvDT.Model;
using System;

namespace EnvDT.UI.Data.Repositories
{
    public interface IPublicationRepository
    {
        Publication GetById(Guid publicationId);
        void Save();
    }
}