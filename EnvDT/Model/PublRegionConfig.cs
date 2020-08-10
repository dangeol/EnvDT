using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnvDT.Model
{
    public class PublRegionConfig : IEntityTypeConfiguration<PublRegion>
    {
        public void Configure(EntityTypeBuilder<PublRegion> builder)
        {
            builder.HasKey(r => new { r.PublicationId, r.RegionId });
            builder.HasOne(pr => pr.Publication)
                .WithMany(p => p.PublRegions)
                .HasForeignKey(pr => pr.PublicationId);
            builder.HasOne(pr => pr.Region)
                .WithMany(p => p.PublRegions)
                .HasForeignKey(pr => pr.RegionId);
        }
    }
}
