using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model.Entity
{
    public class PublRegionConfig : IEntityTypeConfiguration<PublRegion>
    {
        public void Configure(EntityTypeBuilder<PublRegion> builder)
        {
            builder.HasKey(r => new { r.PublicationId, r.RegionId });
            builder.HasOne(pr => pr.Publication)
                .WithMany(p => p.PublRegions)
                .HasForeignKey(pr => pr.PublicationId);
            builder.HasOne(pr => pr.Region)
                .WithMany(p => p.PublRegions)
                .HasForeignKey(pr => pr.RegionId);
            string seedFile = DbResources.publRegionJson;
            if (File.Exists(seedFile))
            {
                var publRegionJson = File.ReadAllText(seedFile);
                var publRegions = JsonSerializer.Deserialize<List<PublRegion>>(publRegionJson);
                builder.HasData(publRegions);
            }
        }
    }
}
