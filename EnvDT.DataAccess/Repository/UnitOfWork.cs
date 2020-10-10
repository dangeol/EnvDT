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
            Projects = new ProjectRepository(_context);
            Samples = new SampleRepository(_context);
            SampleValues = new SampleValueRepository(_context);
        }

        public ILabReportRepository LabReports { get; private set; }
        public IProjectRepository Projects { get; private set; }
        public ISampleRepository Samples { get; private set; }
        public ISampleValueRepository SampleValues { get; private set; }

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
