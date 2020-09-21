using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.DataAccess.Repository
{
    public class ProjectRepository : IProjectRepository
    {
        private EnvDTDbContext _context;

        public ProjectRepository(EnvDTDbContext context)
        {
            _context = context;
        }

        public IEnumerable<LookupItem> GetAllProjects()
        {
            return _context.Projects.ToList()
                .Select(p => new LookupItem
                {
                    LookupItemId = p.ProjectId,
                    DisplayMember = $"{p.ProjectNumber} {p.ProjectName}"
                });
        }

        public Project GetProjectById(Guid projectId)
        {
            return _context.Projects.Single(p => p.ProjectId == projectId);
        }

        public Project GetFirstProject()
        {
            return _context.Projects.First();
        }

        public void CreateProject(Project project)
        {
            _context.Projects.Add(project);
        }

        public void DeleteProject(Guid projectId)
        {
            var existing = _context.Projects.FirstOrDefault(p => p.ProjectId == projectId);
            if (existing != null)
            { 
                _context.Projects.Remove(existing);
            }   
        }

        public void Dispose()
        {
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}