using System;
using System.Collections.Generic;

namespace EnvDT.Model.IRepository
{
    public interface IProjectRepository
    {
        Project GetProjectById(Guid projectId);
        Project GetFirstProject();
        IEnumerable<LookupItem> GetAllProjects();
        void SaveProject(Project project);
        void DeleteProject(Guid projectId);
        void Save();

    }
}