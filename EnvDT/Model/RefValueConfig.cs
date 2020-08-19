using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model
{
    public class RefValueConfig : IEntityTypeConfiguration<RefValue>
    {
        public void Configure(EntityTypeBuilder<RefValue> builder)
        {
            builder.HasKey(r => r.RefValueId);
            builder.HasOne(r => r.Publication)
                .WithMany(r => r.RefValues)
                .HasForeignKey(r => r.PublicationId);
            builder.HasOne(r => r.Parameter)
                .WithMany(r => r.RefValues)
                .HasForeignKey(r => r.ParameterId);
            builder.HasOne(r => r.Unit)
                .WithMany(r => r.RefValues)
                .HasForeignKey(r => r.UnitId);
            builder.HasOne(r => r.ValuationClass)
                .WithMany(r => r.RefValues)
                .HasForeignKey(r => r.ValuationClassId);
            builder.HasOne(r => r.Medium)
                .WithMany(r => r.RefValues)
                .HasForeignKey(r => r.MediumId);
            var refValueJson = File.ReadAllText(Resources.refValueJson);
            var refValues = JsonSerializer.Deserialize<List<RefValue>>(refValueJson);
            builder.HasData(refValues);
        }
    }
}