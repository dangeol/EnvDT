using EnvDT.Model.Core;
using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Moq;
using System;
using System.Collections.Generic;

namespace EnvDT.ModelTests.Core
{
    public class LabReportPreCheckTests
    {
        private LabReportPreCheck _labReportPreCheck;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Publication _publication;
        private PublParam _publParam;
        private List<PublParam> _publParams;
        private List<LabReportParam> _labReportParams;
        private LabReportParam _labReportParam;

        public LabReportPreCheckTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _publParam = new PublParam();
            _publParams = new List<PublParam>();
            _publParams.Add(_publParam);
            _labReportParams = new List<LabReportParam>();
            _labReportParam = new LabReportParam();
            _labReportParams.Add(_labReportParam);

            _unitOfWorkMock.Setup(uw => uw.Publications.GetById(It.IsAny<Guid>()))
                .Returns(_publication);
            _unitOfWorkMock.Setup(uw => uw.LabReportParams.GetLabReportParamsByPublParam(It.IsAny<PublParam>(), It.IsAny<Guid>()))
                .Returns(_labReportParams);

            _labReportPreCheck = new LabReportPreCheck(_unitOfWorkMock.Object);
        }
    }
}
