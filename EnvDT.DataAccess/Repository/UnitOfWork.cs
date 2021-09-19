using EnvDT.Model.IRepository;

namespace EnvDT.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EnvDTDbContext _context;

        public UnitOfWork(EnvDTDbContext context)
        {
            _context = context;           
            ConfigXlsxs = new ConfigXlsxRepository(_context);
            ConfigCsvs = new ConfigCsvRepository(_context);
            Footnotes = new FootnoteRepository(_context);
            FootnoteParams = new FootnoteParamRepository(_context);
            Laboratories = new LaboratoryRepository(_context);
            LabReports = new LabReportRepository(_context);
            LabReportParams = new LabReportParamRepository(_context);
            Languages = new LanguageRepository(_context);
            Parameters = new ParameterRepository(_context);
            ParamNameVariants = new ParamNameVariantRepository(_context);
            Projects = new ProjectRepository(_context);
            Publications = new PublicationRepository(_context);
            PublParams = new PublParamRepository(_context);
            RefValues = new RefValueRepository(_context);
            Regions = new RegionRepository(_context);
            Samples = new SampleRepository(_context);
            SampleValues = new SampleValueRepository(_context);
            Units = new UnitRepository(_context);
            UnitNameVariants = new UnitNameVariantRepository(_context);
            ValuationClasses = new ValuationClassRepository(_context);
            WasteCodeEWCs = new WasteCodeEWCRepository(_context);
        }

        public IConfigXlsxRepository ConfigXlsxs { get; private set; }
        public IConfigCsvRepository ConfigCsvs { get; private set; }
        public IFootnoteRepository Footnotes { get; private set; }
        public IFootnoteParamRepository FootnoteParams { get; private set; }
        public ILaboratoryRepository Laboratories { get; private set; }
        public ILabReportRepository LabReports { get; private set; }
        public ILabReportParamRepository LabReportParams { get; private set; }
        public ILanguageRepository Languages { get; private set; }
        public IParameterRepository Parameters { get; private set; }
        public IParamNameVariantRepository ParamNameVariants { get; private set; }
        public IProjectRepository Projects { get; private set; }
        public IPublicationRepository Publications { get; private set; }
        public IPublParamRepository PublParams { get; private set; }
        public IRefValueRepository RefValues { get; private set; }
        public IRegionRepository Regions { get; private set; }
        public ISampleRepository Samples { get; private set; }
        public ISampleValueRepository SampleValues { get; private set; }
        public IUnitRepository Units { get; private set; }
        public IUnitNameVariantRepository UnitNameVariants { get; private set; }
        public IValuationClassRepository ValuationClasses { get; private set; }
        public IWasteCodeEWCRepository WasteCodeEWCs { get; private set; }

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
