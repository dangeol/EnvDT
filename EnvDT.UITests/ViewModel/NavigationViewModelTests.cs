﻿using EnvDT.Model;
using EnvDT.UI.Data.Repository;
using EnvDT.UI.ViewModel;
using Moq;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EnvDT.UITests.ViewModel
{
    public class NavigationViewModelTests
    {
        private NavigationViewModel _viewModel;

        public NavigationViewModelTests()
        {
            var eventAggregatorMock = new Mock<IEventAggregator>();
            var projectRepositoryMock = new Mock<IProjectRepository>();
            projectRepositoryMock.Setup(pr => pr.GetAllProjects())
                .Returns(new List<LookupItem>
                {
                    new LookupItem
                    {
                        LookupItemId = new Guid("67455421-0498-46af-9241-7287539fcade"),
                        DisplayMember = "111.111 MockProject1"
                    },
                    new LookupItem
                    {
                        LookupItemId = new Guid("13ce3bee-d343-4851-81a8-ce916f6756db"),
                        DisplayMember = "222.222 MockProject2"
                    }

                });
            _viewModel = new NavigationViewModel(
                projectRepositoryMock.Object,
                eventAggregatorMock.Object);

        }

        [Fact]
        public void ShouldLoadProjects()
        {
            _viewModel.LoadProjects();

            Assert.Equal(2, _viewModel.Projects.Count);

            var project = _viewModel.Projects.SingleOrDefault(
                p => p.LookupItemId == Guid.Parse("67455421-0498-46af-9241-7287539fcade"));
            Assert.NotNull(project);
            Assert.Equal("111.111 MockProject1", project.DisplayMember);

            project = _viewModel.Projects.SingleOrDefault(
                p => p.LookupItemId == Guid.Parse("13ce3bee-d343-4851-81a8-ce916f6756db"));
            Assert.NotNull(project);
            Assert.Equal("222.222 MockProject2", project.DisplayMember);
        }

        [Fact]
        public void ShouldLoadProjectsOnlyOnce()
        {
            _viewModel.LoadProjects();
            _viewModel.LoadProjects();

            Assert.Equal(2, _viewModel.Projects.Count);
        }
    }
}
