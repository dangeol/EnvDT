using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model.Entity
{
    public class MediumSubTypeConfig : IEntityTypeConfiguration<MediumSubType>
    {
        public void Configure(EntityTypeBuilder<MediumSubType> builder)
        {
            builder.HasKey(m => m.MedSubTypeId);
            string seedFile = DbResources.mediumSubTypeJson;
            if (File.Exists(seedFile))
            {
                var mediumSubTypeJson = File.ReadAllText(seedFile);
                var mediumSubTypes = JsonSerializer.Deserialize<List<MediumSubType>>(mediumSubTypeJson);
                builder.HasData(mediumSubTypes);
            }
        }
    }
}

