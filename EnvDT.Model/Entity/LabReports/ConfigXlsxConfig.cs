using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model.Entity
{
    public class ConfigXlsxConfig : IEntityTypeConfiguration<ConfigXlsx>
    {
        public void Configure(EntityTypeBuilder<ConfigXlsx> builder)
        {
            builder.HasKey(c => c.ConfigXlsxId);
            builder.HasOne(c => c.Laboratory)
                .WithOne(l => l.ConfigXlsx)
                .HasForeignKey<ConfigXlsx>(c => c.LaboratoryId);
            string seedFile = DbResources.configXlsxJson;
            if (File.Exists(seedFile))
            {
                var configXlsxJson = File.ReadAllText(seedFile);
                var configXlsxs = JsonSerializer.Deserialize<List<ConfigXlsx>>(configXlsxJson);
                builder.HasData(configXlsxs);
            }
        }
    }
}
