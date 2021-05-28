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
            builder.HasOne(fp => fp.Publication)
                .WithMany(fp => fp.FootnoteParams)
                .HasForeignKey(fp => fp.PublicationId);
            builder.HasOne(fp => fp.Parameter)
                .WithMany(fp => fp.FootnoteParams)
                .HasForeignKey(fp => fp.ParameterId);
            builder.HasOne(fp => fp.Unit)
                .WithMany(fp => fp.FootnoteParams)
                .HasForeignKey(fp => fp.UnitId);
            builder.HasOne(fp => fp.Footnote)
                .WithMany(fp => fp.FootnoteParams)
                .HasForeignKey(fp => fp.FootnoteId);
            var FootnoteParamJson = File.ReadAllText(DbResources.footnoteParamJson);
            var FootnoteParams = JsonSerializer.Deserialize<List<FootnoteParam>>(FootnoteParamJson);
            builder.HasData(FootnoteParams);
        }
    }
}