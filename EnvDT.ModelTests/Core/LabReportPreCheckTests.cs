using EnvDT.Model.Core;
using EnvDT.Model.Core.HelperEntity;
using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.ViewModel;
using EnvDT.UI.Wrapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace EnvDT.ModelTests.Core
{
    public class LabReportPreCheckTests
    {
        private LabReportPreCheck _labReportPreCheck;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMessageDialogService> _messageDialogServiceMock;
        private Mock<IMissingParamDialogViewModel> _missingParamDetailViewModelMock;
        private Mock<IDispatcher> _dispatcherMock;
        private Mock<IFootnotes> _footnotesMock;
        private FootnoteResult _footnoteResult;
        private Publication _publication;
        private PublParam _publParam;
        private List<PublParam> _publParams;
        private List<LabReportParam> _labReportParams;
        private LabReportParam _labReportParam;
        private MissingParamNameWrapper _missingParamNameWrapper;
        private ParamNameVariant _paramNameVariant;
        private ObservableCollection<MissingParamNameWrapper> _missingParamNames;
        private ObservableCollection<MissingUnitNameWrapper> _missingUnitNames;
        private HashSet<Guid> _missingParamIds;
        private HashSet<Guid> _missingUnitIds;
        private List<Guid> _publicationIds;

        public LabReportPreCheckTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _messageDialogServiceMock = new Mock<IMessageDialogService>();
            _dispatcherMock = new Mock<IDispatcher>();
            _dispatcherMock.Setup(x => x.Invoke(It.IsAny<Action>()))
                .Callback((Action a) => a());
            _footnoteResult = new FootnoteResult();
            _footnoteResult.Result = true;
            _footnotesMock = new Mock<IFootnotes>();
            _footnotesMock.Setup(fn => fn.IsFootnoteCondTrue(It.IsAny<EvalArgs>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(_footnoteResult);
            _publParam = new PublParam();
            _publParam.ParameterId = new Guid("019df8e5-0042-4e0a-b5b3-93686a81de6b");
            _publParam.IsMandatory = true;
            _publParam.FootnoteId = "";
            _publParams = new List<PublParam>();
            _publParams.Add(_publParam);
            _publication = new Publication();
            _publication.PublParams = _publParams;
            _labReportParams = new List<LabReportParam>();
            _labReportParam = new LabReportParam();
            _paramNameVariant = new ParamNameVariant();
            _missingParamNameWrapper = new MissingParamNameWrapper(_paramNameVariant);
            _missingParamNames = new ObservableCollection<MissingParamNameWrapper>();
            _missingUnitNames = new ObservableCollection<MissingUnitNameWrapper>();
            _missingParamIds = new HashSet<Guid>();
            _missingParamIds.Add(new Guid("4ef4a153-c930-4c70-b437-1eac16bccbf6"));
            _missingUnitIds = new HashSet<Guid>();
            _missingUnitIds.Add(new Guid("90936f80-0363-45b3-9e6c-63374098ca99"));
            _publicationIds = new List<Guid>();
            _publicationIds.Add(new Guid("9be712a3-9c92-4d2f-b595-dcc0fcb092f1"));            

            _unitOfWorkMock.Setup(uw => uw.Publications.GetById(It.IsAny<Guid>()))
                .Returns(_publication);

            _labReportPreCheck = new LabReportPreCheck(_unitOfWorkMock.Object, 
                _messageDialogServiceMock.Object, CreateMissingParamDetailViewModel,
                _dispatcherMock.Object, _footnotesMock.Object);
        }

        private IMissingParamDialogViewModel CreateMissingParamDetailViewModel()
        {
            var missingParamDialogViewModelMock = new Mock<IMissingParamDialogViewModel>();
            missingParamDialogViewModelMock.Setup(mp => mp.Load(It.IsAny<Guid>(), 
                    It.IsAny<HashSet<Guid>>(), It.IsAny<HashSet<Guid>>()))
                .Callback<Guid, HashSet<Guid>, HashSet<Guid>>((labReportId, missingParamIds, missingUnitIds) =>
                {
                    missingParamDialogViewModelMock.Setup(mp => mp.MissingParamNames)
                        .Returns(new ObservableCollection<MissingParamNameWrapper>(_missingParamNames));
                    missingParamDialogViewModelMock.Setup(mp => mp.MissingUnitNames)
                        .Returns(new ObservableCollection<MissingUnitNameWrapper>(_missingUnitNames));
                });
            _missingParamDetailViewModelMock = missingParamDialogViewModelMock;
            return missingParamDialogViewModelMock.Object;
        }

        [Fact]
        public void ShouldLoadMissingParamDetailViewModelWhenMissingParamsFound()
        {
            _unitOfWorkMock.Setup(uw => uw.LabReportParams.GetLabReportParamsByPublParam(It.IsAny<PublParam>(), It.IsAny<Guid>()))
                .Returns(_labReportParams);
            _unitOfWorkMock.Setup(uw => uw.LabReportParams.GetLabReportParamNamesByPublParam(It.IsAny<PublParam>(), It.IsAny<Guid>()))
                .Returns(_labReportParams);

            var readOnlyPublicationIds = new ReadOnlyCollection<Guid>(_publicationIds);
            _labReportPreCheck.FindMissingParametersUnits(It.IsAny<Guid>(), readOnlyPublicationIds);

            _missingParamDetailViewModelMock.Verify(mp => mp.Load(It.IsAny<Guid>(), It.IsAny<HashSet<Guid>>(),
                It.IsAny<HashSet<Guid>>()), Times.Once);
            _messageDialogServiceMock.Verify(ds => ds.ShowMissingParamDialog(It.IsAny<string>(),
                _missingParamDetailViewModelMock.Object), Times.Once);
        }
    }
}
