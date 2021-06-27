using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model.Entity
{
    public class FootnoteConfig : IEntityTypeConfiguration<Footnote>
    {
        public void Configure(EntityTypeBuilder<Footnote> builder)
        {
            builder.HasKey(f => f.FootnoteId);
            string seedFile = DbResources.footnoteJson;
            if (File.Exists(seedFile))
            {
                var footnoteJson = File.ReadAllText(seedFile);
                var footnotes = JsonSerializer.Deserialize<List<Footnote>>(footnoteJson);
                builder.HasData(footnotes);
            }
        }
    }
}
