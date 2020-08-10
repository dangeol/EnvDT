using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnvDT.Model
{
    public class RegionConfig : IEntityTypeConfiguration<Region>
    {
        public void Configure(EntityTypeBuilder<Region> builder)
        {
            builder.HasKey(r => r.RegionId);
            builder.HasOne(pc => pc.Country)
                .WithMany(p => p.Regions)
                .HasForeignKey(pc => pc.CountryId);
        }
    }
}