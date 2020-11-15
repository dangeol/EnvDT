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
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMessageDialogService> _messageDialogServiceMock;
        private Mock<IReadFileHelper> _readFileHelperMock;
        private ImportLabReportService _importLabReportService;
        private LabReport _labReport;
        private string _reportLabIdent = "ident";

        public ImportLabReportServiceTests()
        {
            _labReport = new LabReport();
            _labReport.ReportLabIdent = _reportLabIdent;
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(uw => uw.LabReports.GetByReportLabIdent(_reportLabIdent))
                .Returns(_labReport);
            _messageDialogServiceMock = new Mock<IMessageDialogService>();
            _readFileHelperMock = new Mock<IReadFileHelper>();

            _importLabReportService = new ImportLabReportService(_eventAggregatorMock.Object,
                _messageDialogServiceMock.Object, _unitOfWorkMock.Object, _readFileHelperMock.Object);
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
