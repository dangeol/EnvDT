using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnvDT.Model.Entity
{
    public class PublicationConfig : IEntityTypeConfiguration<Publication>
    {
        public void Configure(EntityTypeBuilder<Publication> builder)
        {
            builder.HasKey(p => p.PublicationId);
            string seedFile = DbResources.publicationJson;
            if (File.Exists(seedFile))
            {
                var publicationJson = File.ReadAllText(DbResources.publicationJson);
                var publications = JsonSerializer.Deserialize<List<Publication>>(publicationJson);
                builder.HasData(publications);
            }
        }
    }
}
