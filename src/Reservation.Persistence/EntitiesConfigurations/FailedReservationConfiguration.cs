using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reservation.Persistence.EntitiesConfigurations;

public class FailedReservationConfiguration : EntityConfiguration<Entities.FailedReservation>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Entities.FailedReservation> builder)
    {
        builder.ToTable("FailedReservations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
          .ValueGeneratedOnAdd();
    }
}