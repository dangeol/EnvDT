using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnvDT.Model
{
    public class SampleValueConfig : IEntityTypeConfiguration<SampleValue>
    {
        public void Configure(EntityTypeBuilder<SampleValue> builder)
        {
            builder.HasKey(s => s.SampleValueId);
            builder.HasOne(ss => ss.Sample)
                .WithMany(s => s.SampleValues)
                .HasForeignKey(ss => ss.SampleId);
            builder.HasOne(sp => sp.Parameter)
                .WithMany(p => p.SampleValues)
                .HasForeignKey(sp => sp.ParameterId);
            builder.HasOne(su => su.Unit)
                .WithMany(u => u.SampleValues)
                .HasForeignKey(su => su.UnitId);
        }
    }
}