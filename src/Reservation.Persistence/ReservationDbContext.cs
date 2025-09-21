using Microsoft.EntityFrameworkCore;

namespace Reservation.Persistence;

public class ReservationDbContext(
    DbContextOptions<ReservationDbContext> options
) : DbContext(options)
{
    public DbSet<Entities.Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}