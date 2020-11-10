using EnvDT.Model.Core;
using EnvDT.Model.IRepository;
using Moq;

namespace EnvDT.ModelTests.Core
{
    public class LabReportPreCheckTests
    {
        private LabReportPreCheck _labReportPreCheck;
        private Mock<IUnitOfWork> _unitOfWorkMock;

        public LabReportPreCheckTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _labReportPreCheck = new LabReportPreCheck(_unitOfWorkMock.Object);
        }
    }
}
