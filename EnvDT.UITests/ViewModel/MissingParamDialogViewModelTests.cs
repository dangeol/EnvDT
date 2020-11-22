﻿using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.ViewModel;
using EnvDT.UI.Wrapper;
using EnvDT.UITests.Extensions;
using Moq;
using Prism.Events;
using System;
using System.Collections.Generic;
using Xunit;

namespace EnvDT.UITests.ViewModel
{
    public class MissingParamDialogViewModelTests
    {
        private MissingParamDialogViewModel _viewModel;
        private Mock<IEventAggregator> _eventAggregatorMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private HashSet<Guid> _missingParamIds;
        private LabReportParam _lapReportParam1;
        private LabReportParam _lapReportParam2;
        private List<LabReportParam> _lapReportParams = new List<LabReportParam>();

        public MissingParamDialogViewModelTests()
        {
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _missingParamIds = new HashSet<Guid>();
            var missingParamId1 = new Guid("a4fbbded-ec20-4fcb-bb84-fb0fde89d02c");
            var missingParamId2 = new Guid("e24a9441-3232-4b88-ae8a-55cdc4245a82");
            _missingParamIds.Add(missingParamId1);
            _missingParamIds.Add(missingParamId2);
            _lapReportParam1 = new LabReportParam();
            _lapReportParam2 = new LabReportParam();

            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(pr => pr.Parameters.GetById(missingParamId1))
                .Returns(new Model.Entity.Parameter
                {
                    ParameterId = missingParamId1,
                    ParamNameDe = "ParamName1",
                });
            _unitOfWorkMock.Setup(pr => pr.Parameters.GetById(missingParamId2))
                .Returns(new Model.Entity.Parameter
                {
                    ParameterId = missingParamId2,
                    ParamNameDe = "ParamName2",
                });

            _lapReportParams.Add(_lapReportParam1);
            _lapReportParams.Add(_lapReportParam2);
            _unitOfWorkMock.Setup(pr => pr.LabReportParams.GetLabReportUnknownParamNamesByLabReportId(It.IsAny<Guid>()))
                .Returns(_lapReportParams);

            _viewModel = new MissingParamDialogViewModel(_eventAggregatorMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public void ShouldLoadMissingParamNames()
        {
            _viewModel.Load(It.IsAny<Guid>(), _missingParamIds);

            Assert.NotNull(_viewModel.MissingParamNames);
            Assert.Equal(_missingParamIds.Count, _viewModel.MissingParamNames.Count);

            _unitOfWorkMock.Verify(uw => uw.Parameters.GetById(It.IsAny<Guid>()), Times.Exactly(2));
        }
    }
}
