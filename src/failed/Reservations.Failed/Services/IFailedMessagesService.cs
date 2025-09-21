using Reservation.Persistence.Entities;

namespace Reservations.Failed.Services;

public interface IFailedMessagesService
{
    Task<FailedReservation> AddMessage(string message);
}
