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
            var mediumSubTypeJson = File.ReadAllText(DbResources.mediumSubTypeJson);
            var mediumSubTypes = JsonSerializer.Deserialize<List<MediumSubType>>(mediumSubTypeJson);
            builder.HasData(mediumSubTypes);
        }
    }
}

