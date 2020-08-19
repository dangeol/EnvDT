using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model
{
    public class UnitConfig : IEntityTypeConfiguration<Unit>
    {
        public void Configure(EntityTypeBuilder<Unit> builder)
        {
            builder.HasKey(u => u.UnitId);
            var unitJson = File.ReadAllText(Resources.unitJson);
            var units = JsonSerializer.Deserialize<List<Unit>>(unitJson);
            builder.HasData(units);
        }
    }
}
