using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model.Entity
{
    public class ConfigXmlConfig : IEntityTypeConfiguration<ConfigXml>
    {
        public void Configure(EntityTypeBuilder<ConfigXml> builder)
        {
            builder.HasKey(c => c.ConfigXmlId);
            builder.HasOne(c => c.Laboratory)
                .WithOne(l => l.ConfigXml)
                .HasForeignKey<ConfigXml>(c => c.LaboratoryId);
            //var configXlsxJson = File.ReadAllText(DbResources.configXlsxJson);
            //var configXlsxs = JsonSerializer.Deserialize<List<ConfigXlsx>>(configXlsxJson);
            //builder.HasData(configXlsxs);
        }
    }
}
