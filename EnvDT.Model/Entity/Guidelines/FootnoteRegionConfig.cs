using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model.Entity
{
    public class FootnoteRegionConfig : IEntityTypeConfiguration<FootnoteRegion>
    {
        public void Configure(EntityTypeBuilder<FootnoteRegion> builder)
        {
            builder.HasKey(r => new { r.FootnoteId, r.RegionId });
            builder.HasOne(fr => fr.Footnote)
                .WithMany(f => f.FootnoteRegions)
                .HasForeignKey(pr => pr.FootnoteId);
            builder.HasOne(fr => fr.Region)
                .WithMany(f => f.FootnoteRegions)
                .HasForeignKey(fr => fr.RegionId);
            string seedFile = DbResources.footnoteRegionJson;
            if (File.Exists(seedFile))
            {
                var footnoteRegionJson = File.ReadAllText(seedFile);
                var footnoteRegions = JsonSerializer.Deserialize<List<FootnoteRegion>>(footnoteRegionJson);
                builder.HasData(footnoteRegions);
            }
        }
    }
}
