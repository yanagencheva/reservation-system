using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Reservation.Persistence;

public interface IDataStore
{
    /// <summary>
    /// Adds a new entity to the data store.
    /// </summary>
    /// <typeparam name="T">The type of the entity to add.</typeparam>
    /// <param name="entity">The entity to add to the data store.</param>
    /// <returns>The added entity with any generated values (e.g., ID).</returns>
    T Add<T>(T entity) where T : class;

    /// <summary>
    /// Retrieves a single entity or null based on the specified filter criteria.
    /// </summary>
    /// <typeparam name="T">The type of entity to retrieve.</typeparam>
    /// <param name="filter">The filter expression to find the specific entity.</param>
    /// <param name="include">Optional function to include related entities in the query.</param>
    /// <param name="tracked">Indicates whether the entity should be tracked by the context for change detection. Default is false.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity if found, otherwise null.</returns>
    /// <exception cref="InvalidOperationException">Thrown when more than one entity matches the filter criteria.</exception>
    Task<T?> GetSingleOrDefaultBy<T>(Expression<Func<T, bool>> filter,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        bool tracked = false) where T : class;

    /// <summary>
    /// Asynchronously saves all changes made in the current context to the underlying data store.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the data store.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously updates the given entity in the database.
    /// </summary>
    /// <typeparam name="T">The type of the entity. Must be a class.</typeparam>
    /// <param name="entity">The entity to be updated.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method marks the entity as <c>Modified</c> in the Entity Framework context 
    /// and then calls <see cref="DbContext.SaveChangesAsync"/> to persist the changes to the database.
    /// </remarks>
    /// <exception cref="DbUpdateException">Thrown if there is an error while saving changes to the database.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the entity is not properly tracked by the DbContext.</exception>
    Task UpdateAsync<T>(T entity) where T : class;

    Task<int> ExecuteStoredProcedureNonQueryAsync(string procedureName, params object[] parameters);
}