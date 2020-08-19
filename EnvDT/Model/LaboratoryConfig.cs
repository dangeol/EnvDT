using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model
{
    public class LaboratoryConfig : IEntityTypeConfiguration<Laboratory>
    {
        public void Configure(EntityTypeBuilder<Laboratory> builder)
        {
            builder.HasKey(r => r.LaboratoryId);
            builder.HasOne(lc => lc.Country)
                .WithMany(p => p.Laboratories)
                .HasForeignKey(lc => lc.CountryId);
            var laboratoryJson = File.ReadAllText(Resources.laboratoryJson);
            var laboratories = JsonSerializer.Deserialize<List<Laboratory>>(laboratoryJson);
            builder.HasData(laboratories);
        }
    }
}
