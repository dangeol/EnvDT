using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model
{
    public class ParameterConfig : IEntityTypeConfiguration<Parameter>
    {
        public void Configure(EntityTypeBuilder<Parameter> builder)
        {
            builder.HasKey(p => p.ParameterId);
            var parameterJson = File.ReadAllText(Resources.parameterJson);
            var parameters = JsonSerializer.Deserialize<List<Parameter>>(parameterJson);
            builder.HasData(parameters);
        }
    }
}
