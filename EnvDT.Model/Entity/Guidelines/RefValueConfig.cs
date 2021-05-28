using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model.Entity
{
    public class RefValueConfig : IEntityTypeConfiguration<RefValue>
    {
        public void Configure(EntityTypeBuilder<RefValue> builder)
        {
            builder.HasKey(r => r.RefValueId);
            builder.HasOne(r => r.PublParam)
                .WithMany(r => r.RefValues)
                .HasForeignKey(r => r.PublParamId);
            builder.HasOne(r => r.ValuationClass)
                .WithMany(r => r.RefValues)
                .HasForeignKey(r => r.ValuationClassId);
            builder.HasOne(r => r.Footnote)
                .WithMany(r => r.RefValues)
                .HasForeignKey(r => r.FootnoteId)
                .IsRequired(false);
            var refValueJson = File.ReadAllText(DbResources.refValueJson);
            var refValues = JsonSerializer.Deserialize<List<RefValue>>(refValueJson);
            builder.HasData(refValues);
        }
    }
}