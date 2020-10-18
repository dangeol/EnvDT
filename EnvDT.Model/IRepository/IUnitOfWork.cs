using System;

namespace EnvDT.Model.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        public ILabReportRepository LabReports { get; }
        public IParameterRepository Parameters { get; }
        public IProjectRepository Projects { get; }
        public IPublicationRepository Publications { get; }
        public IRefValueRepository RefValues { get; }
        public ISampleRepository Samples { get; }
        public ISampleValueRepository SampleValues { get; }
        public IUnitRepository Units { get; }
        public IValuationClassRepository ValuationClasses { get; }
        public int Save();
    }
}
