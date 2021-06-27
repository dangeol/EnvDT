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
            string seedFile = DbResources.wasteCodeEWCJson;
            if (File.Exists(seedFile))
            {
                var wasteCodeEWCJson = File.ReadAllText(seedFile);
                var wasteCodesEWC = JsonSerializer.Deserialize<List<WasteCodeEWC>>(wasteCodeEWCJson);
                builder.HasData(wasteCodesEWC);
            }
        }
    }
}
