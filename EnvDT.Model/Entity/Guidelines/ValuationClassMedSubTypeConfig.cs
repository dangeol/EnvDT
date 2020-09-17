using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model.Entity
{
    public class ValuationClassMedSubTypeConfig : IEntityTypeConfiguration<ValuationClassMedSubType>
    {
        public void Configure(EntityTypeBuilder<ValuationClassMedSubType> builder)
        {
            builder.HasKey(m => new { m.ValuationClassId, m.MedSubTypeId });
            builder.HasOne(vm => vm.ValuationClass)
                .WithMany(v => v.ValuationClassMedSubTypes)
                .HasForeignKey(vm => vm.ValuationClassId);
            builder.HasOne(vm => vm.MediumSubType)
                .WithMany(v => v.ValuationClassMedSubTypes)
                .HasForeignKey(vm => vm.MedSubTypeId);
            var valuationClassMedSubTypeJson = File.ReadAllText(DbResources.valuationClassMedSubTypeJson);
            var valuationClassMedSubTypes = JsonSerializer.Deserialize<List<ValuationClassMedSubType>>(valuationClassMedSubTypeJson);
            builder.HasData(valuationClassMedSubTypes);
        }
    }
}