using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnvDT.Model
{
    public class PublCountryConfig : IEntityTypeConfiguration<PublCountry>
    {
        public void Configure(EntityTypeBuilder<PublCountry> builder)
        {
            builder.HasKey(c => new { c.PublicationId, c.CountryId });
            builder.HasOne(pc => pc.Publication)
                .WithMany(p => p.PublCountries)
                .HasForeignKey(pc => pc.PublicationId);
            builder.HasOne(pc => pc.Country)
                .WithMany(p => p.PublCountries)
                .HasForeignKey(pc => pc.CountryId);
        }
    }
}
