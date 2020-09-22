using Moq;
using System;
using Xunit;
using EnvDT.UI.Event;
using Prism.Events;
using EnvDT.UI.ViewModel;
using EnvDT.UITests.Extensions;

namespace EnvDT.UITests.ViewModel
{
    public class ProjectItemViewModelTests
    {
        private Mock<IEventAggregator> _eventAggregatorMock;
        private ProjectItemViewModel _viewModel;
        private Guid _lookupItemId = new Guid("d18949b1-076c-4da9-9f7e-7e10ce4553a0");

        public ProjectItemViewModelTests()
        {
            _eventAggregatorMock = new Mock<IEventAggregator>();

            _viewModel = new ProjectItemViewModel(_lookupItemId, "MockProject1",
                "ProjectDetailViewModel", _eventAggregatorMock.Object);
        }

        //TO DO: find the reason why this test fails.
        [Fact]
        public void ShouldPublishOpenDetailViewEvent()
        {
            var openDetailViewEventArgs = new OpenDetailViewEventArgs
            {
                Id = It.IsAny<Guid>(),
                ViewModelName = It.IsAny<string>()
            };

            var eventMock = new Mock<OpenDetailViewEvent>();

            _eventAggregatorMock
                .Setup(ea => ea.GetEvent<OpenDetailViewEvent>())
                .Returns(eventMock.Object);

            _viewModel.OpenDetailViewCommand.Execute(null);

            eventMock.Verify(e => e.Publish(openDetailViewEventArgs), Times.Once);
        }

        [Fact]
        public void ShouldRaisePropertyChangedEventForDisplayMember()
        {
            var fired = _viewModel.IsPropertyChangedFired(
                () => { _viewModel.DisplayMember = "changed"; },
                nameof(_viewModel.DisplayMember));

            Assert.True(fired);
        }
    }
}
