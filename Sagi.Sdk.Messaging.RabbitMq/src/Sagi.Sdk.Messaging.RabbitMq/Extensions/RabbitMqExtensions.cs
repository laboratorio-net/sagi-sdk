
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using RabbitMQ.Client;

using Sagi.Sdk.Messaging.RabbitMq.Config;

namespace Sagi.Sdk.Messaging.RabbitMq.Extensions;

public static class RabbitMqExtensions
{
    private static CancellationTokenSource Source => new();
    private static CancellationToken Token => Source.Token;
    private static IConnection? Connection { get; set; }

    public static IServiceCollection AddRabbit(this IServiceCollection services,
        Action<RabbitOptions> configureOptions,
        Action<RabbitConfigurator> action)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configureOptions, nameof(configureOptions));

        var options = new RabbitOptions();
        configureOptions.Invoke(options);
        services.ConfigureFactory(options);

        var configurator = new RabbitConfigurator(Connection!, services, Token);
        action.Invoke(configurator);

        return services;
    }

    public static IServiceCollection AddRabbit(this IServiceCollection services,
        IConfiguration configuration,
        Action<RabbitConfigurator> action)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        var options = new RabbitOptions();
        configuration.Bind(options);
        services.ConfigureFactory(options);

        var configurator = new RabbitConfigurator(Connection!, services, Token);
        action.Invoke(configurator);

        return services;
    }

    private static void ConfigureFactory(this IServiceCollection services, RabbitOptions options)
    {
        var factory = new ConnectionFactory()
        {
            HostName = options.HostName ?? "localhost",
            VirtualHost = options.VirtualHost ?? ConnectionFactory.DefaultVHost,
            UserName = options.UserName ?? ConnectionFactory.DefaultUser,
            Password = options.Password ?? ConnectionFactory.DefaultPass,
            Port = options.Port,
        };

        Connection = factory.CreateConnectionAsync(Token).Result;

        services.AddSingleton(factory);
        services.AddSingleton<IConnection>(Connection);
        services.AddSingleton<IChannel>(provider =>
        {
            var connection = provider.GetRequiredService<IConnection>();
            return connection.CreateChannelAsync(cancellationToken: Token).Result;
        });
    }

    public static void Dispose()
    {
        Connection?.Dispose();
        Source.Cancel();
        Source.Dispose();
    }
}

