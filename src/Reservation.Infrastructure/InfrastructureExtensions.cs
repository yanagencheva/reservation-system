using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reservation.Initializer.RabbitMq;

namespace Reservation.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMQSettings>(options => configuration.GetSection("RabbitMq").Bind(options));

        services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<RabbitMQSettings>>().Value;
            var logger = sp.GetRequiredService<ILogger<RabbitMQConnection>>();
            return new RabbitMQConnection(settings, logger);
        });
        services.AddSingleton<IRabbitMqConsumerService, RabbitMqConsumerService>();

        return services;
    }
}
