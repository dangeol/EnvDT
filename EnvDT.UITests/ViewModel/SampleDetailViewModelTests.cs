using EnvDT.Model.IRepository;
using EnvDT.Model.Core;
using Moq;
using Prism.Events;
using System;
using Xunit;
using EnvDT.UI.ViewModel;
using EnvDT.UI.Event;
using EnvDT.Model.Entity;
using System.Collections.Generic;
using System.Linq;
using EnvDT.Model.Core.HelperEntity;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Wrapper;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EnvDT.UITests.ViewModel
{
    public class SampleDetailViewModelTests
    {
        private Guid _labReportId = new Guid("35b396f3-d755-49a6-a748-54b55e46a6c6");
        private Mock<IEventAggregator> _eventAggregatorMock;
        private Mock<IMessageDialogService> _messageDialogServiceMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IEvalLabReportService> _evalLabReportServiceMock;
        private Mock<ISampleEditDialogViewModel> _sampleEditDialogViewModelMock;
        private SampleDetailViewModel _viewModel;
        private Mock<DetailClosedEvent> _detailClosedEventMock;
        private Mock<IMissingParamDialogViewModel> _missingParamDialogVM;
        private Mock<ISampleEditDialogViewModel> _sampleEditDialogVM;
        private Mock<IDispatcher> _dispatcherMock;
        private List<Sample> _samples;
        private LabReport _labReport;
        private List<Publication> _publications;
        private string _reportLabIdent = "ident";
        private EvalResult _evalResult;
        private HashSet<string> _generalFootnoteTexts;

        public SampleDetailViewModelTests()
        {
            _detailClosedEventMock = new Mock<DetailClosedEvent>();
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _eventAggregatorMock.Setup(ea => ea.GetEvent<DetailClosedEvent>())
                .Returns(_detailClosedEventMock.Object);
            _messageDialogServiceMock = new Mock<IMessageDialogService>();
            _samples = new List<Sample>();
            _samples.Add(new Model.Entity.Sample
            {
                SampleId = new Guid("1f343ed6-e410-4f4b-9432-073acada899b"),
                SampleName = "Sample1",
                MediumSubTypeId = new Guid("9a523236-b656-4c0a-a873-c272b15b1e83"),
                ConditionId = new Guid("2f94eb45-8da4-4817-85e2-530377869016")
            });
            _samples.Add(new Model.Entity.Sample
            {
                SampleId = new Guid("f515fda2-e370-4a22-b20c-4ffdc5c12503"),
                SampleName = "Sample2",
                MediumSubTypeId = Guid.Empty,
                ConditionId = Guid.Empty
            }); 
             _labReport = new LabReport();
            _labReport.ReportLabIdent = _reportLabIdent;
            _publications = new List<Publication>();
            _publications.Add(new Model.Entity.Publication
            {
                PublicationId = new Guid("b2dbef1c-680d-400e-a8a4-7f540b360fa5"),
                Abbreviation = "Publ1",
                OrderId = 1

            });
            _publications.Add(new Model.Entity.Publication
            {
                PublicationId = new Guid("09d6aa16-006f-4e7d-b595-00f747eefad6"),
                Abbreviation = "Publ2",
                OrderId = 2
            });
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(uw => uw.Samples.GetSamplesByLabReportId(_labReportId))
                .Returns(_samples);
            _unitOfWorkMock.Setup(uw => uw.LabReports.GetById(It.IsAny<Guid>()))
                .Returns(_labReport);
            _unitOfWorkMock.Setup(uw => uw.Publications.GetAll())
                .Returns(_publications);
            _evalLabReportServiceMock = new Mock<IEvalLabReportService>();
            _evalLabReportServiceMock.Setup(er => er.LabReportPreCheck(It.IsAny<Guid>(), It.IsAny<IReadOnlyCollection<Guid>>()))
                .Returns(true);

            var sampleWrapper1 = new SampleWrapper(_samples[0]);
            var sampleWrapper2 = new SampleWrapper(_samples[1]);
            var sampleWrappers = new ObservableCollection<SampleWrapper>();
            sampleWrappers.Add(sampleWrapper1);
            sampleWrappers.Add(sampleWrapper2);
            _sampleEditDialogViewModelMock = new Mock<ISampleEditDialogViewModel>();
            _sampleEditDialogViewModelMock.Setup(vm => vm.Samples)
                .Returns(sampleWrappers);

            _generalFootnoteTexts = new();
            _evalResult = new EvalResult();
            _evalResult.GeneralFootnoteTexts = _generalFootnoteTexts;
            _evalResult.MissingParams = "";
            _evalResult.MinValueParams = "";
            _evalResult.TakingAccountOf = "";
            _evalResult.ExceedingValues = "Param1 (Value 1 Unit 1)";
            _evalLabReportServiceMock.Setup(er => er.GetEvalResult(It.IsAny<EvalArgs>()))
                .Returns(_evalResult);

            _missingParamDialogVM = new Mock<IMissingParamDialogViewModel>();
            _sampleEditDialogVM = new Mock<ISampleEditDialogViewModel>();
            _dispatcherMock = new Mock<IDispatcher>();
            _dispatcherMock.Setup(x => x.Invoke(It.IsAny<Action>()))
                .Callback((Action a) => a());

            _viewModel = new SampleDetailViewModel(_eventAggregatorMock.Object,
                _messageDialogServiceMock.Object, _unitOfWorkMock.Object,
                _evalLabReportServiceMock.Object, _sampleEditDialogViewModelMock.Object,
                _dispatcherMock.Object);
        }

        [Fact]
        public void ShouldLoadSamplesAndSetTabTitle()
        {
            _viewModel.Load(_labReportId);

            Assert.Equal(2, _viewModel.Samples.Count());

            Assert.NotNull(_viewModel.Title);
            Assert.Equal(_viewModel.Title, _reportLabIdent);
        }

        [Fact]
        public void ShouldPublishDetailClosedEventWhenCloseDetailViewCommandIsExecuted()
        {
            _viewModel.Load(_labReportId);

            _viewModel.CloseDetailViewCommand.Execute(null);

            _detailClosedEventMock.Verify(e => e.Publish(It.IsAny<DetailClosedEventArgs>()), Times.Once);
        }

        [Fact]
        public async Task ShouldBuildCorrectEvalResultDataViewDependingOnSampleDataViewTable()
        {
            _viewModel.Load(_labReportId);
            _viewModel.SampleDataView.Table.Rows[0][1] = true;
            _viewModel.SampleDataView.Table.Rows[0][2] = false;
            _viewModel.SampleDataView.Table.Rows[1][1] = true;
            _viewModel.SampleDataView.Table.Rows[1][2] = false;

            Assert.False(_viewModel.IsEvalResultVisible);

            await _viewModel.OnEvalExecuteImpl();

            //1 Publication selected
            Assert.Equal(1, _viewModel.SelectedPublsDataView.Table.Rows.Count);
            //2 samples in labreport
            Assert.Equal(2, _viewModel.EvalResultDataView.Table.Rows.Count);
            //For each selected publication, two new columns are added; 
            //1 sample name column + 2 new colums = 3.
            Assert.Equal(3, _viewModel.EvalResultDataView.Table.Columns.Count);
            Assert.Equal(_samples[0].SampleName, _viewModel.EvalResultDataView.Table.Rows[0][0]);
            Assert.Equal(_samples[1].SampleName, _viewModel.EvalResultDataView.Table.Rows[1][0]);            

            _viewModel.SampleDataView.Table.Rows[0][1] = false;
            _viewModel.SampleDataView.Table.Rows[0][2] = false;
            _viewModel.SampleDataView.Table.Rows[1][1] = true;
            _viewModel.SampleDataView.Table.Rows[1][2] = true;

            await _viewModel.OnEvalExecuteImpl();

            Assert.True(_viewModel.IsEvalResultVisible);

            //2 Publications selected
            Assert.Equal(2, _viewModel.SelectedPublsDataView.Table.Rows.Count);
            Assert.Equal(1, _viewModel.EvalResultDataView.Table.Rows.Count);
            Assert.Equal(5, _viewModel.EvalResultDataView.Table.Columns.Count);
            //No missing params
            Assert.Equal(0, _viewModel.FootnotesDataView.Table.Rows.Count);
            _messageDialogServiceMock.Verify(ds => ds.ShowSampleEditDialog(It.IsAny<string>(),
                (ISampleEditDialogViewModel)It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task ShouldShowSampleEditDialogWhenSelectedPublicationUsesMEdSubTypesOrConditions()
        {
            _publications.Add(new Model.Entity.Publication
            {
                PublicationId = new Guid("12bd0b3f-c703-4e54-aee2-9f39e297dcb1"),
                Abbreviation = "Publ3",
                OrderId = 3,
                UsesMediumSubTypes = true,
                UsesConditions = true
            });

            _viewModel.Load(_labReportId);
            _viewModel.SampleDataView.Table.Rows[1][3] = true;

            await _viewModel.OnEvalExecuteImpl();

            _messageDialogServiceMock.Verify(ds => ds.ShowSampleEditDialog(It.IsAny<string>(),
                (ISampleEditDialogViewModel)It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task ShouldShowGeneralFootnoteTextsInFootnotesWhenNecessary()
        {
            HashSet<string> _generalFootnoteTexts = new();
            _generalFootnoteTexts.Add("new footnote text");
            _generalFootnoteTexts.Add("new footnote text");
            _generalFootnoteTexts.Add("another footnote text");
            _evalResult.GeneralFootnoteTexts = _generalFootnoteTexts;

            _viewModel.Load(_labReportId);
            _viewModel.SampleDataView.Table.Rows[1][1] = true;
            _viewModel.SampleDataView.Table.Rows[1][2] = true;

            await _viewModel.OnEvalExecuteImpl();

            Assert.Equal(2, _viewModel.FootnotesDataView.Table.Rows.Count);
        }

        [Fact]
        public async Task ShouldShowMissingParamsInFootnotesWhenParametersAreMissing()
        {
            _evalResult.MissingParams = "missing params string";

            _viewModel.Load(_labReportId);
            _viewModel.SampleDataView.Table.Rows[1][1] = true;
            _viewModel.SampleDataView.Table.Rows[1][2] = true;

            await _viewModel.OnEvalExecuteImpl();

            // Currently 2 samples checked
            // Only one line, because both times, same MissingParams string
            Assert.Equal(1, _viewModel.FootnotesDataView.Table.Rows.Count);
        }

        [Fact]
        public async Task ShouldShowMinValueParamsInFootnotesWhenNecessary()
        {
            _evalResult.MinValueParams = "MinValueParams";

            _viewModel.Load(_labReportId);
            _viewModel.SampleDataView.Table.Rows[1][1] = true;
            _viewModel.SampleDataView.Table.Rows[1][2] = true;

            await _viewModel.OnEvalExecuteImpl();

            Assert.Equal(1, _viewModel.FootnotesDataView.Table.Rows.Count);
        }
    }
}
