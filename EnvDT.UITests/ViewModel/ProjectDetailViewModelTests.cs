using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Event;
using EnvDT.UI.ViewModel;
using EnvDT.UITests.Extensions;
using Moq;
using Prism.Events;
using System;
using Xunit;
using System.Collections.ObjectModel;

namespace EnvDT.UITests.ViewModel
{
    public class ProjectDetailViewModelTests
    {
        private Guid _projectId = new Guid("77ec605f-3909-471f-a866-a2c4759bf5a0");
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IEventAggregator> _eventAggregatorMock;
        private Mock<IMessageDialogService> _messageDialogServiceMock;
        private Mock<ILabReportViewModel> _labReportViewModelMock;
        private ProjectDetailViewModel _viewModel;
        private Mock<DetailSavedEvent> _projectSavedEventMock;
        private Mock<DetailDeletedEvent> _projectDeletedEventMock;

        public ProjectDetailViewModelTests()
        {
            _projectSavedEventMock = new Mock<DetailSavedEvent>();
            _projectDeletedEventMock = new Mock<DetailDeletedEvent>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(pr => pr.Projects.GetById(_projectId))
                .Returns(new Model.Entity.Project { ProjectId = _projectId, 
                    ProjectNumber = "012345", ProjectName = "name" });
            _eventAggregatorMock = new Mock<IEventAggregator>(); 
            _eventAggregatorMock.Setup(ea => ea.GetEvent<DetailSavedEvent>())
                .Returns(_projectSavedEventMock.Object);
            _eventAggregatorMock.Setup(ea => ea.GetEvent<DetailDeletedEvent>())
                .Returns(_projectDeletedEventMock.Object);

            _messageDialogServiceMock = new Mock<IMessageDialogService>();

            _viewModel = new ProjectDetailViewModel(_unitOfWorkMock.Object, 
                _eventAggregatorMock.Object, _messageDialogServiceMock.Object,
                CreateLabReportViewModel);
        }

        private ILabReportViewModel CreateLabReportViewModel()
        {
            var labReportViewModelMock = new Mock<ILabReportViewModel>();
            labReportViewModelMock.Setup(vm => vm.Load(It.IsAny<Guid>()))
                .Callback<Guid?>(projectId =>
                {
                    _projectId = projectId.Value;
                    labReportViewModelMock.Setup(vm => vm.LabReports)
                        .Returns(new ObservableCollection<NavItemViewModel>());
                });
            _labReportViewModelMock = labReportViewModelMock;
            return labReportViewModelMock.Object;
        }

        [Fact]
        public void ShouldLoadProject()
        {
            _viewModel.Load(_projectId);

            Assert.NotNull(_viewModel.Project);
            Assert.Equal(_projectId, _viewModel.Project.ProjectId);

            _unitOfWorkMock.Verify(uw => uw.Projects.GetById(_projectId), Times.Once);
        }

        [Fact]
        public void ShouldLoadLabReportViewModel()
        {
            _viewModel.Load(_projectId);

            _labReportViewModelMock.Verify(lr => lr.Load(_projectId), Times.Once);
        }
        
        [Fact]
        public void ShoudlRaisePropertyChangedEventForProject()
        {
            var fired = _viewModel.IsPropertyChangedFired(
                () => _viewModel.Load(_projectId),
                nameof(_viewModel.Project));

            Assert.True(fired);
        }

