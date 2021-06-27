using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model.Entity
{
    public class UnitConfig : IEntityTypeConfiguration<Unit>
    {
        public void Configure(EntityTypeBuilder<Unit> builder)
        {
            builder.HasKey(u => u.UnitId);
            string seedFile = DbResources.unitJson;
            if (File.Exists(seedFile))
            {
                var unitJson = File.ReadAllText(seedFile);
                var units = JsonSerializer.Deserialize<List<Unit>>(unitJson);
                builder.HasData(units);
            }
        }
    }
}
