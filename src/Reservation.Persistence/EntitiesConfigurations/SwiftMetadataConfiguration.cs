using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reservation.Persistence.EntitiesConfigurations;

public class SwiftMetadataConfiguration : EntityConfiguration<Entities.SwiftMetadata>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Entities.SwiftMetadata> builder)
    {
        builder.ToTable("SwiftMetadata");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
          .ValueGeneratedOnAdd();
    }
}