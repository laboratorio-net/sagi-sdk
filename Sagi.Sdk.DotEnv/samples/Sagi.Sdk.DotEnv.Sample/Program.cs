using Microsoft.Extensions.Configuration;
using Sagi.Sdk.DotEnv.Extensions;

// Demonstração 1: AddDotEnv() sem argumentos — busca .env no diretório atual
IConfiguration config1 = new ConfigurationBuilder()
    .AddDotEnv()
    .Build();

Console.WriteLine("=== AddDotEnv() sem argumentos ===");
Console.WriteLine($"DATABASE_URL : {config1["DATABASE_URL"]}");
Console.WriteLine($"APP_NAME     : {config1["APP_NAME"]}");
Console.WriteLine($"DEBUG        : {config1["DEBUG"]}");
Console.WriteLine($"APP_SECRET   : {config1["APP_SECRET"]}");

Console.WriteLine();

// Demonstração 2: AddDotEnv(Action<DotEnvOptions>) com opções explícitas
IConfiguration config2 = new ConfigurationBuilder()
    .AddDotEnv(opt =>
    {
        opt.FileName = ".env";
    })
    .Build();

Console.WriteLine("=== AddDotEnv(opt => ...) com opções explícitas ===");
Console.WriteLine($"DATABASE_URL : {config2["DATABASE_URL"]}");
Console.WriteLine($"APP_NAME     : {config2["APP_NAME"]}");
Console.WriteLine($"DEBUG        : {config2["DEBUG"]}");
Console.WriteLine($"APP_SECRET   : {config2["APP_SECRET"]}");

Console.WriteLine();

// Demonstração 3: Sobreescrevendo appsettings
IConfiguration config3 = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddDotEnv()
    .Build();

Console.WriteLine("=== Overwriting AppSettings ===");
Console.WriteLine($"MyConfig : {config3["MyConfig"]}");
Console.WriteLine($"MyOtherConfig : {config3["MyOtherConfig"]}");