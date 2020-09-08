using Moq;
using System;
using Xunit;
using EnvDT.UI.Event;
using Prism.Events;
using EnvDT.UI.ViewModel;

namespace EnvDT.UITests.ViewModel
{
    public class ProjectItemViewModelTests
    {
        [Fact]
        public void ShouldPublishOpenProjectEditViewEvent()
        {
            var lookupItemId = new Guid("d18949b1-076c-4da9-9f7e-7e10ce4553a0");
            var eventMock = new Mock<OpenProjectEditViewEvent>();
            var eventAggregatorMock = new Mock<IEventAggregator>();
            eventAggregatorMock
                .Setup(ea => ea.GetEvent<OpenProjectEditViewEvent>())
                .Returns(eventMock.Object);

            var viewModel = new ProjectItemViewModel(lookupItemId, "MockProject1",
                eventAggregatorMock.Object);

            viewModel.OpenProjectEditViewCommand.Execute(null);

            eventMock.Verify(e => e.Publish(lookupItemId), Times.Once);
        }
    }
}
