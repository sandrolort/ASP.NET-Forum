using Logging;
using Mapster;
using Microsoft.Extensions.Configuration;
using Services.Contracts.Contracts;

namespace Tests;

public class TestBase
{
    protected IConfiguration Configuration;
    protected ILoggerService Logger;

    protected TestBase()
    {
        Logger = new ConsoleLogger();

        var config = new Dictionary<string, string>
        {
            { "JwtSettings:Expires", "10" },
            { "JwtSettings:Issuer", "localhost" },
            { "JwtSettings:Audience", "localhost" },
            { "ForumConfiguration:MinCommentsRequired", "5"}
        };

        Configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(config)
            .Build();
        
        TypeAdapterConfig.GlobalSettings.Default.MapToConstructor(true);
    }
}