using System;

namespace EnvDT.Model.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ILabReportRepository LabReports { get; }
        IProjectRepository Projects { get; }
        ISampleRepository Samples { get; }
        ISampleValueRepository SampleValues { get; }
        int Save();
    }
}
