using EnvDT.Model.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace EnvDT.DataAccess
{
    public class EnvDTDbContext : DbContext
    {
        public EnvDTDbContext()
        {
            CreateAppDirectory();
        }

        // Guidelines
        public DbSet<CAS> CASs { get; set; }
        public DbSet<Condition> Conditions { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Laboratory> Laboratories { get; set; }
        public DbSet<Medium> Media { get; set; }
        public DbSet<MediumSubType> MediumSubTypes { get; set; }
        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<ParameterLaboratory> ParameterLaboratories { get; set; }
        public DbSet<PublCountry> PublCountries { get; set; }
        public DbSet<Publication> Publications { get; set; }
        public DbSet<PublParam> PublParams { get; set; }
        public DbSet<PublRegion> PublRegions { get; set; }
        public DbSet<RefValue> RefValues { get; set; }
        public DbSet<ValuationClass> ValuationClasses { get; set; }
        public DbSet<ValuationClassCondition> ValuationClassConditions { get; set; }
        public DbSet<ValuationClassMedSubType> ValuationClassMedMedSubTypes { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Unit> Units { get; set; }

        //LabReports
        public DbSet<LabReport> LabReports { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Sample> Samples { get; set; }
        public DbSet<SampleValue> SampleValues { get; set; }

        private const string envDtDir = "EnvDT";
        private static string appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), envDtDir);
        
        private string sqlitePath = Path.Combine(appPath,@"envdt.db");
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={sqlitePath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.RemovePluralizingTableNameConvention();
            // Guidelines
            modelBuilder.ApplyConfiguration(new CASConfig());
            modelBuilder.ApplyConfiguration(new ConditionConfig());
            modelBuilder.ApplyConfiguration(new CountryConfig());
            modelBuilder.ApplyConfiguration(new LaboratoryConfig());
            modelBuilder.ApplyConfiguration(new MediumConfig());
            modelBuilder.ApplyConfiguration(new MediumSubTypeConfig());
            modelBuilder.ApplyConfiguration(new ParameterConfig());
            modelBuilder.ApplyConfiguration(new ParameterLaboratoryConfig());
            modelBuilder.ApplyConfiguration(new PublCountryConfig());
            modelBuilder.ApplyConfiguration(new PublicationConfig());
            modelBuilder.ApplyConfiguration(new PublParamConfig());
            modelBuilder.ApplyConfiguration(new PublRegionConfig());
            modelBuilder.ApplyConfiguration(new RefValueConfig());
            modelBuilder.ApplyConfiguration(new ValuationClassConfig());
            modelBuilder.ApplyConfiguration(new ValuationClassConditionConfig());
            modelBuilder.ApplyConfiguration(new ValuationClassMedSubTypeConfig());
            modelBuilder.ApplyConfiguration(new RegionConfig());
            modelBuilder.ApplyConfiguration(new UnitConfig());

            // LabReports
            modelBuilder.ApplyConfiguration(new LabReportConfig());
            modelBuilder.ApplyConfiguration(new ProjectConfig());
            modelBuilder.ApplyConfiguration(new SampleConfig());
            modelBuilder.ApplyConfiguration(new SampleValueConfig());
        }

        private void CreateAppDirectory()
        {
            Directory.CreateDirectory(Path.Combine(appPath));
        }
    }
}