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
using EnvDT.Model.IDataService;
using EnvDT.UI.Wrapper;
using EnvDT.Model.Entity;

namespace EnvDT.UITests.ViewModel
{
    public class LabDetailViewModelTests
    {
        private Guid _labId = new Guid("9c9f2215-c57d-447d-b5f5-816764c15fd0");
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IEventAggregator> _eventAggregatorMock;
        private Mock<IMessageDialogService> _messageDialogServiceMock;
        private Mock<ILookupDataService> _lookupDataServiceMock;
        private Mock<ILabReportViewModel> _labReportViewModelMock;
        private LabDetailViewModel _viewModel;
        private Mock<DetailSavedEvent> _labSavedEventMock;
        private Mock<DetailDeletedEvent> _labDeletedEventMock;
        private Mock<ISampleDetailViewModel> _sampleDetailViewModelMock;
        private Mock<ILabViewModel> _labViewModelMock;
        private ObservableCollection<IMainTabViewModel> _tabbedViewModels;
        private Mock<IConfigXlsxDetailViewModel> _configXlsxDetailViewModelMock;
        private Mock<IConfigXmlDetailViewModel> _configXmlDetailViewModelMock;
        private ConfigXlsx _configXlsx;
        private ConfigXml _configXml;


        public LabDetailViewModelTests()
        {
            _labSavedEventMock = new Mock<DetailSavedEvent>();
            _labDeletedEventMock = new Mock<DetailDeletedEvent>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(pr => pr.Laboratories.GetById(_labId))
                .Returns(new Model.Entity.Laboratory {
                    LaboratoryId = _labId, 
                    LabCompany = "company", LabName = "name" });
            _eventAggregatorMock = new Mock<IEventAggregator>(); 
            _eventAggregatorMock.Setup(ea => ea.GetEvent<DetailSavedEvent>())
                .Returns(_labSavedEventMock.Object);
            _eventAggregatorMock.Setup(ea => ea.GetEvent<DetailDeletedEvent>())
                .Returns(_labDeletedEventMock.Object);
            _messageDialogServiceMock = new Mock<IMessageDialogService>();
            _lookupDataServiceMock = new Mock<ILookupDataService>(); 
            _labViewModelMock = new Mock<ILabViewModel>();
            _configXlsx = new ConfigXlsx();
            _configXml = new ConfigXml();

            _viewModel = new LabDetailViewModel(_unitOfWorkMock.Object, 
                _eventAggregatorMock.Object, _messageDialogServiceMock.Object,
                _lookupDataServiceMock.Object, CreateConfigXlsxDetailViewModel,
                CreateConfigXmlDetailViewModel);
        }

        private IConfigXlsxDetailViewModel CreateConfigXlsxDetailViewModel()
        {
            var configXlsxDetailViewModelMock = new Mock<IConfigXlsxDetailViewModel>();
            configXlsxDetailViewModelMock.Setup(vm => vm.Load(It.IsAny<Guid>()))
                .Callback<Guid?>(laboratoryId =>
                {
                    _configXlsx.LaboratoryId = laboratoryId.Value;
                    configXlsxDetailViewModelMock.Setup(vm => vm.ConfigXlsx)
                        .Returns(new ConfigXlsxWrapper(_configXlsx));
                });
            _configXlsxDetailViewModelMock = configXlsxDetailViewModelMock;
            return configXlsxDetailViewModelMock.Object;
        }

        private IConfigXmlDetailViewModel CreateConfigXmlDetailViewModel()
        {
            var configXmlDetailViewModelMock = new Mock<IConfigXmlDetailViewModel>();
            configXmlDetailViewModelMock.Setup(vm => vm.Load(It.IsAny<Guid>()))
                .Callback<Guid?>(laboratoryId =>
                {
                    _configXml.LaboratoryId = laboratoryId.Value;
                    configXmlDetailViewModelMock.Setup(vm => vm.ConfigXml)
                        .Returns(new ConfigXmlWrapper(_configXml));
                });
            _configXmlDetailViewModelMock = configXmlDetailViewModelMock;
            return configXmlDetailViewModelMock.Object;
        }

        [Fact]
        public void ShouldLoadLab()
        {
            _viewModel.Load(_labId);

            Assert.NotNull(_viewModel.Laboratory);
            Assert.Equal(_labId, _viewModel.Laboratory.LaboratoryId);

            _unitOfWorkMock.Verify(uw => uw.Laboratories.GetById(_labId), Times.Once);
        }
        
        [Fact]
        public void ShoudlRaisePropertyChangedEventForLab()
        {
            var fired = _viewModel.IsPropertyChangedFired(
                () => _viewModel.Load(_labId),
                nameof(_viewModel.Laboratory));

            Assert.True(fired);
        }

