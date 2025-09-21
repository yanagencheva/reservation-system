using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Reservation.Persistence;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
       IConfiguration configuration)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString(nameof(ReservationDbContext)));
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<ReservationDbContext>(options =>
            options.UseNpgsql(configuration
                .GetConnectionString(nameof(ReservationDbContext))))
            .AddScoped<IDataStore, SqlDataStore>();

        return services;
    }
}
