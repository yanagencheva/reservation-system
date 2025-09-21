namespace Reservation.Persistence.Entities;

public class FailedReservation: Entity
{
    public DateTimeOffset DT { get; set; }
    public required string Raw_Request { get; set; }
}
