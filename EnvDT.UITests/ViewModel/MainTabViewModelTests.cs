using EnvDT.Model.Core;
using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Event;
using EnvDT.UI.ViewModel;
using EnvDT.UITests.Extensions;
using Moq;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace EnvDT.UITests.ViewModel
{
    public class MainTabViewModelTests
    {
        private MainTabViewModel _viewModel;
        private Mock<IEventAggregator> _eventAggregatorMock;
        private Mock<IMessageDialogService> _messageDialogServiceMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IProjectViewModel> _projectViewModelMock;
        private Mock<IEvalLabReportService> _evalLabReportServiceMock;
        private Mock<ISampleEditDialogViewModel> _sampleEditDialogViewModel;
        private Mock<ISampleDetailViewModel> _sampleDetailViewModelMock;
        private Mock<IDispatcher> _dispatcherMock;
        private SampleDetailViewModel _sampleDetailViewModel;
        private OpenDetailViewEvent _openDetailViewEvent;
        private DetailClosedEvent _detailClosedEvent;
        private Mock<ITab> _tabMock;
        private ObservableCollection<IMainTabViewModel> _tabbedViewModels;
        private LabReport _labReport;
        private string _detailViewModelName;
        private Guid? _labReportId1;

        public MainTabViewModelTests()
        {
            _openDetailViewEvent = new OpenDetailViewEvent();
            _detailClosedEvent = new DetailClosedEvent();
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _eventAggregatorMock.Setup(ea => ea.GetEvent<OpenDetailViewEvent>())
                .Returns(_openDetailViewEvent);
            _eventAggregatorMock.Setup(ea => ea.GetEvent<DetailClosedEvent>())
                .Returns(_detailClosedEvent);
            _messageDialogServiceMock = new Mock<IMessageDialogService>();
            _projectViewModelMock = new Mock<IProjectViewModel>();
            _tabbedViewModels = new ObservableCollection<IMainTabViewModel>();
            _sampleDetailViewModelMock = new Mock<ISampleDetailViewModel>();
            _tabMock = new Mock<ITab>();
            _tabMock.Setup(t => t.TabbedViewModels).Returns(_tabbedViewModels);
            _labReport = new LabReport();
            _unitOfWorkMock = new Mock<IUnitOfWork>();            
            _evalLabReportServiceMock = new Mock<IEvalLabReportService>();
            _sampleEditDialogViewModel = new Mock<ISampleEditDialogViewModel>();
            _dispatcherMock = new Mock<IDispatcher>();
            _detailViewModelName = "SampleDetailViewModel";
            _labReportId1 = new Guid("ce3444d8-adf9-4a7d-a2f7-40ac21905af9");
            _sampleDetailViewModel = new SampleDetailViewModel(_eventAggregatorMock.Object,
                _messageDialogServiceMock.Object, _unitOfWorkMock.Object, 
                _evalLabReportServiceMock.Object, _sampleEditDialogViewModel.Object,
                _dispatcherMock.Object);

            _viewModel = new MainTabViewModel(_eventAggregatorMock.Object, _tabMock.Object,
                _projectViewModelMock.Object, CreateSampleDetailViewModel);
        }

        private ISampleDetailViewModel CreateSampleDetailViewModel()
        {
            var sampleDetailViewModelMock = new Mock<ISampleDetailViewModel>();
            sampleDetailViewModelMock.Setup(vm => vm.Load(It.IsAny<Guid>()))
                .Callback<Guid?>(labReportId =>
                {
                    _labReport.LabReportId = labReportId.Value;
                });
            _sampleDetailViewModelMock = sampleDetailViewModelMock;
            _sampleDetailViewModelMock.SetupGet(vm => vm.LabReportId)
                .Returns(_labReportId1);
            return sampleDetailViewModelMock.Object;
        }

        [Fact]
        public void ShouldRaisePropertyChangedEventForSelectedTabbedViewModel()
        {
            var fired = _viewModel.IsPropertyChangedFired(() =>
            {
                _viewModel.SelectedTabbedViewModel = _sampleDetailViewModel;
            }, nameof(_viewModel.SelectedTabbedViewModel));

            Assert.True(fired);
        }

        [Fact]
        public void ShouldCallAddProjectViewModelToTabbedViewModels()
        {
            Assert.Single(_viewModel.TabbedViewModels);
        }

        [Fact]
        public void ShouldLoadSampleDetailViewModelIntoNewTabAndFocusToItWhenOpenDetailViewEventExecuted()
        {
            _openDetailViewEvent.Publish(
                new OpenDetailViewEventArgs
                {
                    Id = _labReportId1,
                    ViewModelName = _detailViewModelName
                }
            );

            Assert.Equal(2, _viewModel.TabbedViewModels.Count);
            Assert.Equal(_sampleDetailViewModelMock.Object, _viewModel.SelectedTabbedViewModel);
            _sampleDetailViewModelMock.Verify(sd => sd.Load(_labReportId1), Times.Once);
        }

        [Fact]
        public void ShouldLoadSampleDetailViewModelOnlyOnce()
        {
            _openDetailViewEvent.Publish(
                new OpenDetailViewEventArgs
                {
                    Id = _labReportId1,
                    ViewModelName = _detailViewModelName
                }
            );
            _tabMock.Setup(t => t.GetTabbedViewModelByEventArgs(It.IsAny<IDetailEventArgs>()))
                .Returns(_tabbedViewModels.Last());
            _viewModel.SelectedTabbedViewModel = _viewModel.TabbedViewModels.First();
            var tabsCountBefore = _viewModel.TabbedViewModels.Count;
            _openDetailViewEvent.Publish(
                new OpenDetailViewEventArgs
                {
                    Id = _labReportId1,
                    ViewModelName = _detailViewModelName
                }
            );
            var tabsCountAfter = _viewModel.TabbedViewModels.Count;

            Assert.Equal(tabsCountBefore, tabsCountAfter);
            Assert.Equal(_viewModel.TabbedViewModels.Last().LabReportId, 
                _viewModel.SelectedTabbedViewModel.LabReportId);
        }

        [Fact]
        public void ShouldRemoveTabbedViewModelItemWhenSampleDetailViewIsClosed()
        {
            _openDetailViewEvent.Publish(
                new OpenDetailViewEventArgs
                {
                    Id = _labReportId1,
                    ViewModelName = _detailViewModelName
                }
            );

            Assert.Equal(2, _viewModel.TabbedViewModels.Count);

            _tabMock.Setup(t => t.GetTabbedViewModelByEventArgs(It.IsAny<IDetailEventArgs>()))
                .Returns(_tabbedViewModels.Last());

            _detailClosedEvent.Publish(
                new DetailClosedEventArgs
                {
                    Id = _labReportId1,
                    ViewModelName = "ISampleDetailViewModelProxy"
                });

            Assert.Single(_viewModel.TabbedViewModels);
            Assert.Equal(_projectViewModelMock.Object, _viewModel.SelectedTabbedViewModel);
        }
    }
}
