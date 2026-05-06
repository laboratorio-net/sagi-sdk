using Microsoft.Extensions.Configuration;

using Sagi.Sdk.DotEnv.Extensions;

// Demonstração 1: AddDotEnv() sem argumentos — busca .env no diretório atual
IConfiguration config1 = new ConfigurationBuilder()
    .AddDotEnv(opt => opt.Directory = AppContext.BaseDirectory)
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
        opt.Directory = AppContext.BaseDirectory;
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
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .AddDotEnv(opt => opt.Directory = AppContext.BaseDirectory)
    .Build();

Console.WriteLine("=== Overwriting AppSettings ===");
Console.WriteLine($"MyConfig : {config3["MyConfig"]}");
Console.WriteLine($"MyOtherConfig : {config3["MyOtherConfig"]}");

Console.WriteLine();

// Demonstração 4: Fazendo bind com Objeto complexo
IConfiguration config4 = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .AddDotEnv()
    .Build();

var obj = config4.GetSection("ObjConfig").Get<Obj1>();

Console.WriteLine("=== Binding with Complex Objects ===");
Console.WriteLine($"obj?.Prop1 : {obj?.Prop1}");
Console.WriteLine($"obj?.Obj2.Prop2 : {obj?.Obj2.Prop2}");


class Obj1
{
    public string Prop1 { get; set; } = "";
    public Obj2 Obj2 { get; set; } = new();
}

class Obj2
{
    public string Prop2 { get; set; } = "";
}