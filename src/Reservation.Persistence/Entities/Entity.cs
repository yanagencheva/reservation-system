using System.ComponentModel.DataAnnotations;
namespace Reservation.Persistence.Entities;

public abstract class Entity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
}
