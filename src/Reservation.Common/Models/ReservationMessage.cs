using Reservation.Common.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Reservation.Common.Models;

public class ReservationMessage
{
    [Required]
    public string ClientName { get; set; } = default!;

    [Required]
    [Phone]
    public string ClientTelephone { get; set; } = default!;

    [Range(1, 100)]
    public int NumberOfReservedTable { get; set; }

    [Required]
    [JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime DateOfReservation { get; set; }
}
