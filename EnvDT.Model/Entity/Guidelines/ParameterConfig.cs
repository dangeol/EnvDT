using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model.Entity
{
    public class ParameterConfig : IEntityTypeConfiguration<Parameter>
    {
        public void Configure(EntityTypeBuilder<Parameter> builder)
        {
            builder.HasKey(p => p.ParameterId);
            var parameterJson = File.ReadAllText(DbResources.parameterJson);
            var parameters = JsonSerializer.Deserialize<List<Parameter>>(parameterJson);
            builder.HasData(parameters);
        }
    }
}
