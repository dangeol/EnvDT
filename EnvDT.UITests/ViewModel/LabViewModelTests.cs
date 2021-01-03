using EnvDT.Model.Entity;
using EnvDT.Model.IDataService;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Event;
using EnvDT.UI.ViewModel;
using EnvDT.UI.Wrapper;
using EnvDT.UITests.Extensions;
using Moq;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EnvDT.UITests.ViewModel
{
    public class LabViewModelTests
    {
        private LabViewModel _viewModel;
        private Mock<ILabDetailViewModel> _labDetailViewModelMock;

        private Mock<IEventAggregator> _eventAggregatorMock;
        private Mock<IMessageDialogService> _messageDialogServiceMock;
        private Mock<ILookupDataService> _lookupDataServiceMock;
        private OpenDetailViewEvent _openDetailViewEvent;
        private DetailSavedEvent _labSavedEvent;
        private DetailDeletedEvent _detailDeletedEvent;
        private Laboratory _laboratory;
        private string _detailViewModelName;

        public LabViewModelTests()
        {
            _openDetailViewEvent = new OpenDetailViewEvent();
            _labSavedEvent = new DetailSavedEvent();
            _detailDeletedEvent = new DetailDeletedEvent();
            _laboratory = new Laboratory();
            _detailViewModelName = "LabDetailViewModel";
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _eventAggregatorMock.Setup(ea => ea.GetEvent<OpenDetailViewEvent>())
                .Returns(_openDetailViewEvent);
            _eventAggregatorMock.Setup(ea => ea.GetEvent<DetailSavedEvent>())
                .Returns(_labSavedEvent);
            _eventAggregatorMock.Setup(ea => ea.GetEvent<DetailDeletedEvent>())
                .Returns(_detailDeletedEvent);

            _messageDialogServiceMock = new Mock<IMessageDialogService>();

            _lookupDataServiceMock = new Mock<ILookupDataService>();
            _lookupDataServiceMock.Setup(ld => ld.GetAllLaboratoriesLookup())
                .Returns(new List<LookupItem>
                {
                    new LookupItem
                    {
                        LookupItemId = new Guid("929ed16b-74b3-4915-805a-bcdf8133f790"),
                        DisplayMember = "MockLab1"
                    },
                    new LookupItem
                    {
                        LookupItemId = new Guid("acb98980-0c5c-4de7-be95-3563c92cd2b6"),
                        DisplayMember = "MockLab2"
                    }
                });
            _viewModel = new LabViewModel(
                _lookupDataServiceMock.Object,
                _eventAggregatorMock.Object,
                CreateLabDetailViewModel,
                _messageDialogServiceMock.Object);
        }

        private ILabDetailViewModel CreateLabDetailViewModel()
        {
            var labDetailViewModelMock = new Mock<ILabDetailViewModel>();
            labDetailViewModelMock.Setup(vm => vm.Load(It.IsAny<Guid>()))
                .Callback<Guid?>(labId =>
                {
                    _laboratory.LaboratoryId = labId.Value;
                    labDetailViewModelMock.Setup(vm => vm.Laboratory)
                    .Returns(new LabWrapper(_laboratory));
                });
            _labDetailViewModelMock = labDetailViewModelMock;
            return labDetailViewModelMock.Object;
        }

        [Fact]
        public void ShouldCallTheLoadMethod()
        {
            //2 Labs + 1 NavItemViewModelNull
            Assert.Equal(3, _viewModel.Laboratories.Count);
        }

        [Fact]
        public void ShouldLoadLabs()
        {
            _viewModel.LoadModels();

            Assert.Equal(3, _viewModel.Laboratories.Count);

            var lab = _viewModel.Laboratories.SingleOrDefault(
                p => p.LookupItemId == Guid.Parse("929ed16b-74b3-4915-805a-bcdf8133f790"));
            Assert.NotNull(lab);
            Assert.Equal("MockLab1", lab.DisplayMember);

            lab = _viewModel.Laboratories.SingleOrDefault(
                p => p.LookupItemId == Guid.Parse("acb98980-0c5c-4de7-be95-3563c92cd2b6"));
            Assert.NotNull(lab);
            Assert.Equal("MockLab2", lab.DisplayMember);
        }

        [Fact]
        public void ShouldLoadLabDetailViewModel()
        {
            var labId = new Guid("a1b57e3b-3aa6-4f1c-801c-d160853811f3");
            _openDetailViewEvent.Publish(
            new OpenDetailViewEventArgs
            {
                Id = labId,
                ViewModelName = _detailViewModelName
            });

            Assert.NotNull(_viewModel.DetailViewModel);
            var labDetailVm = _viewModel.DetailViewModel;
            Assert.Equal(labDetailVm, _viewModel.DetailViewModel);
            _labDetailViewModelMock.Verify(vm => vm.Load(labId), Times.Once);
        }

        [Fact]
        public void ShouldNotLoadLabDetailViewModelWhenDefaultItemIsSelected()
        {
            _openDetailViewEvent.Publish(
                new OpenDetailViewEventArgs
                {
                    Id = new Guid(),
                    ViewModelName = _detailViewModelName
                });
            Assert.Null(_viewModel.DetailViewModel);
        }

            [Fact]
        public void ShouldLoadLabDetailViewModelAndLoadItWithIdNull()
        {
            Type type = typeof(LabDetailViewModel);
            _viewModel.CreateNewDetailCommand.Execute(parameter: type);

            Assert.NotNull(_viewModel.DetailViewModel);
            Assert.Equal(_labDetailViewModelMock.Object, _viewModel.DetailViewModel);
            _labDetailViewModelMock.Verify(vm => vm.Load(null), Times.Once);
        }

        [Fact]
        public void ShouldEnableLabDetailViewWhenLabSelected()
        {
            var labId = new Guid("a1b57e3b-3aa6-4f1c-801c-d160853811f3");
            _openDetailViewEvent.Publish(
                new OpenDetailViewEventArgs
                {
                    Id = labId,
                    ViewModelName = _detailViewModelName
                });

            Assert.True(_viewModel.IsDetailViewEnabled);
        }

        [Fact]
        public void ShouldRaisePropertyChangedEventForSelectedLab()
        {
            var labDetailVmMock = new Mock<ILabDetailViewModel>();
            var fired = _viewModel.IsPropertyChangedFired(() =>
            {
                _viewModel.DetailViewModel = labDetailVmMock.Object;
            }, nameof(_viewModel.DetailViewModel));

            Assert.True(fired);
        }

        [Fact]
        public void ShouldLoadLabsOnlyOnce()
        {
            _viewModel.LoadModels();
            _viewModel.LoadModels();

            Assert.Equal(3, _viewModel.Laboratories.Count);
        }

        [Fact]
        public void ShouldUpdateLabItemWhenLabSaved()
        {
            _viewModel.LoadModels();

            var labItem = _viewModel.Laboratories.First();

            var labId = labItem.LookupItemId;
            var labName = "ProName1Changed";

            _labSavedEvent.Publish(
                new DetailSavedEventArgs
                {
                    Id = labId,
                    DisplayMember = $"{labName}",
                    ViewModelName = nameof(LabDetailViewModel)
                });

            Assert.Equal("ProName1Changed", labItem.DisplayMember);
        }

        [Fact]
        public void ShouldAddLabItemWhenAddedLabIsSaved()
        {
            _viewModel.LoadModels();

            var newLabId = new Guid("097e364a-5ef3-40a0-bde5-51caa26d7f48");
            var labName = "New Lab";

            _labSavedEvent.Publish(
                new DetailSavedEventArgs
                {
                    Id = newLabId,
                    DisplayMember = $"{labName}",
                    ViewModelName = nameof(LabDetailViewModel)
                });

            Assert.Equal(4, _viewModel.Laboratories.Count);

            var addDetailItem = _viewModel.Laboratories.SingleOrDefault(p => p.LookupItemId == newLabId);
            Assert.NotNull(addDetailItem);
            Assert.Equal("New Lab", addDetailItem.DisplayMember);
        }

        [Fact]
        public void ShouldRemoveLabItemWhenLabIsDeleted()
        {
            _viewModel.LoadModels();

            var existingLabId = _viewModel.Laboratories.First().LookupItemId;

            _detailDeletedEvent.Publish(
            new DetailDeletedEventArgs
            {
                Id = existingLabId,
                ViewModelName = _detailViewModelName
            });

            Assert.Equal(2, _viewModel.Laboratories.Count);

            var deletDetailem = _viewModel.Laboratories.SingleOrDefault(p => p.LookupItemId == existingLabId);
            Assert.Null(deletDetailem);
        }

        [Fact]
        public void ShouldRemoveDetailViewModelOnDeletedEvent()
        {
            var deletedLabId = Guid.Parse("8abbae81-ad7e-4453-8546-1278b625c50f");
            _openDetailViewEvent.Publish(
            new OpenDetailViewEventArgs
            {
                Id = Guid.Parse("d24af541-2cda-4580-bbb1-675b41f16dea"),
                ViewModelName = _detailViewModelName
            });
            _openDetailViewEvent.Publish(
            new OpenDetailViewEventArgs
            {
                Id = Guid.Parse("2f9f2fa5-9db0-4aae-ba28-113e282e65c5"),
                ViewModelName = _detailViewModelName
            });

            _detailDeletedEvent.Publish(
            new DetailDeletedEventArgs
            {
                Id = deletedLabId,
                ViewModelName = _detailViewModelName
            });

            Assert.Null(_viewModel.DetailViewModel);
            Assert.False(_viewModel.IsDetailViewEnabled);
        }

        [Theory]
        [InlineData(MessageDialogResult.Yes, 1)]
        [InlineData(MessageDialogResult.No, 0)]
        public void ShouldNotLoadNewLabDetailViewModelWhenUnsavedChangesLeft(
            MessageDialogResult result, int labViewLoaded)
        {    
            _messageDialogServiceMock.Setup(ds => ds.ShowYesNoDialog(It.IsAny<string>(),
                It.IsAny<string>())).Returns(result);

            var labId1 = new Guid("a1b57e3b-3aa6-4f1c-801c-d160853811f3");
            _openDetailViewEvent.Publish(
            new OpenDetailViewEventArgs
            {
                Id = labId1,
                ViewModelName = _detailViewModelName
            });

            _labDetailViewModelMock.Setup(vm => vm.HasChanges).Returns(true);

            var labId2 = new Guid("aa4ec543-065e-41c5-8324-ccb39d071d0b");
            _openDetailViewEvent.Publish(
            new OpenDetailViewEventArgs
            {
                Id = labId2,
                ViewModelName = _detailViewModelName
            });

            _labDetailViewModelMock.Verify(vm => vm.Load(labId2), Times.Exactly(labViewLoaded));
            _messageDialogServiceMock.Verify(ds => ds.ShowYesNoDialog(It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }
    }
}

