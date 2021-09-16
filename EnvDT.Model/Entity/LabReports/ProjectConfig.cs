using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model.Entity
{
    public class ProjectConfig : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(p => p.ProjectId);
            builder.Property(p => p.ProjectId).ValueGeneratedOnAdd();
            builder.Property(p => p.ProjectName).IsRequired();
            builder.HasOne(pc => pc.Country)
                .WithMany(c => c.Projects)
                .HasForeignKey(pc => pc.CountryId);
            builder.HasOne(pr => pr.Region)
                .WithMany(r => r.Projects)
                .HasForeignKey(pr => pr.RegionId);
            string seedFile = DbResources.projectJson;
            if (File.Exists(seedFile))
            {
                var projectJson = File.ReadAllText(seedFile);
                var projects = JsonSerializer.Deserialize<List<Project>>(projectJson);
                builder.HasData(projects);
            }
        }
    }
}
