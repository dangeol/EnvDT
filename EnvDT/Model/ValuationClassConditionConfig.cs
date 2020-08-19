using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model
{
    public class ValuationClassConditionConfig : IEntityTypeConfiguration<ValuationClassCondition>
    {
        public void Configure(EntityTypeBuilder<ValuationClassCondition> builder)
        {
            builder.HasKey(c => new { c.ValuationClassId, c.ConditionId });
            builder.HasOne(vc => vc.ValuationClass)
                .WithMany(v => v.ValuationClassConditions)
                .HasForeignKey(vc => vc.ValuationClassId);
            builder.HasOne(vc => vc.Condition)
                .WithMany(v => v.ValuationClassConditions)
                .HasForeignKey(vc => vc.ConditionId);
            var valuationClassConditionJson = File.ReadAllText(Resources.valuationClassConditionJson);
            var valuationClassConditions = JsonSerializer.Deserialize<List<ValuationClassCondition>>(valuationClassConditionJson);
            builder.HasData(valuationClassConditions);
        }
    }
}