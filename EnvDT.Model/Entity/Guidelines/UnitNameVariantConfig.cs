using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnvDT.Model.Entity
{
    public class UnitNameVariantConfig : IEntityTypeConfiguration<UnitNameVariant>
    {
        public void Configure(EntityTypeBuilder<UnitNameVariant> builder)
        {
            builder.HasKey(uv => uv.UnitNameVariantId);
            builder.HasOne(uv => uv.Unit)
                .WithMany(u => u.UnitNameVariants)
                .HasForeignKey(uv => uv.UnitId);
        }
    }
}

