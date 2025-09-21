using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reservation.Persistence.EntitiesConfigurations;

public class ReservationConfiguration : EntityConfiguration<Entities.Reservation>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Entities.Reservation> builder)
    {
        builder.ToTable("Reservations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
          .ValueGeneratedOnAdd();

        #region Indexes
        builder.HasIndex(x => x.Id)
            .HasDatabaseName("IX_Reservation_Id");
        #endregion
    }
}