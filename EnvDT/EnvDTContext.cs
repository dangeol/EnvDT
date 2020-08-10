using System.Collections.Generic;
using EnvDT.Model;
using Microsoft.EntityFrameworkCore;

namespace EnvDT
{
    public class EnvDTContext : DbContext
    {
        public DbSet<CAS> CASs { get; set; }
        public DbSet<Condition> Conditions { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Medium> Media { get; set; }
        public DbSet<MediumSubType> MediumSubTypes { get; set; }
        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<PublCountry> PublCountries { get; set; }
        public DbSet<Publication> Publications { get; set; }
        public DbSet<PublRegion> PublRegions { get; set; }
        public DbSet<RefValue> RefValues { get; set; }
        public DbSet<RefValueMedSubType> RefValueMedSubTypes { get; set; }
        public DbSet<RefValueName> RefValueNames { get; set; }
        public DbSet<RefValueParam> RefValueParams { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Unit> Units { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=envdt.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CASConfig());
            modelBuilder.ApplyConfiguration(new ConditionConfig());
            modelBuilder.ApplyConfiguration(new CountryConfig());
            modelBuilder.ApplyConfiguration(new MediumConfig());
            modelBuilder.ApplyConfiguration(new MediumSubTypeConfig());
            modelBuilder.ApplyConfiguration(new ParameterConfig());
            modelBuilder.ApplyConfiguration(new PublCountryConfig());
            modelBuilder.ApplyConfiguration(new PublicationConfig());
            modelBuilder.ApplyConfiguration(new PublRegionConfig());
            modelBuilder.ApplyConfiguration(new RefValueConfig());
            modelBuilder.ApplyConfiguration(new RefValueMedSubTypeConfig());
            modelBuilder.ApplyConfiguration(new RefValueNameConfig());
            modelBuilder.ApplyConfiguration(new RefValueParamConfig());
            modelBuilder.ApplyConfiguration(new RegionConfig());
            modelBuilder.ApplyConfiguration(new UnitConfig());
        }
    }
}