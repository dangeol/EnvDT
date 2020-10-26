using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model.Entity
{
    public class PublParamConfig : IEntityTypeConfiguration<PublParam>
    {
        public void Configure(EntityTypeBuilder<PublParam> builder)
        {
            builder.HasKey(p => p.PublParamId);
            builder.HasOne(p => p.Publication)
                .WithMany(p => p.PublParams)
                .HasForeignKey(p => p.PublicationId);
            builder.HasOne(p => p.Parameter)
                .WithMany(p => p.PublParams)
                .HasForeignKey(p => p.ParameterId);
            builder.HasOne(p => p.Unit)
                .WithMany(p => p.PublParams)
                .HasForeignKey(p => p.UnitId);
            builder.HasOne(p => p.Medium)
                .WithMany(p => p.PublParams)
                .HasForeignKey(p => p.MediumId);
            var publParamJson = File.ReadAllText(DbResources.publParamJson);
            var publParams = JsonSerializer.Deserialize<List<PublParam>>(publParamJson);
            builder.HasData(publParams);
        }
    }
}