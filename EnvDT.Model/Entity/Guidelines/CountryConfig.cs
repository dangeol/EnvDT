using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model.Entity
{
    public class CountryConfig : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasKey(c => c.CountryId);
            var countryJson = File.ReadAllText(DbResources.countryJson);
            var countries = JsonSerializer.Deserialize<List<Country>>(countryJson);
            builder.HasData(countries);
        }
    }
}