        [Fact]
        public void ShouldDisableSaveCommandWhenProjectIsLoaded()
        {
            _viewModel.Load(_projectId);

            Assert.False(_viewModel.SaveCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldDisableSaveCommandWhenProjectHasValidationErrors()
        {
            _viewModel.Load(_projectId);

            _viewModel.Project.ProjectName = "";

            Assert.False(_viewModel.SaveCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldEnableSaveCommandWhenProjectIsChanged()
        {
            _viewModel.Load(_projectId);
            //_viewModel.Project.ProjectNumber = "Changed";
            _viewModel.HasChanges = true;

            Assert.True(_viewModel.SaveCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldDisableSaveCommandWithoutLoad()
        {
            Assert.False(_viewModel.SaveCommand.CanExecute(null));
        }

        /* TO DO
        [Fact]
        public void ShouldRaiseCanExecuteChangedForSaveCommandWhenProjectIsChanged()
        {
            _viewModel.Load(_projectId);
            var fired = false;
            _viewModel.SaveCommand.CanExecuteChanged += (s, e) => fired = true;
            _viewModel.Project.ProjectNumber = "Changed";
            Assert.True(fired);
        }

        [Fact]
        public void ShouldRaiseCanExecuteChangedForSaveCommandAfterLoad()
        {
            var fired = false;
            _viewModel.SaveCommand.CanExecuteChanged += (s, e) => fired = true;
            _viewModel.Load(_projectId);
            Assert.True(fired);
        }

        [Fact]
        public void ShouldRaiseCanExecuteChangedForDeleteCommandWhenAcceptingChanges()
        {
            _viewModel.Load(_projectId);
            var fired = false;
            _viewModel.Project.ProjectNumber = "Changed";
            _viewModel.DeleteCommand.CanExecuteChanged += (s, e) => fired = true;
            _viewModel.HasChanges = true;
            Assert.True(fired);
        }   
        
        [Fact]
        public void ShouldRaiseCanExecuteChangedForDeleteCommandAfterLoad()
        {
            var fired = false;
            _viewModel.DeleteCommand.CanExecuteChanged += (s, e) => fired = true;
            _viewModel.Load(_projectId);
            Assert.True(fired);
        }
        */

        [Fact]
        public void ShouldCallSaveMethodOfProjectRepositoryWhenSaveCommandIsExecuted()
        {
            _viewModel.Load(_projectId);
            _viewModel.Project.ProjectNumber = "Changed";

            _viewModel.SaveCommand.Execute(null);
            _unitOfWorkMock.Verify(uw => uw.Save(), Times.Once);
        }

        [Fact]
        public void ShouldAcceptChangesWhenSaveCommandIsExecuted()
        {
            _viewModel.Load(_projectId);
            _viewModel.Project.ProjectNumber = "Changed";

            _viewModel.SaveCommand.Execute(null);
            Assert.False(_viewModel.HasChanges);
        }

        [Fact]
        public void ShouldPublishProjectSavedEventWhenSaveCommandIsExecuted()
        {
            _viewModel.Load(_projectId);
            _viewModel.Project.ProjectNumber = "Changed";

            _viewModel.SaveCommand.Execute(null);

            _projectSavedEventMock.Verify(e => e.Publish(It.IsAny<DetailSavedEventArgs>()), Times.Once);
        }

        [Fact]
        public void ShouldCreateNewProjectWhenNullIsPassedToLoadMethod()
        {
            _viewModel.Load(null);

            Assert.NotNull(_viewModel.Project);
            Assert.Equal(Guid.Empty, _viewModel.Project.ProjectId);
            Assert.Null(_viewModel.Project.ProjectNumber);
            Assert.Null(_viewModel.Project.ProjectClient);
            Assert.Equal("", _viewModel.Project.ProjectName);
            Assert.Null(_viewModel.Project.ProjectAddress);

            _unitOfWorkMock.Verify(uw => uw.Projects.GetById(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public void ShouldEnableDeleteCommandForExistingProject()
        {
            _viewModel.Load(_projectId);

            Assert.True(_viewModel.DeleteCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldDisableDeleteCommandForNewProject()
        {
            _viewModel.Load(null);

            Assert.False(_viewModel.DeleteCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldDisableDeleteCommandWithoudLoad()
        {
            Assert.False(_viewModel.DeleteCommand.CanExecute(null));
        }

        [Theory]
        [InlineData(MessageDialogResult.Yes, 1, true)]
        [InlineData(MessageDialogResult.No, 0, false)]
        public void ShouldCallDeleteMethodOfProjectRepositoryWhenDeleteCommandIsExecuted(
            MessageDialogResult result, int expectedDeleteProjectCalls, bool laboratoryReportViewModelIsNull)
        {
            _viewModel.Load(_projectId);

            _messageDialogServiceMock.Setup(ds => ds.ShowYesNoDialog(It.IsAny<string>(),
                It.IsAny<string>())).Returns(result);

            _viewModel.DeleteCommand.Execute(null);

            _unitOfWorkMock.Verify(uw => uw.Projects.Delete(_viewModel.Project.Model), 
                Times.Exactly(expectedDeleteProjectCalls));
            Assert.Equal(laboratoryReportViewModelIsNull, _viewModel.LabReportViewModel == null);
            _messageDialogServiceMock.Verify(ds => ds.ShowYesNoDialog(It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData(MessageDialogResult.Yes, 1)]
        [InlineData(MessageDialogResult.No, 0)]
        public void ShouldPublishProjectDeletedEventWhenDeleteCommandIsExecuted(
            MessageDialogResult result, int expectedPublishCalls)
        {
            _viewModel.Load(_projectId);

            _messageDialogServiceMock.Setup(ds => ds.ShowYesNoDialog(It.IsAny<string>(),
                It.IsAny<string>())).Returns(result);

            _viewModel.DeleteCommand.Execute(null);

            _projectDeletedEventMock.Verify(e => e.Publish(It.IsAny<DetailDeletedEventArgs>()), 
                Times.Exactly(expectedPublishCalls));

            _messageDialogServiceMock.Verify(ds => ds.ShowYesNoDialog(It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ShouldDisplayCorrectMessageInDeleteDialog()
        {
            _viewModel.Load(_projectId);

            var p = _viewModel.Project;
            p.ProjectClient = "Client";
            p.ProjectName = "ProjectName";

            _viewModel.DeleteCommand.Execute(null);

            _messageDialogServiceMock.Verify(d => d.ShowYesNoDialog("Delete Project",
                $"Do you really want to delete the friend '{p.ProjectClient} {p.ProjectName}'?"),
                Times.Once);
        }
    }
}
