using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnvDT.Model
{
    public class RefValueConfig : IEntityTypeConfiguration<RefValue>
    {
        public void Configure(EntityTypeBuilder<RefValue> builder)
        {
            builder.HasKey(r => r.RefValueId);
            builder.HasOne(r => r.RefValueName)
                .WithMany(r => r.RefValues)
                .HasForeignKey(r => r.RefValueNameId);
            builder.HasOne(r => r.Unit)
                .WithMany(r => r.RefValues)
                .HasForeignKey(r => r.UnitId);
            builder.HasOne(r => r.Medium)
                .WithMany(r => r.RefValues)
                .HasForeignKey(r => r.MediumId);
            builder.HasOne(r => r.Publication)
                .WithMany(r => r.RefValues)
                .HasForeignKey(r => r.PublicationId);
            builder.HasOne(r => r.Condition)
                .WithMany(r => r.RefValues)
                .HasForeignKey(r => r.ConditionId);
        }
    }
}