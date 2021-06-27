using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model.Entity
{
    public class FootnoteParamConfig : IEntityTypeConfiguration<FootnoteParam>
    {
        public void Configure(EntityTypeBuilder<FootnoteParam> builder)
        {
            builder.HasKey(fp => fp.FootnoteParamId);
            builder.HasOne(fp => fp.PublParam)
                .WithMany(fp => fp.FootnoteParams)
                .HasForeignKey(fp => fp.PublParamId);
            builder.HasOne(fp => fp.Footnote)
                .WithMany(fp => fp.FootnoteParams)
                .HasForeignKey(fp => fp.FootnoteId);
            string seedFile = DbResources.footnoteParamJson;
            if (File.Exists(seedFile))
            {
                var FootnoteParamJson = File.ReadAllText(seedFile);
                var FootnoteParams = JsonSerializer.Deserialize<List<FootnoteParam>>(FootnoteParamJson);
                builder.HasData(FootnoteParams);
            }
        }
    }
}