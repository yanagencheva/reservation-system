using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Npgsql;
using NpgsqlTypes;

namespace Reservation.Persistence;

public class SqlDataStore(ReservationDbContext dbContext) : IDataStore
{
    private readonly ReservationDbContext _dbContext = dbContext;

    public T Add<T>(T entity) where T : class
    {
        var created = _dbContext.Add(entity);
        return created.Entity;
    }

    public async Task<T?> GetSingleOrDefaultBy<T>(
        Expression<Func<T, bool>> filter,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null!,
        bool tracked = false) where T : class
    {
        if (filter == null)
        {
            return default;
        }

        IQueryable<T> query = All<T>(tracked);

        if (include != null)
        {
            query = include(query);
        }

        var result = await query.SingleOrDefaultAsync(filter);
        return result;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync<T>(T entity) where T : class
    {
        _dbContext.Set<T>().Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<int> ExecuteStoredProcedureNonQueryAsync(string procedureName, params object[] parameters)
    {
        var placeholders = string.Join(", ", parameters.Select((_, i) => $"{{{i}}}"));
        var sql = $"CALL {procedureName}({placeholders})";

        return await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters);
    }

    private IQueryable<T> All<T>(bool tracked = false) where T : class
    {
        IQueryable<T> query = _dbContext.Set<T>();

        if (!tracked)
        {
            query = query.AsNoTracking();
        }

        return query;
    }
}