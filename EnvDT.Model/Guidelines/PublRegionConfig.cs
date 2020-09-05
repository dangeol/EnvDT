﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model
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
            var publRegionJson = File.ReadAllText(DbResources.publRegionJson);
            var publRegions = JsonSerializer.Deserialize<List<PublRegion>>(publRegionJson);
            builder.HasData(publRegions);
        }
    }
}