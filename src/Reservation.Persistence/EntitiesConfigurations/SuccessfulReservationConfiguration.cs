using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reservation.Persistence.EntitiesConfigurations;

public class SuccessfulReservationConfiguration : EntityConfiguration<Entities.SuccessfulReservation>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Entities.SuccessfulReservation> builder)
    {
        builder.ToTable("SuccessfulReservations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
          .ValueGeneratedOnAdd();
    }
}
