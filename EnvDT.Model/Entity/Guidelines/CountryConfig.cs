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
            string seedFile = DbResources.countryJson;
            if (File.Exists(seedFile))
            {
                var countryJson = File.ReadAllText(seedFile);
                var countries = JsonSerializer.Deserialize<List<Country>>(countryJson);
                builder.HasData(countries);
            }
        }
    }
}
