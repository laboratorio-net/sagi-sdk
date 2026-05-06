namespace Sagi.Sdk.DotEnv.Options;

public class DotEnvOptions
{
    public string Directory { get; set; } = System.IO.Directory.GetCurrentDirectory();
    public string FileName { get; set; } = ".env";
}
