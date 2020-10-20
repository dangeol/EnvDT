using EnvDT.Model.Core;
using EnvDT.Model.IRepository;
using Moq;
using Xunit;

namespace EnvDT.ModelTests.Core
{
    public class EvalLabReportServiceTests
    {
        private EvalLabReportService _evalLabReportService;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IEvalCalcService> _evalCalcServiceMock;

        public EvalLabReportServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _evalCalcServiceMock = new Mock<IEvalCalcService>();

            _evalLabReportService = new EvalLabReportService(_unitOfWorkMock.Object, _evalCalcServiceMock.Object);
        }

        // TO DO
    }
}
