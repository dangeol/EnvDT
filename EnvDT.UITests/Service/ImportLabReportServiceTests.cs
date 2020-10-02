using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Service;
using Moq;
using Prism.Events;
using Xunit;

namespace EnvDT.UITests.Service
{
    public class ImportLabReportServiceTests
    {
        private Mock<IEventAggregator> _eventAggregatorMock;
        private Mock<ILabReportRepository> _labReportRepositoryMock;
        private Mock<ISampleRepository> _sampleRepositoryMock;
        private Mock<ISampleValueRepository> _sampleValueRepositoryMock;
        private Mock<IMessageDialogService> _messageDialogServiceMock;
        private ImportLabReportService _importLabReportService;
        private LabReport _labReport;
        private string _reportLabIdent = "ident";

        public ImportLabReportServiceTests()
        {
            _labReport = new LabReport();
            _labReport.ReportLabIdent = _reportLabIdent;
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _labReportRepositoryMock = new Mock<ILabReportRepository>();
            _labReportRepositoryMock.Setup(lr => lr.GetByReportLabIdent(_reportLabIdent))
                .Returns(_labReport);
            _sampleRepositoryMock = new Mock<ISampleRepository>();
            _sampleValueRepositoryMock = new Mock<ISampleValueRepository>();

            _messageDialogServiceMock = new Mock<IMessageDialogService>();

            _importLabReportService = new ImportLabReportService(_eventAggregatorMock.Object,
                _messageDialogServiceMock.Object, _labReportRepositoryMock.Object,
                _sampleRepositoryMock.Object, _sampleValueRepositoryMock.Object);
        }

        [Fact]
        public void ShouldDisplayMessageWhenLabReportIsAlreadyPresent()
        {
            _importLabReportService.IsLabReportAlreadyPresent(_reportLabIdent);

            _messageDialogServiceMock.Verify(d => d.ShowOkDialog("Import LabReport",
                    $"This LabReport has already been imported. Please chose another file or" +
                    $" delete the LabReport first."),
                Times.Once);
        }
    }
}
