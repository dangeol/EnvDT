﻿using EnvDT.Model;
using System;
using System.Collections.Generic;

namespace EnvDT.UI.Data.Repository
{
    public interface IProjectRepository
    {
        Project GetProjectById(Guid projectId);
        Project GetFirstProject();
        IEnumerable<LookupItem> GetAllProjects();
        void Save();
    }
}