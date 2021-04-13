using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model.Entity
{
    public class WasteCodeEWCConfig : IEntityTypeConfiguration<WasteCodeEWC>
    {
        public void Configure(EntityTypeBuilder<WasteCodeEWC> builder)
        {
            builder.HasKey(w => w.WasteCodeEWCId);
            var wasteCodeEWCJson = File.ReadAllText(DbResources.wasteCodeEWCJson);
            var wasteCodesEWC = JsonSerializer.Deserialize<List<WasteCodeEWC>>(wasteCodeEWCJson);
            builder.HasData(wasteCodesEWC);
        }
    }
}
