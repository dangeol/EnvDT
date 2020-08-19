using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model
{
    public class RegionConfig : IEntityTypeConfiguration<Region>
    {
        public void Configure(EntityTypeBuilder<Region> builder)
        {
            builder.HasKey(r => r.RegionId);
            builder.HasOne(rc => rc.Country)
                .WithMany(c => c.Regions)
                .HasForeignKey(rc => rc.CountryId);
            var regionJson = File.ReadAllText(Resources.regionJson);
            var regions = JsonSerializer.Deserialize<List<Region>>(regionJson);
            builder.HasData(regions);
        }
    }
}