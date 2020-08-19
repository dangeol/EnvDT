using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model
{
    public class ParameterLaboratoryConfig : IEntityTypeConfiguration<ParameterLaboratory>
    {
        public void Configure(EntityTypeBuilder<ParameterLaboratory> builder)
        {
            builder.HasKey(r => new { r.ParameterId, r.LaboratoryId });
            builder.HasOne(pl => pl.Parameter)
                .WithMany(p => p.ParameterLaboratories)
                .HasForeignKey(pl => pl.ParameterId);
            builder.HasOne(pl => pl.Laboratory)
                .WithMany(p => p.ParameterLaboratories)
                .HasForeignKey(pl => pl.LaboratoryId);
            var parameterLaboratoryJson = File.ReadAllText(Resources.parameterLaboratoryJson);
            var parameterLaboratories = JsonSerializer.Deserialize<List<ParameterLaboratory>>(parameterLaboratoryJson);
            builder.HasData(parameterLaboratories);
        }
    }
}

