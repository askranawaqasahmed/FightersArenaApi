using Ideageek.FightersArena.Core.Entities;
using SqlKata.Execution;

namespace Ideageek.FightersArena.Core.Repositories;

public class GameRepository : BaseRepository<Game>
{
    public GameRepository(QueryFactory db) : base(db, "Games") { }
}

public class SponsorRepository : BaseRepository<Sponsor>
{
    public SponsorRepository(QueryFactory db) : base(db, "Sponsors") { }
}

public class PlayerRepository : BaseRepository<Player>
{
    public PlayerRepository(QueryFactory db) : base(db, "Players") { }

    public Task<IEnumerable<Player>> GetByIdsAsync(IEnumerable<Guid> ids) => Query().WhereIn("Id", ids).GetAsync<Player>();

    public async Task SetPlayerGamesAsync(Guid playerId, IEnumerable<Guid>? gameIds)
    {
        await Db.Query("PlayerGames").Where("PlayerId", playerId).DeleteAsync();
        if (gameIds is null) return;

        var payload = gameIds.Select(id => new PlayerGame { PlayerId = playerId, GameId = id });
        await Db.Query("PlayerGames").InsertAsync(payload);
    }
}

public class TeamRepository : BaseRepository<Team>
{
    public TeamRepository(QueryFactory db) : base(db, "Teams") { }

    public async Task SetMembersAsync(Guid teamId, IEnumerable<Guid>? playerIds)
    {
        await Db.Query("TeamMembers").Where("TeamId", teamId).DeleteAsync();
        if (playerIds is null) return;

        var members = playerIds.Select(id => new TeamMember
        {
            TeamId = teamId,
            PlayerId = id,
            Role = "Player",
            JoinedAt = DateTime.UtcNow
        });
        await Db.Query("TeamMembers").InsertAsync(members);
    }
}

public class SeasonRepository : BaseRepository<Season>
{
    public SeasonRepository(QueryFactory db) : base(db, "Seasons") { }

    public async Task<Season?> GetActiveAsync()
    {
        var season = await Query().Where("IsActive", true).FirstOrDefaultAsync<Season>();
        return season;
    }
}

public class TournamentRepository : BaseRepository<Tournament>
{
    public TournamentRepository(QueryFactory db) : base(db, "Tournaments") { }

    public Task<IEnumerable<Tournament>> GetUpcomingAsync(int top = 5) =>
        Query().Where("StartDate", ">", DateTime.UtcNow).OrderBy("StartDate").Limit(top).GetAsync<Tournament>();

    public Task<int> SetStatusAsync(Guid tournamentId, string status) =>
        UpdateAsync(tournamentId, new { Status = status });
}

public class TournamentStageRepository : BaseRepository<TournamentStage>
{
    public TournamentStageRepository(QueryFactory db) : base(db, "TournamentStages") { }

    public Task<IEnumerable<TournamentStage>> GetByTournamentAsync(Guid tournamentId) =>
        Query().Where("TournamentId", tournamentId).OrderBy("StageOrder").GetAsync<TournamentStage>();
}

public class StageParticipantRepository : BaseRepository<StageParticipant>
{
    public StageParticipantRepository(QueryFactory db) : base(db, "StageParticipants") { }

    public Task<IEnumerable<StageParticipant>> GetByStageAsync(Guid stageId) =>
        Query().Where("StageId", stageId).GetAsync<StageParticipant>();

    public Task<IEnumerable<StageParticipant>> GetByTournamentAsync(Guid tournamentId) =>
        Query().Join("TournamentStages as ts", "ts.Id", "StageParticipants.StageId")
            .Where("ts.TournamentId", tournamentId)
            .GetAsync<StageParticipant>();

    public async Task ReplaceAsync(Guid stageId, IEnumerable<StageParticipant> participants)
    {
        await Query().Where("StageId", stageId).DeleteAsync();
        await Query().InsertAsync(participants);
    }
}

public class MatchRepository : BaseRepository<Match>
{
    public MatchRepository(QueryFactory db) : base(db, "Matches") { }

    public Task<IEnumerable<Match>> GetByStageAsync(Guid stageId) =>
        Query().Where("StageId", stageId).GetAsync<Match>();

    public Task<IEnumerable<Match>> GetByTournamentAsync(Guid tournamentId) =>
        Query().Join("TournamentStages as ts", "ts.Id", "Matches.StageId")
            .Where("ts.TournamentId", tournamentId)
            .GetAsync<Match>();

    public Task InsertManyAsync(IEnumerable<Match> matches) => Query().InsertAsync(matches);
}

public class MatchResultRepository : BaseRepository<MatchResult>
{
    public MatchResultRepository(QueryFactory db) : base(db, "MatchResults", "MatchId") { }

    public async Task UpsertAsync(MatchResult result)
    {
        await Query().Where("MatchId", result.MatchId).DeleteAsync();
        await InsertAsync(result);
    }

    public Task<IEnumerable<MatchResult>> GetByTournamentAsync(Guid tournamentId) =>
        Query().Join("Matches as m", "m.Id", "MatchResults.MatchId")
            .Join("TournamentStages as ts", "ts.Id", "m.StageId")
            .Where("ts.TournamentId", tournamentId)
            .GetAsync<MatchResult>();
}

public class PlacementRepository : BaseRepository<Placement>
{
    public PlacementRepository(QueryFactory db) : base(db, "Placements") { }

    public async Task UpsertAsync(Placement placement)
    {
        await Query().Where("TournamentId", placement.TournamentId)
            .Where("ParticipantId", placement.ParticipantId)
            .DeleteAsync();
        await InsertAsync(placement);
    }
}

public class PointsRuleRepository : BaseRepository<PointsRule>
{
    public PointsRuleRepository(QueryFactory db) : base(db, "PointsRules") { }

    public Task<IEnumerable<PointsRule>> GetByTierAsync(string tier) =>
        Query().Where("Tier", tier).GetAsync<PointsRule>();
}

public class PointsLedgerRepository : BaseRepository<PointsLedger>
{
    public PointsLedgerRepository(QueryFactory db) : base(db, "PointsLedger") { }

    public Task<IEnumerable<PointsLedger>> GetBySeasonAsync(Guid seasonId) =>
        Query().Where("SeasonId", seasonId).GetAsync<PointsLedger>();
}
