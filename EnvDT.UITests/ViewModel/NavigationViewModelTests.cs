using EnvDT.Model;
using EnvDT.UI.Data.Repository;
using EnvDT.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EnvDT.UITests.ViewModel
{
    public class NavigationViewModelTests
    {
        [Fact]
        public void ShouldLoadProjects()
        {
            var viewModel = new NavigationViewModel(new ProjectRepositoryMock());

            viewModel.LoadProjects();

            Assert.Equal(2, viewModel.Projects.Count);

            var project = viewModel.Projects.SingleOrDefault(
                p => p.LookupItemId == Guid.Parse("67455421-0498-46af-9241-7287539fcade"));
            Assert.NotNull(project);
            Assert.Equal("111.111 MockProject1", project.DisplayMember);

            project = viewModel.Projects.SingleOrDefault(
                p => p.LookupItemId == Guid.Parse("13ce3bee-d343-4851-81a8-ce916f6756db"));
            Assert.NotNull(project);
            Assert.Equal("222.222 MockProject2", project.DisplayMember);
        }

        [Fact]
        public void ShouldLoadProjectsOnlyOnce()
        {
            var viewModel = new NavigationViewModel(new ProjectRepositoryMock());

            viewModel.LoadProjects();
            viewModel.LoadProjects();

            Assert.Equal(2, viewModel.Projects.Count);
        }
    }

    public class ProjectRepositoryMock : IProjectRepository
    {
        public IEnumerable<LookupItem> GetAllProjects()
        {
            yield return new LookupItem
            {
                LookupItemId = new Guid("67455421-0498-46af-9241-7287539fcade"),
                DisplayMember = "111.111 MockProject1"
            };
            yield return new LookupItem
            {
                LookupItemId = new Guid("13ce3bee-d343-4851-81a8-ce916f6756db"),
                DisplayMember = "222.222 MockProject2"
            };
        }

        public Project GetFirstProject()
        {
            throw new NotImplementedException();
        }

        public Project GetProjectById(Guid projectId)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
