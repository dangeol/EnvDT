using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model.Entity
{
    public class UnitNameVariantConfig : IEntityTypeConfiguration<UnitNameVariant>
    {
        public void Configure(EntityTypeBuilder<UnitNameVariant> builder)
        {
            builder.HasKey(uv => uv.UnitNameVariantId);
            builder.HasOne(uv => uv.Unit)
                .WithMany(u => u.UnitNameVariants)
                .HasForeignKey(uv => uv.UnitId);
            string seedFile = DbResources.unitNameVariantJson;
            if (File.Exists(seedFile))
            {
                var unitNameVariantJson = File.ReadAllText(seedFile);
                var unitNameVariants = JsonSerializer.Deserialize<List<UnitNameVariant>>(unitNameVariantJson);
                builder.HasData(unitNameVariants);
            }
        }
    }
}

