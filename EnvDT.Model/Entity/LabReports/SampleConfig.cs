using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnvDT.Model.Entity
{
    public class SampleConfig : IEntityTypeConfiguration<Sample>
    {
        public void Configure(EntityTypeBuilder<Sample> builder)
        {
            builder.HasKey(s => s.SampleId);
            builder.HasOne(s => s.LabReport)
                .WithMany(lr => lr.Samples)
                .HasForeignKey(s => s.LabReportId);
            builder.HasOne(s => s.Medium)
                .WithMany(m => m.Samples)
                .HasForeignKey(s => s.MediumId);
            builder.HasOne(s => s.MediumSubType)
                .WithMany(ms => ms.Samples)
                .HasForeignKey(s => s.MediumSubTypeId);
            builder.HasOne(s => s.Condition)
                .WithMany(c => c.Samples)
                .HasForeignKey(s => s.ConditionId);
            builder.HasOne(s => s.WasteCodeEWC)
                .WithMany(wc => wc.Samples)
                .HasForeignKey(s => s.WasteCodeEWCId);
        }
    }
}