using Ideageek.FightersArena.Api;
using Ideageek.FightersArena.Core.Services;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlKata.Compilers;
using SqlKata.Execution;
using Xunit;

namespace Ideageek.FightersArena.Tests;

public class DiSmokeTests
{
    private ServiceProvider BuildProvider()
    {
        var configValues = new Dictionary<string, string?>
        {
            ["ConnectionStrings:Default"] = "Server=localhost;Database=Ideageek.FightersArena;Trusted_Connection=True;TrustServerCertificate=True",
            ["Jwt:Issuer"] = "Ideageek.FightersArena",
            ["Jwt:Audience"] = "Ideageek.FightersArena.Api",
            ["Jwt:Key"] = "TESTING_KEY_SHOULD_BE_REPLACED",
            ["Jwt:ExpiryMinutes"] = "60"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues!)
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddIdeageekFightersArenaApi(configuration);
        services.AddAuthentication();
        return services.BuildServiceProvider();
    }

    [Fact]
    public void QueryFactory_Resolves()
    {
        using var provider = BuildProvider();
        var factory = provider.GetService<QueryFactory>();
        var compiler = provider.GetService<Compiler>();

        Assert.NotNull(factory);
        Assert.NotNull(compiler);
        var compiled = compiler!.Compile(new SqlKata.Query("Players").Where("GamerTag", "=", "test"));
        Assert.Contains("Players", compiled.Sql);
    }

    [Fact]
    public void Services_Resolve()
    {
        using var provider = BuildProvider();
        var tournamentService = provider.GetService<ITournamentService>();
        var authService = provider.GetService<IAuthService>();

        Assert.NotNull(tournamentService);
        Assert.NotNull(authService);
    }
}
