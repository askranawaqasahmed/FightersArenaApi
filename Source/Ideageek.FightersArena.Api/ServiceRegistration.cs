using Ideageek.FightersArena.Core.Entities.Authorization;
using Ideageek.FightersArena.Core.Repositories;
using Ideageek.FightersArena.Core.Services;
using Ideageek.FightersArena.Services.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data;

namespace Ideageek.FightersArena.Api;

public static class ServiceRegistration
{
    public static IServiceCollection AddIdeageekFightersArenaApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<IDbConnection>(_ =>
        {
            var connectionString = configuration.GetConnectionString("Default") ??
                                   configuration.GetConnectionString("CAFASuiteConnection") ??
                                   throw new InvalidOperationException("Connection string not configured");
            return new SqlConnection(connectionString);
        });

        services.AddSingleton<Compiler, SqlServerCompiler>();
        services.AddScoped(sp =>
        {
            var db = sp.GetRequiredService<IDbConnection>();
            var compiler = sp.GetRequiredService<Compiler>();
            var factory = new QueryFactory(db, compiler)
            {
                Logger = sql => Console.WriteLine($"[SQL] {sql.Sql}")
            };
            return factory;
        });

        services.AddIdentityCore<AspNetUser>()
            .AddRoles<AspNetRole>()
            .AddUserStore<UserStore>()
            .AddRoleStore<RoleStore>()
            .AddSignInManager<SignInManager<AspNetUser>>()
            .AddRoleManager<RoleManager<AspNetRole>>()
            .AddDefaultTokenProviders();

        services.AddScoped<IUserStore<AspNetUser>, UserStore>();
        services.AddScoped<IRoleStore<AspNetRole>, RoleStore>();
        services.AddScoped<IPasswordHasher<AspNetUser>, PasswordHasher<AspNetUser>>();

        services.AddScoped<GameRepository>();
        services.AddScoped<SponsorRepository>();
        services.AddScoped<PlayerRepository>();
        services.AddScoped<TeamRepository>();
        services.AddScoped<SeasonRepository>();
        services.AddScoped<TournamentRepository>();
        services.AddScoped<TournamentStageRepository>();
        services.AddScoped<StageParticipantRepository>();
        services.AddScoped<MatchRepository>();
        services.AddScoped<MatchResultRepository>();
        services.AddScoped<PlacementRepository>();
        services.AddScoped<PointsRuleRepository>();
        services.AddScoped<PointsLedgerRepository>();
        services.AddScoped<LeagueRepository>();
        services.AddScoped<LeagueParticipantRepository>();
        services.AddScoped<LeagueMatchRepository>();
        services.AddScoped<LeagueMatchResultRepository>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<ISponsorService, SponsorService>();
        services.AddScoped<IPlayerService, PlayerService>();
        services.AddScoped<ITeamService, TeamService>();
        services.AddScoped<ISeasonService, SeasonService>();
        services.AddScoped<ITournamentService, TournamentService>();
        services.AddScoped<ILeaderboardService, LeaderboardService>();
        services.AddScoped<IHomeService, HomeService>();
        services.AddScoped<IPointsService, PointsService>();
        services.AddScoped<ILeagueService, LeagueService>();

        return services;
    }
}
