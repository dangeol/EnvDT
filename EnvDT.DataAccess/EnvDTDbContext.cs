﻿using EnvDT.Model.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace EnvDT.DataAccess
{
    public class EnvDTDbContext : DbContext
    {
        private DbContextOptions<EnvDTDbContext> _options;
        private const string _envDtDir = "EnvDT";
        private static string _appPath;
        private string _sqlitePath;

        public EnvDTDbContext()
        {
            CreateAppDirectory();
            _appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _envDtDir);
            _sqlitePath = Path.Combine(_appPath, @"envdt.db");
        }

        public EnvDTDbContext(DbContextOptions<EnvDTDbContext> options)
            : base(options)
        {
            _options = options;
        }

        // Guidelines
        public DbSet<CAS> CASs { get; set; }
        public DbSet<Condition> Conditions { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Medium> Media { get; set; }
        public DbSet<MediumSubType> MediumSubTypes { get; set; }
        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<ParamNameVariant> ParamNameVariants { get; set; }
        public DbSet<PublCountry> PublCountries { get; set; }
        public DbSet<Publication> Publications { get; set; }
        public DbSet<PublParam> PublParams { get; set; }
        public DbSet<PublRegion> PublRegions { get; set; }
        public DbSet<RefValue> RefValues { get; set; }
        public DbSet<UnitNameVariant> UnitNameVariants { get; set; }
        public DbSet<ValuationClass> ValuationClasses { get; set; }
        public DbSet<ValuationClassCondition> ValuationClassConditions { get; set; }
        public DbSet<ValuationClassMedSubType> ValuationClassMedMedSubTypes { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Unit> Units { get; set; }

        //LabReports
        public DbSet<ConfigXlsx> ConfigXlsxs { get; set; }
        public DbSet<Laboratory> Laboratories { get; set; }
        public DbSet<LabReport> LabReports { get; set; }
        public DbSet<LabReportParam> LabReportParams { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Sample> Samples { get; set; }
        public DbSet<SampleValue> SampleValues { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (_options == null)
            {
                options.UseSqlite($"Data Source={_sqlitePath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.RemovePluralizingTableNameConvention();
            // Guidelines
            modelBuilder.ApplyConfiguration(new CASConfig());
            modelBuilder.ApplyConfiguration(new ConditionConfig());
            modelBuilder.ApplyConfiguration(new CountryConfig());
            modelBuilder.ApplyConfiguration(new LanguageConfig());
            modelBuilder.ApplyConfiguration(new MediumConfig());
            modelBuilder.ApplyConfiguration(new MediumSubTypeConfig());
            modelBuilder.ApplyConfiguration(new ParameterConfig());
            modelBuilder.ApplyConfiguration(new ParamNameVariantConfig());
            modelBuilder.ApplyConfiguration(new PublCountryConfig());
            modelBuilder.ApplyConfiguration(new PublicationConfig());
            modelBuilder.ApplyConfiguration(new PublParamConfig());
            modelBuilder.ApplyConfiguration(new PublRegionConfig());
            modelBuilder.ApplyConfiguration(new RefValueConfig());
            modelBuilder.ApplyConfiguration(new UnitNameVariantConfig());
            modelBuilder.ApplyConfiguration(new ValuationClassConfig());
            modelBuilder.ApplyConfiguration(new ValuationClassConditionConfig());
            modelBuilder.ApplyConfiguration(new ValuationClassMedSubTypeConfig());
            modelBuilder.ApplyConfiguration(new RegionConfig());
            modelBuilder.ApplyConfiguration(new UnitConfig());

            // LabReports
            modelBuilder.ApplyConfiguration(new ConfigXlsxConfig());
            modelBuilder.ApplyConfiguration(new LaboratoryConfig());
            modelBuilder.ApplyConfiguration(new LabReportConfig());
            modelBuilder.ApplyConfiguration(new ProjectConfig());
            modelBuilder.ApplyConfiguration(new SampleConfig());
            modelBuilder.ApplyConfiguration(new SampleValueConfig());
        }

        private void CreateAppDirectory()
        {
            try
            {
                Directory.CreateDirectory(Path.Combine(_appPath));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}