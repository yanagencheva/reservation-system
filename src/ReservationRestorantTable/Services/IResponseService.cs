using Reservation.Common.Models;

namespace ReservationRestorantTable.Services;

/// <summary>
/// Defines a contract for processing response messages.
/// </summary>
public interface IResponseService
{
    /// <summary>
    /// Processes the specified message, performing necessary
    /// operations such as parsing, validation, and persistence.
    /// </summary>
    /// <param name="message">
    /// The raw message content to be processed, typically in JSON format.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous processing operation.
    /// </returns>
    Task ProcessMessage(string message);
}