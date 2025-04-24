using Microsoft.Extensions.DependencyInjection;

using Sagi.Sdk.Messaging.RabbitMq.Config;

namespace Sagi.Sdk.Messaging.RabbitMq.Extensions;

public static class RabbitMqExtensions
{
    private static CancellationTokenSource Source => new();

    public static IServiceCollection AddRabbit(this IServiceCollection services,
        Action<RabbitConfigurator> configure, CancellationToken? token = null)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        var configurator = new RabbitConfigurator(services, token ?? Source.Token);
        configure(configurator);
        return services;
    }

    public static void Dispose()
    {
        Source.Cancel();
        Source.Dispose();
    }
}