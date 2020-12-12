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
        private string _lapReportParamName1;
        private string _lapReportParamName2;
        private string _lapReportUnitName1;
        private List<string> _lapReportParamNames = new List<string>();

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
            _lapReportParamName1 = "ParamName1";
            _lapReportParamName2 = "ParamName2";
            _lapReportUnitName1 = "UnitName1";

            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(uw => uw.Parameters.GetById(missingParamId1))
                .Returns(new Model.Entity.Parameter
                {
                    ParameterId = missingParamId1,
                    ParamNameDe = _lapReportParamName1,
                });
            _unitOfWorkMock.Setup(uw => uw.Parameters.GetById(missingParamId2))
                .Returns(new Model.Entity.Parameter
                {
                    ParameterId = missingParamId2,
                    ParamNameDe = _lapReportParamName2,
                });
            _unitOfWorkMock.Setup(uw => uw.Units.GetById(missingUnitId1))
                .Returns(new Model.Entity.Unit
                {
                    UnitId = missingUnitId1,
                    UnitName = _lapReportUnitName1,
                });
            _unitOfWorkMock.Setup(uw => uw.Units.GetById(missingUnitId1))
                .Returns(new Model.Entity.Unit
                {
                    UnitId = missingUnitId1,
                    UnitName = "Unitame1",
                });
            _unitOfWorkMock.Setup(uw => uw.ParamNameVariants.GetAll())
                .Returns(It.IsAny<List<ParamNameVariant>>());
            _unitOfWorkMock.Setup(uw => uw.UnitNameVariants.GetAll())
                .Returns(It.IsAny<List<UnitNameVariant>>());
            _lookupDataServiceMock = new Mock<ILookupDataService>();

            _lapReportParamNames.Add(_lapReportParamName1);
            _lapReportParamNames.Add(_lapReportParamName2);
            _unitOfWorkMock.Setup(uw => uw.LabReportParams.GetLabReportUnknownParamNamesByLabReportId(It.IsAny<Guid>()))
                .Returns(_lapReportParamNames);

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

        [Fact]
        public void ShoudlRaisePropertyChangedEventForIsMissingParamNamesVisible()
        {
            var fired = _viewModel.IsPropertyChangedFired(
                () => _viewModel.Load(It.IsAny<Guid>(), _missingParamIds, _missingUnitIds),
                nameof(_viewModel.IsMissingParamNamesVisible));

            Assert.True(fired);
        }

        [Fact]
        public void ShouldDisableSaveCommandWhenViewModelIsLoaded()
        {
            _viewModel.Load(It.IsAny<Guid>(), _missingParamIds, _missingUnitIds);

            Assert.False(_viewModel.SaveCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldEnableSaveCommandWhenWrappersHaveNoValidationErrors()
        {
            _viewModel.Load(It.IsAny<Guid>(), _missingParamIds, _missingUnitIds);

            _viewModel.MissingParamNames[0].ParamNameAlias = _lapReportParamName1;
            _viewModel.MissingParamNames[0].LanguageId = new Guid("6289ff3f-4f1c-40db-9ba8-d788dacb8371");
            _viewModel.MissingParamNames[1].ParamNameAlias = _lapReportParamName2;
            _viewModel.MissingParamNames[1].LanguageId = new Guid("e2100847-1bd8-4a7e-88d4-68b48e8a1d31");
            _viewModel.MissingUnitNames[0].UnitNameAlias = _lapReportUnitName1;

            Assert.True(_viewModel.SaveCommand.CanExecute(null));
        }
    }
}
