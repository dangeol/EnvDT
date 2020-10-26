using EnvDT.Model.IRepository;

namespace EnvDT.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EnvDTDbContext _context;

        public UnitOfWork(EnvDTDbContext context)
        {
            _context = context;
            LabReports = new LabReportRepository(_context);
            Parameters = new ParameterRepository(_context);
            Projects = new ProjectRepository(_context);
            Publications = new PublicationRepository(_context);
            PublParams = new PublParamRepository(_context);
            RefValues = new RefValueRepository(_context);
            Samples = new SampleRepository(_context);
            SampleValues = new SampleValueRepository(_context);
            Units = new UnitRepository(_context);
            ValuationClasses = new ValuationClassRepository(_context);
        }

        public ILabReportRepository LabReports { get; private set; }
        public IParameterRepository Parameters { get; private set; }
        public IProjectRepository Projects { get; private set; }
        public IPublicationRepository Publications { get; private set; }
        public IPublParamRepository PublParams { get; private set; }
        public IRefValueRepository RefValues { get; private set; }
        public ISampleRepository Samples { get; private set; }
        public ISampleValueRepository SampleValues { get; private set; }
        public IUnitRepository Units { get; private set; }
        public IValuationClassRepository ValuationClasses { get; private set; }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
