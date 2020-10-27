using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model.Entity
{
    public class ParamNameVariantConfig : IEntityTypeConfiguration<ParamNameVariant>
    {
        public void Configure(EntityTypeBuilder<ParamNameVariant> builder)
        {
            builder.HasKey(pv => pv.ParamNameVariantId);
            builder.HasOne(pv => pv.Parameter)
                .WithMany(p => p.ParamNameVariants)
                .HasForeignKey(pv => pv.ParameterId);
            builder.HasOne(pv => pv.Language)
                .WithMany(p => p.ParamNameVariants)
                .HasForeignKey(pv => pv.LanguageId);
            var paramNameVariantJson = File.ReadAllText(DbResources.paramNameVariantJson);
            var paramNameVariants = JsonSerializer.Deserialize<List<ParamNameVariant>>(paramNameVariantJson);
            builder.HasData(paramNameVariants);
        }
    }
}

