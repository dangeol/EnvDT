using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model
{
    public class ValuationClassConfig : IEntityTypeConfiguration<ValuationClass>
    {
        public void Configure(EntityTypeBuilder<ValuationClass> builder)
        {
            builder.HasKey(v => v.ValuationClassId);
            builder.HasOne(vp => vp.Publication)
                .WithMany(p => p.ValuationClasses)
                .HasForeignKey(vp => vp.PublicationId);
            var valuationClassJson = File.ReadAllText(Resources.valuationClassJson);
            var valuationClasses = JsonSerializer.Deserialize<List<ValuationClass>>(valuationClassJson);
            builder.HasData(valuationClasses);
        }
    }
}
