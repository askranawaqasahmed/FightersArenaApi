using Ideageek.FightersArena.Core.Entities;

namespace Ideageek.FightersArena.Core.Dtos;

public record PagedResult<T>(IEnumerable<T> Items, int Total);

public record AuthRegisterRequest(string Email, string Password, string GamerTag, string DisplayName);
public record AuthLoginRequest(string Email, string Password);
public record AuthResponse(Guid UserId, string Token, DateTime ExpiresAt);

public record CreateGameRequest(string Name, string Slug, string? CoverUrl);
public record CreateSponsorRequest(string Name, string? LogoUrl, string? WebsiteUrl);
public record CreatePlayerRequest(Guid UserId, string GamerTag, string DisplayName, Guid? SponsorId, string? AvatarUrl, string? Bio, string? Country, IEnumerable<Guid>? GameIds);
public record UpdatePlayerRequest(string DisplayName, Guid? SponsorId, string? AvatarUrl, string? Bio, string? Country, IEnumerable<Guid>? GameIds);

public record CreateTeamRequest(string Name, string? Tag, string? LogoUrl, Guid? CaptainPlayerId, IEnumerable<Guid>? MemberIds);
public record UpdateTeamRequest(string Name, string? Tag, string? LogoUrl, Guid? CaptainPlayerId, IEnumerable<Guid>? MemberIds);

public record CreateSeasonRequest(string Name, DateTime StartDate, DateTime? EndDate, bool IsActive);
public record CreateTournamentRequest(Guid SeasonId, Guid GameId, string Name, string Tier, DateTime StartDate, DateTime? EndDate, string? BannerUrl, string? RulesText);
public record AddStageRequest(int StageOrder, string Name, string Format, string? ConfigJson);
public record AddStageParticipantsRequest(IEnumerable<StageParticipantDto> Participants);
public record StageParticipantDto(string ParticipantType, Guid ParticipantId, int Seed, string? GroupId);
public record GenerateMatchesRequest(string Strategy);
public record RecordMatchResultRequest(Guid WinnerId, int ScoreA, int ScoreB, string? DetailsJson);
public record FinalizeTournamentRequest(bool AwardPoints);
public record LeaderboardEntryDto(Guid ParticipantId, string ParticipantType, int Points);

public record HomeSummaryDto(
    IEnumerable<Tournament> UpcomingTournaments,
    IEnumerable<Player> TopPlayers,
    IEnumerable<Team> TopTeams);
