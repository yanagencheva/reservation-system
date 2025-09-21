namespace Reservation.Persistence.Entities;

public class Reservation: Entity
{
    public DateTimeOffset DT { get; set; }
    public required string Raw_Request { get; set; }
    public int Validation_Result { get; set; }
}
