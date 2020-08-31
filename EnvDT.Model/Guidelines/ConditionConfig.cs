using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model
{
    public class ConditionConfig : IEntityTypeConfiguration<Condition>
    {
        public void Configure(EntityTypeBuilder<Condition> builder)
        {
            builder.HasKey(c => c.ConditionId);
            var conditionJson = File.ReadAllText(DbResources.conditionJson);
            var conditions = JsonSerializer.Deserialize<List<Condition>>(conditionJson);
            builder.HasData(conditions);
        }
    }
}