        [Fact]
        public void ShouldDisableSaveCommandWhenLabIsLoaded()
        {
            _viewModel.Load(_labId);

            Assert.False(_viewModel.SaveCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldDisableSaveCommandWhenLabHasValidationErrors()
        {
            _viewModel.Load(_labId);

            _viewModel.Laboratory.LabName = "";

            Assert.False(_viewModel.SaveCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldEnableSaveCommandWhenLabIsChanged()
        {
            _viewModel.Load(_labId);
            //_viewModel.Lab.LabCompany = "Changed";
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
        public void ShouldRaiseCanExecuteChangedForSaveCommandWhenLabIsChanged()
        {
            _viewModel.Load(_labId);
            var fired = false;
            _viewModel.SaveCommand.CanExecuteChanged += (s, e) => fired = true;
            _viewModel.Lab.LabCompany = "Changed";
            Assert.True(fired);
        }

        [Fact]
        public void ShouldRaiseCanExecuteChangedForSaveCommandAfterLoad()
        {
            var fired = false;
            _viewModel.SaveCommand.CanExecuteChanged += (s, e) => fired = true;
            _viewModel.Load(_labId);
            Assert.True(fired);
        }

        [Fact]
        public void ShouldRaiseCanExecuteChangedForDeleteCommandWhenAcceptingChanges()
        {
            _viewModel.Load(_labId);
            var fired = false;
            _viewModel.Lab.LabCompany = "Changed";
            _viewModel.DeleteCommand.CanExecuteChanged += (s, e) => fired = true;
            _viewModel.HasChanges = true;
            Assert.True(fired);
        }   
        
        [Fact]
        public void ShouldRaiseCanExecuteChangedForDeleteCommandAfterLoad()
        {
            var fired = false;
            _viewModel.DeleteCommand.CanExecuteChanged += (s, e) => fired = true;
            _viewModel.Load(_labId);
            Assert.True(fired);
        }
        */

        [Fact]
        public void ShouldCallSaveMethodOfLabRepositoryWhenSaveCommandIsExecuted()
        {
            _viewModel.Load(_labId);
            _viewModel.Laboratory.LabCompany = "Changed";

            _viewModel.SaveCommand.Execute(null);
            _unitOfWorkMock.Verify(uw => uw.Save(), Times.Once);
        }

        [Fact]
        public void ShouldAcceptChangesWhenSaveCommandIsExecuted()
        {
            _viewModel.Load(_labId);
            _viewModel.Laboratory.LabCompany = "Changed";

            _viewModel.SaveCommand.Execute(null);
            Assert.False(_viewModel.HasChanges);
        }

        [Fact]
        public void ShouldPublishLabSavedEventWhenSaveCommandIsExecuted()
        {
            _viewModel.Load(_labId);
            _viewModel.Laboratory.LabCompany = "Changed";

            _viewModel.SaveCommand.Execute(null);

            _labSavedEventMock.Verify(e => e.Publish(It.IsAny<DetailSavedEventArgs>()), Times.Once);
        }

        [Fact]
        public void ShouldCreateNewLabWhenNullIsPassedToLoadMethod()
        {
            _viewModel.Load(null);

            Assert.NotNull(_viewModel.Laboratory);
            Assert.Equal(Guid.Empty, _viewModel.Laboratory.LaboratoryId);
            //Assert.Null(_viewModel.Laboratory.LabCompany);
            Assert.Equal("", _viewModel.Laboratory.LabName);

            _unitOfWorkMock.Verify(uw => uw.Laboratories.GetById(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public void ShouldEnableDeleteCommandForExistingLab()
        {
            _viewModel.Load(_labId);

            Assert.True(_viewModel.DeleteCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldDisableDeleteCommandForNewLab()
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
        [InlineData(MessageDialogResult.OK, 1, true)]
        [InlineData(MessageDialogResult.Cancel, 0, false)]
        public void ShouldCallDeleteMethodOfLabRepositoryWhenDeleteCommandIsExecuted(
            MessageDialogResult result, int expectedDeleteLabCalls, bool configDetailViewModelIsNull)
        {
            _viewModel.Load(_labId);

            _messageDialogServiceMock.Setup(ds => ds.ShowOkCancelDialog(It.IsAny<string>(),
                It.IsAny<string>())).Returns(result);

            _viewModel.DeleteCommand.Execute(null);

            _unitOfWorkMock.Verify(uw => uw.Laboratories.Delete(_viewModel.Laboratory.Model), 
                Times.Exactly(expectedDeleteLabCalls));
            //Assert.Equal(configDetailViewModelIsNull, _viewModel.ConfigXlsxDetailViewModel == null);
            //Assert.Equal(configDetailViewModelIsNull, _viewModel.ConfigXmlDetailViewModel == null);
            _messageDialogServiceMock.Verify(ds => ds.ShowOkCancelDialog(It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData(MessageDialogResult.OK, 1)]
        [InlineData(MessageDialogResult.Cancel, 0)]
        public void ShouldPublishLabDeletedEventWhenDeleteCommandIsExecuted(
            MessageDialogResult result, int expectedPublishCalls)
        {
            _viewModel.Load(_labId);

            _messageDialogServiceMock.Setup(ds => ds.ShowOkCancelDialog(It.IsAny<string>(),
                It.IsAny<string>())).Returns(result);

            _viewModel.DeleteCommand.Execute(null);

            _labDeletedEventMock.Verify(e => e.Publish(It.IsAny<DetailDeletedEventArgs>()), 
                Times.Exactly(expectedPublishCalls));

            _messageDialogServiceMock.Verify(ds => ds.ShowOkCancelDialog(It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ShouldDisplayCorrectMessageInDeleteDialog()
        {
            _viewModel.Load(_labId);

            var p = _viewModel.Laboratory;
            p.LabCompany = "LabCompany";
            p.LabName = "LabName";

            _viewModel.DeleteCommand.Execute(null);

            _messageDialogServiceMock.Verify(d => d.ShowOkCancelDialog(It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }
    }
}
