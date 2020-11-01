using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnvDT.Model.Entity
{
    public class SampleValueConfig : IEntityTypeConfiguration<SampleValue>
    {
        public void Configure(EntityTypeBuilder<SampleValue> builder)
        {
            builder.HasKey(s => s.SampleValueId);
            builder.HasOne(sv => sv.Sample)
                .WithMany(s => s.SampleValues)
                .HasForeignKey(sv => sv.SampleId);
            builder.HasOne(sv => sv.LabReportParam)
                .WithMany(lp => lp.SampleValues)
                .HasForeignKey(sv => sv.LabReportParamId);
        }
    }
}