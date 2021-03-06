﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model.Entity
{
    public class LaboratoryConfig : IEntityTypeConfiguration<Laboratory>
    {
        public void Configure(EntityTypeBuilder<Laboratory> builder)
        {
            builder.HasKey(r => r.LaboratoryId);
            builder.HasOne(lc => lc.Country)
                .WithMany(p => p.Laboratories)
                .HasForeignKey(lc => lc.CountryId);
            string seedFile = DbResources.laboratoryJson;
            if (File.Exists(seedFile))
            {
                var laboratoryJson = File.ReadAllText(seedFile);
                var laboratories = JsonSerializer.Deserialize<List<Laboratory>>(laboratoryJson);
                builder.HasData(laboratories);
            }
        }
    }
}
