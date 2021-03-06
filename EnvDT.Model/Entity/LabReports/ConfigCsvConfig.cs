﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model.Entity
{
    public class ConfigCsvConfig : IEntityTypeConfiguration<ConfigCsv>
    {
        public void Configure(EntityTypeBuilder<ConfigCsv> builder)
        {
            builder.HasKey(c => c.ConfigCsvId);
            builder.HasOne(c => c.Laboratory)
                .WithOne(l => l.ConfigCsv)
                .HasForeignKey<ConfigCsv>(c => c.LaboratoryId);
            string seedFile = DbResources.configCsvJson;
            if (File.Exists(seedFile))
            {
                var configCsvJson = File.ReadAllText(seedFile);
                var configCsvs = JsonSerializer.Deserialize<List<ConfigCsv>>(configCsvJson);
                builder.HasData(configCsvs);
            }
        }
    }
}
