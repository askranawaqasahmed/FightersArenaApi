using System.Collections.Generic;
using System.Data;
using System.Text;
using Dapper;
using Ideageek.FightersArena.Core.Entities.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ideageek.FightersArena.Api.Migrations;

internal static class StartupMigrationRunner
{
    private record MigrationDefinition(string Id, string Sql);

    private static readonly MigrationDefinition[] Migrations = new[]
    {
        new MigrationDefinition(
            "20240101_initial_schema",
            @"
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Sponsors')
BEGIN
    CREATE TABLE Sponsors (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        LogoUrl NVARCHAR(500) NULL,
        WebsiteUrl NVARCHAR(500) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Games')
BEGIN
    CREATE TABLE Games (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        Slug NVARCHAR(200) NOT NULL UNIQUE,
        CoverUrl NVARCHAR(500) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Players')
BEGIN
    CREATE TABLE Players (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        UserId UNIQUEIDENTIFIER NOT NULL,
        GamerTag NVARCHAR(100) NOT NULL UNIQUE,
        DisplayName NVARCHAR(200) NOT NULL,
        SponsorId UNIQUEIDENTIFIER NULL,
        AvatarUrl NVARCHAR(500) NULL,
        Bio NVARCHAR(MAX) NULL,
        Country NVARCHAR(50) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PlayerGames')
BEGIN
    CREATE TABLE PlayerGames (
        PlayerId UNIQUEIDENTIFIER NOT NULL,
        GameId UNIQUEIDENTIFIER NOT NULL,
        CONSTRAINT PK_PlayerGames PRIMARY KEY (PlayerId, GameId)
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Teams')
BEGIN
    CREATE TABLE Teams (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL UNIQUE,
        Tag NVARCHAR(50) NULL,
        LogoUrl NVARCHAR(500) NULL,
        CaptainPlayerId UNIQUEIDENTIFIER NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TeamMembers')
BEGIN
    CREATE TABLE TeamMembers (
        TeamId UNIQUEIDENTIFIER NOT NULL,
        PlayerId UNIQUEIDENTIFIER NOT NULL,
        Role NVARCHAR(100) NOT NULL DEFAULT 'Player',
        JoinedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        LeftAt DATETIME2 NULL,
        CONSTRAINT PK_TeamMembers PRIMARY KEY (TeamId, PlayerId, JoinedAt)
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Seasons')
BEGIN
    CREATE TABLE Seasons (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        StartDate DATETIME2 NOT NULL,
        EndDate DATETIME2 NULL,
        IsActive BIT NOT NULL DEFAULT 0
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Tournaments')
BEGIN
    CREATE TABLE Tournaments (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        SeasonId UNIQUEIDENTIFIER NOT NULL,
        GameId UNIQUEIDENTIFIER NOT NULL,
        Name NVARCHAR(200) NOT NULL,
        Tier NVARCHAR(50) NOT NULL,
        BannerUrl NVARCHAR(500) NULL,
        StartDate DATETIME2 NOT NULL,
        EndDate DATETIME2 NULL,
        Status NVARCHAR(50) NOT NULL,
        RulesText NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TournamentStages')
BEGIN
    CREATE TABLE TournamentStages (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        TournamentId UNIQUEIDENTIFIER NOT NULL,
        StageOrder INT NOT NULL,
        Name NVARCHAR(200) NOT NULL,
        Format NVARCHAR(50) NOT NULL,
        ConfigJson NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'StageParticipants')
BEGIN
    CREATE TABLE StageParticipants (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        StageId UNIQUEIDENTIFIER NOT NULL,
        ParticipantType NVARCHAR(50) NOT NULL,
        ParticipantId UNIQUEIDENTIFIER NOT NULL,
        Seed INT NOT NULL,
        GroupId NVARCHAR(50) NULL
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Matches')
BEGIN
    CREATE TABLE Matches (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        StageId UNIQUEIDENTIFIER NOT NULL,
        RoundNumber INT NOT NULL,
        BracketType NVARCHAR(50) NULL,
        GroupId NVARCHAR(50) NULL,
        AId UNIQUEIDENTIFIER NOT NULL,
        BId UNIQUEIDENTIFIER NOT NULL,
        ScheduledAt DATETIME2 NULL,
        Status NVARCHAR(50) NOT NULL
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MatchResults')
BEGIN
    CREATE TABLE MatchResults (
        MatchId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        WinnerId UNIQUEIDENTIFIER NOT NULL,
        ScoreA INT NOT NULL,
        ScoreB INT NOT NULL,
        DetailsJson NVARCHAR(MAX) NULL
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Placements')
BEGIN
    CREATE TABLE Placements (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        TournamentId UNIQUEIDENTIFIER NOT NULL,
        ParticipantType NVARCHAR(50) NOT NULL,
        ParticipantId UNIQUEIDENTIFIER NOT NULL,
        PlacementNumber INT NOT NULL
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PointsRules')
BEGIN
    CREATE TABLE PointsRules (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Tier NVARCHAR(50) NOT NULL,
        PlacementNumber INT NOT NULL,
        Points INT NOT NULL
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PointsLedger')
BEGIN
    CREATE TABLE PointsLedger (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        SeasonId UNIQUEIDENTIFIER NOT NULL,
        TournamentId UNIQUEIDENTIFIER NOT NULL,
        ParticipantType NVARCHAR(50) NOT NULL,
        ParticipantId UNIQUEIDENTIFIER NOT NULL,
        Points INT NOT NULL,
        Reason NVARCHAR(500) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUsers')
BEGIN
    CREATE TABLE AspNetUsers (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        FullName NVARCHAR(200) NULL,
        UserName NVARCHAR(256) NULL,
        NormalizedUserName NVARCHAR(256) NULL,
        PasswordHash NVARCHAR(MAX) NULL,
        Email NVARCHAR(256) NULL,
        NormalizedEmail NVARCHAR(256) NULL,
        EmailConfirmed BIT NOT NULL DEFAULT 0
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetRoles')
BEGIN
    CREATE TABLE AspNetRoles (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Name NVARCHAR(256) NULL,
        NormalizedName NVARCHAR(256) NULL
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUserRoles')
BEGIN
    CREATE TABLE AspNetUserRoles (
        UserId UNIQUEIDENTIFIER NOT NULL,
        RoleId UNIQUEIDENTIFIER NOT NULL,
        CONSTRAINT PK_AspNetUserRoles PRIMARY KEY (UserId, RoleId)
    );
END;
")
        ,
        new MigrationDefinition(
            "20260113_seed_roles",
            @"
INSERT INTO AspNetRoles (Id, Name, NormalizedName)
SELECT NEWID(), 'SuperAdmin', 'SUPERADMIN'
WHERE NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'SUPERADMIN');

INSERT INTO AspNetRoles (Id, Name, NormalizedName)
SELECT NEWID(), 'Admin', 'ADMIN'
WHERE NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'ADMIN');

INSERT INTO AspNetRoles (Id, Name, NormalizedName)
SELECT NEWID(), 'Organizer', 'ORGANIZER'
WHERE NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'ORGANIZER');

INSERT INTO AspNetRoles (Id, Name, NormalizedName)
SELECT NEWID(), 'Player', 'PLAYER'
WHERE NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'PLAYER');
")
        ,
        new MigrationDefinition(
            "20260113_leagues_schema",
            @"
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Leagues')
BEGIN
    CREATE TABLE Leagues (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        SeasonId UNIQUEIDENTIFIER NOT NULL,
        GameId UNIQUEIDENTIFIER NOT NULL,
        Name NVARCHAR(200) NOT NULL,
        StartDate DATETIME2 NOT NULL,
        EndDate DATETIME2 NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Draft',
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LeagueParticipants')
BEGIN
    CREATE TABLE LeagueParticipants (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        LeagueId UNIQUEIDENTIFIER NOT NULL,
        ParticipantType NVARCHAR(50) NOT NULL,
        ParticipantId UNIQUEIDENTIFIER NOT NULL,
        Seed INT NOT NULL,
        GroupId NVARCHAR(50) NULL
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LeagueMatches')
BEGIN
    CREATE TABLE LeagueMatches (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        LeagueId UNIQUEIDENTIFIER NOT NULL,
        RoundNumber INT NOT NULL,
        AId UNIQUEIDENTIFIER NOT NULL,
        BId UNIQUEIDENTIFIER NOT NULL,
        ScheduledAt DATETIME2 NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Scheduled'
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LeagueMatchResults')
BEGIN
    CREATE TABLE LeagueMatchResults (
        MatchId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        WinnerId UNIQUEIDENTIFIER NOT NULL,
        ScoreA INT NOT NULL,
        ScoreB INT NOT NULL,
        DetailsJson NVARCHAR(MAX) NULL
    );
END;
")
    };

    public static async Task ApplyMigrationsAsync(IServiceProvider services)
    {
        var configuration = services.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("Default")
            ?? configuration.GetConnectionString("CAFASuiteConnection")
            ?? throw new InvalidOperationException("Connection string not configured");

        EnsureDatabaseExists(connectionString);

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await EnsureMigrationsTableAsync(connection);

        var applied = new HashSet<string>(await connection.QueryAsync<string>("SELECT MigrationId FROM SchemaMigrations"));

        foreach (var migration in Migrations)
        {
            if (applied.Contains(migration.Id))
            {
                continue;
            }

            await using var transaction = await connection.BeginTransactionAsync();
            await connection.ExecuteAsync(migration.Sql, transaction: transaction);
            await connection.ExecuteAsync(
                "INSERT INTO SchemaMigrations (MigrationId, AppliedOn) VALUES (@Id, SYSUTCDATETIME())",
                new { migration.Id },
                transaction);
            await transaction.CommitAsync();
        }
    }

    private static void EnsureDatabaseExists(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);
        var databaseName = builder.InitialCatalog;
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new InvalidOperationException("InitialCatalog/Database must be set in the connection string.");
        }

        builder.InitialCatalog = "master";
        using var connection = new SqlConnection(builder.ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"IF DB_ID(@dbName) IS NULL CREATE DATABASE [{databaseName}]";
        var parameter = command.CreateParameter();
        parameter.ParameterName = "@dbName";
        parameter.Value = databaseName;
        command.Parameters.Add(parameter);
        command.ExecuteNonQuery();
    }

    private static async Task EnsureMigrationsTableAsync(SqlConnection connection)
    {
        const string sql = @"
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SchemaMigrations')
BEGIN
    CREATE TABLE SchemaMigrations (
        MigrationId NVARCHAR(200) NOT NULL PRIMARY KEY,
        AppliedOn DATETIME2 NOT NULL
    );
END;";
        await connection.ExecuteAsync(sql);
    }
}
