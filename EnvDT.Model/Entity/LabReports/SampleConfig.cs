using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnvDT.Model.Entity
{
    public class SampleConfig : IEntityTypeConfiguration<Sample>
    {
        public void Configure(EntityTypeBuilder<Sample> builder)
        {
            builder.HasKey(s => s.SampleId);
            builder.HasOne(sl => sl.LabReport)
                .WithMany(l => l.Samples)
                .HasForeignKey(sl => sl.LabReportId);
        }
    }
}