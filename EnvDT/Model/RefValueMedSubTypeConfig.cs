using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnvDT.Model
{
    public class RefValueMedSubTypeConfig : IEntityTypeConfiguration<RefValueMedSubType>
    {
        public void Configure(EntityTypeBuilder<RefValueMedSubType> builder)
        {
            builder.HasKey(m => new { m.RefValueId, m.MedSubTypeId });
            builder.HasOne(rm => rm.RefValue)
                .WithMany(r => r.RefValueMedSubTypes)
                .HasForeignKey(rm => rm.RefValueId);
            builder.HasOne(rm => rm.MediumSubType)
                .WithMany(r => r.RefValueMedSubTypes)
                .HasForeignKey(rm => rm.MedSubTypeId);
        }
    }
}