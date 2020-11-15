using EnvDT.Model.IRepository;
using EnvDT.UI.ViewModel;
using Moq;
using Prism.Events;
using Xunit;

namespace EnvDT.UITests.ViewModel
{
    public class MissingParamDialogViewModelTests
    {
        private MissingParamDialogViewModel _viewModel;
        private Mock<IEventAggregator> _eventAggregatorMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;

        public MissingParamDialogViewModelTests()
        {
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _viewModel = new MissingParamDialogViewModel(_eventAggregatorMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public void template()
        {
        }
    }
}
