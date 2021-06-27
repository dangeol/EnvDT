using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model.Entity
{
    public class ConditionConfig : IEntityTypeConfiguration<Condition>
    {
        public void Configure(EntityTypeBuilder<Condition> builder)
        {
            builder.HasKey(c => c.ConditionId);
            string seedFile = DbResources.conditionJson;
            if (File.Exists(seedFile))
            {
                var conditionJson = File.ReadAllText(seedFile);
                var conditions = JsonSerializer.Deserialize<List<Condition>>(conditionJson);
                builder.HasData(conditions);
            }            
        }
    }
}
