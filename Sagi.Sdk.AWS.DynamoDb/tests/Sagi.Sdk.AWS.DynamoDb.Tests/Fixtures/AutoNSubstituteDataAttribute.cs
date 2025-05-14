using Amazon.DynamoDBv2.Model;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using Bogus;
using Sagi.Sdk.AWS.DynamoDb.Config;
using Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures.Fakes;

namespace Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class AutoNSubstituteDataAttribute : AutoDataAttribute
{
    private readonly static Faker Faker = new();

    public AutoNSubstituteDataAttribute() : base(FixtureFactory) { }

    public static IFixture FixtureFactory()
    {
        var fixture = new Fixture()
            .Customize(new AutoNSubstituteCustomization { ConfigureMembers = true })
            .Customize(new DynamoModelCustomization(Faker))
           ;

        fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));
        fixture.RepeatCount = 1;

        fixture.Register(() => Faker);
        return fixture;
    }

    internal abstract class Customization(Faker faker) : ICustomization
    {
        public Faker Faker { get; } = faker;
        public abstract void Customize(IFixture fixture);
    }

    internal class DynamoModelCustomization(Faker faker) : Customization(faker)
    {
        public override void Customize(IFixture fixture)
        {
            fixture.Register<ScanResponse>(() => new()
            {
                Items =
                [
                    new() { { "Id", new AttributeValue { S = Faker.Lorem.Word() } } },
                    new() { { "Id", new AttributeValue { S = Faker.Lorem.Word() } } }
                ],
                LastEvaluatedKey = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { S = Faker.Lorem.Word() } }
                }
            });

            fixture.Register<CreateTableRequest>(() => new()
            {
                TableName = FakeModel.TABLE_NAME,
            });

            fixture.Register<DeleteItemRequest>(() => new()
            {
                TableName = Faker.Random.Word(),
                Key = new() {
                    { "Id", new () { S = Faker.Random.Guid().ToString() } }
                },
            });
        }
    }

    internal class ConfiguratorsCustomizations(Faker faker) : Customization(faker)
    {
        public override void Customize(IFixture fixture)
        {
            fixture.Register<DynamoDbConfigurator>(() => new()
            {
                Accesskey = Faker.Internet.UserName(),
                SecretKey = Faker.Internet.Password(),
                ServiceURL = Faker.Internet.Url(),
                InitializeDb = false,
            });            
        }
    }
}