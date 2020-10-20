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

        public EvalLabReportServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _evalLabReportService = new EvalLabReportService(_unitOfWorkMock.Object);
        }
    }
}
