using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

using Sagi.Sdk.MongoDb.Options;

namespace Sagi.Sdk.MongoDb.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddMongo(this IServiceCollection services, IConfiguration configuration)
    {
        var options = new MongoOptions();
        configuration.GetSection(MongoOptions.SectionName).Bind(options);
        ThrowIfNull(options);

        services.ConfigureMongoDatabase(options);
        ConfigureConvention();
        return services;
    }

    public static IServiceCollection AddMongo(this IServiceCollection services, Action<MongoOptions> action)
    {
        var options = new MongoOptions();
        action.Invoke(options);
        ThrowIfNull(options);

        services.ConfigureMongoDatabase(options);
        ConfigureConvention();
        return services;
    }

    private static void ConfigureConvention()
    {
        var conventionPack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new EnumRepresentationConvention(BsonType.String)
        };

        ConventionRegistry.Register("camelCase", conventionPack, t => true);
        ConventionRegistry.Register("EnumStringConvention", conventionPack, t => true);
    }
 
    private static void ConfigureMongoDatabase(this IServiceCollection services, MongoOptions options)
    {
        var mongoClient = new MongoClient(options.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(options.DatabaseName);
        services.AddSingleton(mongoDatabase);
    }

    private static void ThrowIfNull(MongoOptions options)
    {
        ArgumentNullException.ThrowIfNull(options.ConnectionString, nameof(options.ConnectionString));
        ArgumentNullException.ThrowIfNull(options.DatabaseName, nameof(options.DatabaseName));
    }
}