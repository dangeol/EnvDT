using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.DataAccess.Repository
{
    public class ProjectRepository : GenericRepository<Project,EnvDTDbContext>,
        IProjectRepository
    {
        public ProjectRepository(EnvDTDbContext context)
            :base(context)
        {
        }

        public IEnumerable<LookupItem> GetAllProjects()
        {
            return Context.Projects.ToList()
                .Select(p => new LookupItem
                {
                    LookupItemId = p.ProjectId,
                    DisplayMember = $"{p.ProjectNumber} {p.ProjectName}"
                });
        }

        public override Project GetById(Guid projectId)
        {
            return Context.Projects.FirstOrDefault(p => p.ProjectId == projectId);
        }

        public override Project GetFirst()
        {
            return Context.Projects.First();
        }

        public void Dispose()
        {
        }
    }
}