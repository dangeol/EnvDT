using EnvDT.Model.Core;
using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Event;
using EnvDT.UI.ViewModel;
using EnvDT.UITests.Extensions;
using Moq;
using Prism.Events;
using System;
using Xunit;

namespace EnvDT.UITests.ViewModel
{
    public class MainTabViewModelTests
    {
        private MainTabViewModel _viewModel;
        private Mock<IEventAggregator> _eventAggregatorMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IProjectViewModel> _projectViewModelMock;
        private Mock<Func<ISampleDetailViewModel>> _sampleDetailVmCreatorMock;
        private Mock<IEvalLabReportService> _evalLabReportServiceMock;
        private Mock<ISampleDetailViewModel> _sampleDetailViewModelMock;
        private SampleDetailViewModel _sampleDetailViewModel;
        private OpenDetailViewEvent _openDetailViewEvent;
        private LabReport _labReport;
        private string _detailViewModelName;

        public MainTabViewModelTests()
        {
            _openDetailViewEvent = new OpenDetailViewEvent();
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _eventAggregatorMock.Setup(ea => ea.GetEvent<OpenDetailViewEvent>())
                .Returns(_openDetailViewEvent);
            _labReport = new LabReport();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _projectViewModelMock = new Mock<IProjectViewModel>();
            _sampleDetailVmCreatorMock = new Mock<Func<ISampleDetailViewModel>>();
            _evalLabReportServiceMock = new Mock<IEvalLabReportService>();
            _sampleDetailViewModel = new SampleDetailViewModel(_eventAggregatorMock.Object,
                _unitOfWorkMock.Object, _evalLabReportServiceMock.Object);
            _detailViewModelName = "SampleDetailViewModel";

            _viewModel = new MainTabViewModel(_eventAggregatorMock.Object,
                _projectViewModelMock.Object, CreateSampleDetailViewModel);
        }

        private ISampleDetailViewModel CreateSampleDetailViewModel()
        {
            var sampleDetailViewModelMock = new Mock<ISampleDetailViewModel>();
            sampleDetailViewModelMock.Setup(vm => vm.Load(It.IsAny<Guid>()))
                .Callback<Guid?>(labReportId =>
                {
                    _labReport.LabReportId = labReportId.Value;
                    //not yet implemented
                });
            _sampleDetailViewModelMock = sampleDetailViewModelMock;
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
            var labReportId = new Guid("ce3444d8-adf9-4a7d-a2f7-40ac21905af9");
            _openDetailViewEvent.Publish(
                new OpenDetailViewEventArgs
                {
                    Id = labReportId,
                    ViewModelName = _detailViewModelName
                }
            );

            Assert.Equal(2, _viewModel.TabbedViewModels.Count);
            Assert.Equal(_sampleDetailViewModelMock.Object, _viewModel.SelectedTabbedViewModel);
            _sampleDetailViewModelMock.Verify(sd => sd.Load(labReportId), Times.Once);
        }
    }
}
