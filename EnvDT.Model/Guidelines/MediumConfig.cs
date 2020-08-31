using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model
{
    public class MediumConfig : IEntityTypeConfiguration<Medium>
    {
        public void Configure(EntityTypeBuilder<Medium> builder)
        {
            builder.HasKey(m => m.MediumId);
            var mediumJson = File.ReadAllText(DbResources.mediumJson);
            var media = JsonSerializer.Deserialize<List<Medium>>(mediumJson);
            builder.HasData(media);
        }
    }
}
