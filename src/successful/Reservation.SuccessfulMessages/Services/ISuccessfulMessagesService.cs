using Reservation.Common.Models;

namespace Reservation.SuccessfulMessages.Services;

public interface ISuccessfulMessagesService
{
    Task<ResultMessage?> AddMessage(string message);
}
