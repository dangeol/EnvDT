using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model.Entity
{
    public class MediumConfig : IEntityTypeConfiguration<Medium>
    {
        public void Configure(EntityTypeBuilder<Medium> builder)
        {
            builder.HasKey(m => m.MediumId);
            string seedFile = DbResources.mediumJson;
            if (File.Exists(seedFile))
            {
                var mediumJson = File.ReadAllText(seedFile);
                var media = JsonSerializer.Deserialize<List<Medium>>(mediumJson);
                builder.HasData(media);
            }
        }
    }
}
