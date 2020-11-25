using EnvDT.Model.Entity;
using EnvDT.Model.IDataService;
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
        private Mock<ILookupDataService> _lookupDataServiceMock;
        private HashSet<Guid> _missingParamIds;
        private HashSet<Guid> _missingUnitIds;
        private LookupItem _lapReportParam1;
        private LookupItem _lapReportParam2;
        private List<LookupItem> _lapReportParams = new List<LookupItem>();

        public MissingParamDialogViewModelTests()
        {
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _missingParamIds = new HashSet<Guid>();
            var missingParamId1 = new Guid("a4fbbded-ec20-4fcb-bb84-fb0fde89d02c");
            var missingParamId2 = new Guid("e24a9441-3232-4b88-ae8a-55cdc4245a82");
            _missingParamIds.Add(missingParamId1);
            _missingParamIds.Add(missingParamId2);
            _missingUnitIds = new HashSet<Guid>();
            var missingUnitId1 = new Guid("d94a0c81-0681-4979-ae3c-c43bfe48314f");
            _missingUnitIds.Add(missingUnitId1);
            _lapReportParam1 = new LookupItem();
            _lapReportParam2 = new LookupItem();

            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(uw => uw.Parameters.GetById(missingParamId1))
                .Returns(new Model.Entity.Parameter
                {
                    ParameterId = missingParamId1,
                    ParamNameDe = "ParamName1",
                });
            _unitOfWorkMock.Setup(uw => uw.Parameters.GetById(missingParamId2))
                .Returns(new Model.Entity.Parameter
                {
                    ParameterId = missingParamId2,
                    ParamNameDe = "ParamName2",
                });
            _unitOfWorkMock.Setup(uw => uw.Units.GetById(missingUnitId1))
                .Returns(new Model.Entity.Unit
                {
                    UnitId = missingUnitId1,
                    UnitName = "Unitame1",
                });
            _lookupDataServiceMock = new Mock<ILookupDataService>();

            _lapReportParams.Add(_lapReportParam1);
            _lapReportParams.Add(_lapReportParam2);
            _lookupDataServiceMock.Setup(ld => ld.GetLabReportUnknownParamNamesLookupByLabReportId(It.IsAny<Guid>()))
                .Returns(_lapReportParams);

            _viewModel = new MissingParamDialogViewModel(_eventAggregatorMock.Object, _unitOfWorkMock.Object,
                _lookupDataServiceMock.Object);
        }

        [Fact]
        public void ShouldLoadMissingParamNames()
        {
            _viewModel.Load(It.IsAny<Guid>(), _missingParamIds, _missingUnitIds);

            Assert.NotNull(_viewModel.MissingParamNames);
            Assert.NotNull(_viewModel.MissingUnitNames);
            Assert.Equal(_missingParamIds.Count, _viewModel.MissingParamNames.Count);
            Assert.Equal(_missingUnitIds.Count, _viewModel.MissingUnitNames.Count);

            _unitOfWorkMock.Verify(uw => uw.Parameters.GetById(It.IsAny<Guid>()), Times.Exactly(2));
            _unitOfWorkMock.Verify(uw => uw.Units.GetById(It.IsAny<Guid>()), Times.Exactly(1));
        }
    }
}
