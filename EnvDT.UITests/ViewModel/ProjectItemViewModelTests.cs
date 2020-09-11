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
                _eventAggregatorMock.Object);
        }

        [Fact]
        public void ShouldPublishOpenProjectEditViewEvent()
        {
            var eventMock = new Mock<OpenProjectEditViewEvent>(); 
            _eventAggregatorMock
                .Setup(ea => ea.GetEvent<OpenProjectEditViewEvent>())
                .Returns(eventMock.Object);

            _viewModel.OpenProjectEditViewCommand.Execute(null);

            eventMock.Verify(e => e.Publish(_lookupItemId), Times.Once);
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
