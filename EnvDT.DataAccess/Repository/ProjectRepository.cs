using EnvDT.Model;
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
        public Project GetProjectById(Guid projectId)
        {
            return _context.Projects.Single(p => p.ProjectId == projectId);
        }

        public Project GetFirstProject()
        {
            return _context.Projects.First();
        }

        public void SaveProject(Project project)
        {
            if (project.ProjectId == Guid.Empty || project.ProjectId == null)
            {
                CreateProject(project);
            }
            else
            {
                UpdateProject(project);
            }
        }

        public void DeleteProject(Guid projectId)
        {
            var existing = _context.Projects.FirstOrDefault(p => p.ProjectId == projectId);
            if (existing != null)
            { 
                _context.Projects.Remove(existing);
                _context.SaveChanges();
            }   
        }

        private void UpdateProject(Project project)
        {
            var existing = _context.Projects.Single(p => p.ProjectId == project.ProjectId);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(project);
                _context.SaveChanges();
            }
        }

        private void CreateProject(Project project)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();
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

        public void Dispose()
        {
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}