using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model.Entity
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
            var publCountryJson = File.ReadAllText(DbResources.publCountryJson);
            var publCountries = JsonSerializer.Deserialize<List<PublCountry>>(publCountryJson);
            builder.HasData(publCountries);
        }
    }
}
