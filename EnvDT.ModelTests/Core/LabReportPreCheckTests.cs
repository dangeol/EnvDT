using EnvDT.Model.Core;
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
        private Publication _publication;
        private PublParam _publParam;
        private List<PublParam> _publParams;
        private List<LabReportParam> _labReportParams;
        private LabReportParam _labReportParam;
        private MissingParamNameWrapper _missingParamNameWrapper;
        private ParamNameVariant _paramNameVariant;
        private ObservableCollection<MissingParamNameWrapper> _missingParamNames;
        private HashSet<Guid> _missingParamIds;
        private List<Guid> _publicationIds;

        public LabReportPreCheckTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _messageDialogServiceMock = new Mock<IMessageDialogService>();
            _publParam = new PublParam();
            _publParam.ParameterId = new Guid("019df8e5-0042-4e0a-b5b3-93686a81de6b");
            _publParams = new List<PublParam>();
            _publParams.Add(_publParam);
            _publication = new Publication();
            _publication.PublParams = _publParams;
            _labReportParams = new List<LabReportParam>();
            _labReportParam = new LabReportParam();
            _paramNameVariant = new ParamNameVariant();
            _missingParamNameWrapper = new MissingParamNameWrapper(_paramNameVariant);
            _missingParamNames = new ObservableCollection<MissingParamNameWrapper>();
            _missingParamIds = new HashSet<Guid>();
            _missingParamIds.Add(new Guid("4ef4a153-c930-4c70-b437-1eac16bccbf6"));
            _publicationIds = new List<Guid>();
            _publicationIds.Add(new Guid("9be712a3-9c92-4d2f-b595-dcc0fcb092f1"));

            _unitOfWorkMock.Setup(uw => uw.Publications.GetById(It.IsAny<Guid>()))
                .Returns(_publication);

            _labReportPreCheck = new LabReportPreCheck(_unitOfWorkMock.Object, 
                _messageDialogServiceMock.Object, CreateMissingParamDetailViewModel);
        }

        private IMissingParamDialogViewModel CreateMissingParamDetailViewModel()
        {
            var missingParamDetailViewModelMock = new Mock<IMissingParamDialogViewModel>();
            missingParamDetailViewModelMock.Setup(mp => mp.Load(It.IsAny<HashSet<Guid>>()))
                .Callback<HashSet<Guid>>(missingParamIds =>
                {
                    //missingParamIds = new HashSet<Guid>();
                    //missingParamIds = _missingParamIds;
                    foreach (Guid missingParamId in _missingParamIds)
                    {
                        missingParamIds.Add(_missingParamNameWrapper.ParameterId);
                    }
                    missingParamDetailViewModelMock.Setup(mp => mp.MissingParamNames)
                        .Returns(new ObservableCollection<MissingParamNameWrapper>(_missingParamNames));
                });
            _missingParamDetailViewModelMock = missingParamDetailViewModelMock;
            return missingParamDetailViewModelMock.Object;
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

            _missingParamDetailViewModelMock.Verify(mp => mp.Load(It.IsAny<HashSet<Guid>>()), Times.Once);
            _messageDialogServiceMock.Verify(ds => ds.ShowMissingParamDialog(It.IsAny<string>(),
                _missingParamDetailViewModelMock.Object), Times.Once);
        }
    }
}
