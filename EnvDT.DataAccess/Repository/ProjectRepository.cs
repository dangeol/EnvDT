using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
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

        public override Project GetById(Guid projectId)
        {
            return Context.Projects
                .Include(p => p.LabReports)
                .FirstOrDefault(p => p.ProjectId == projectId);
        }

        public override Project GetFirst()
        {
            return Context.Projects.First();
        }

        public void DeleteLabReport(LabReport model)
        {
            Context.LabReports.Remove(model);
        }

        public void Dispose()
        {
        }
    }
}