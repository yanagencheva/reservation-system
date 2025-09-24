using Reservation.Common.Enums;

namespace Reservation.Common.Models;

public class ResultMessage
{
    public DateTimeOffset DT { get; set; }
    public required string Raw_Request { get; set; }
    public ValidationResults Validation_Result { get; set; }
}
