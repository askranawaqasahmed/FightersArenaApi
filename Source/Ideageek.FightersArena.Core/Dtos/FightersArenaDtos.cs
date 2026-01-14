using Ideageek.FightersArena.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Ideageek.FightersArena.Core.Dtos;

public record PagedResult<T>(IEnumerable<T> Items, int Total);

public record AuthRegisterRequest(
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password,
    [Required] string GamerTag,
    [Required] string DisplayName);

public record AuthLoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);

public record AuthResponse(Guid UserId, string Token, DateTime ExpiresAt);
public record ForgotPasswordRequest([Required, EmailAddress] string Email);

public record CreateGameRequest(
    [Required] string Name,
    [Required] string Slug,
    string? CoverUrl);

public record CreateSponsorRequest(
    [Required] string Name,
    string? LogoUrl,
    string? WebsiteUrl);

public record CreatePlayerRequest(
    [Required] Guid UserId,
    [Required] string GamerTag,
    [Required] string DisplayName,
    Guid? SponsorId,
    string? AvatarUrl,
    string? Bio,
    string? Country,
    IEnumerable<Guid>? GameIds);

public record UpdatePlayerRequest(
    [Required] string DisplayName,
    Guid? SponsorId,
    string? AvatarUrl,
    string? Bio,
    string? Country,
    IEnumerable<Guid>? GameIds);

public record PlayerProfileDto(Player Player, IEnumerable<Guid> GameIds);

public record CreateTeamRequest(
    [Required] string Name,
    string? Tag,
    string? LogoUrl,
    Guid? CaptainPlayerId,
    IEnumerable<Guid>? MemberIds);

public record UpdateTeamRequest(
    [Required] string Name,
    string? Tag,
    string? LogoUrl,
    Guid? CaptainPlayerId,
    IEnumerable<Guid>? MemberIds);

public record CreateSeasonRequest(
    [Required] string Name,
    [Required] DateTime StartDate,
    DateTime? EndDate,
    bool IsActive);

public record CreateTournamentRequest(
    [Required] Guid SeasonId,
    [Required] Guid GameId,
    [Required] string Name,
    [Required] string Tier,
    [Required] DateTime StartDate,
    DateTime? EndDate,
    string? BannerUrl,
    string? RulesText);

public record AddStageRequest(
    [Range(1, int.MaxValue)] int StageOrder,
    [Required] string Name,
    [Required] string Format,
    string? ConfigJson);

public record AddStageParticipantsRequest(
    [MinLength(1)] IEnumerable<StageParticipantDto> Participants);

public record StageParticipantDto(
    [Required] string ParticipantType,
    [Required] Guid ParticipantId,
    [Range(1, int.MaxValue)] int Seed,
    string? GroupId);

public record GenerateMatchesRequest([Required] string Strategy);

public record RecordMatchResultRequest(
    [Required] Guid WinnerId,
    [Range(0, int.MaxValue)] int ScoreA,
    [Range(0, int.MaxValue)] int ScoreB,
    string? DetailsJson);

public record FinalizeTournamentRequest(bool AwardPoints);

public record RegisterTournamentRequest(
    [Required] string ParticipantType,
    [Required] Guid ParticipantId,
    [Range(1, int.MaxValue)] int? Seed,
    string? GroupId);

public record LeaderboardEntryDto(Guid ParticipantId, string ParticipantType, int Points);

public record HomeSummaryDto(
    IEnumerable<Tournament> UpcomingTournaments,
    IEnumerable<Player> TopPlayers,
    IEnumerable<Team> TopTeams);

public record CreateLeagueRequest(
    [Required] Guid SeasonId,
    [Required] Guid GameId,
    [Required] string Name,
    [Required] DateTime StartDate,
    DateTime? EndDate);

public record UpdateLeagueRequest(
    [Required] Guid SeasonId,
    [Required] Guid GameId,
    [Required] string Name,
    [Required] DateTime StartDate,
    DateTime? EndDate,
    [Required] string Status);

public record AddLeagueParticipantsRequest(
    [MinLength(1)] IEnumerable<StageParticipantDto> Participants);

public record GenerateLeagueMatchesRequest(bool DoubleRoundRobin = false);

public record RecordLeagueMatchResultRequest(
    [Required] Guid WinnerId,
    [Range(0, int.MaxValue)] int ScoreA,
    [Range(0, int.MaxValue)] int ScoreB,
    string? DetailsJson);

public record LeagueStandingDto(Guid ParticipantId, string ParticipantType, int Wins, int Losses, int Points);
