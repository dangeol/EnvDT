using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model.Entity
{
    public class LanguageConfig : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.HasKey(r => r.LanguageId);
            var languageJson = File.ReadAllText(DbResources.languageJson);
            var languages = JsonSerializer.Deserialize<List<Language>>(languageJson);
            builder.HasData(languages);
        }
    }
}
