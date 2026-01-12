using System.ComponentModel.DataAnnotations;

namespace Ideageek.FightersArena.Core.Entities;

public class Player
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    [Required]
    public string GamerTag { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public Guid? SponsorId { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public string? Country { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Sponsor
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Game
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Slug { get; set; } = string.Empty;
    public string? CoverUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class PlayerGame
{
    public Guid PlayerId { get; set; }
    public Guid GameId { get; set; }
}

public class Team
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Tag { get; set; }
    public string? LogoUrl { get; set; }
    public Guid? CaptainPlayerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class TeamMember
{
    public Guid TeamId { get; set; }
    public Guid PlayerId { get; set; }
    public string Role { get; set; } = "Player";
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LeftAt { get; set; }
}

public class Season
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
}

public class Tournament
{
    [Key]
    public Guid Id { get; set; }
    public Guid SeasonId { get; set; }
    public Guid GameId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string Tier { get; set; } = "Open";
    public string? BannerUrl { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "Draft";
    public string? RulesText { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class TournamentStage
{
    [Key]
    public Guid Id { get; set; }
    public Guid TournamentId { get; set; }
    public int StageOrder { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public string? ConfigJson { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class StageParticipant
{
    [Key]
    public Guid Id { get; set; }
    public Guid StageId { get; set; }
    public string ParticipantType { get; set; } = "Player";
    public Guid ParticipantId { get; set; }
    public int Seed { get; set; }
    public string? GroupId { get; set; }
}

public class Match
{
    [Key]
    public Guid Id { get; set; }
    public Guid StageId { get; set; }
    public int RoundNumber { get; set; }
    public string? BracketType { get; set; }
    public string? GroupId { get; set; }
    public Guid AId { get; set; }
    public Guid BId { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public string Status { get; set; } = "Pending";
}

public class MatchResult
{
    [Key]
    public Guid MatchId { get; set; }
    public Guid WinnerId { get; set; }
    public int ScoreA { get; set; }
    public int ScoreB { get; set; }
    public string? DetailsJson { get; set; }
}

public class Placement
{
    [Key]
    public Guid Id { get; set; }
    public Guid TournamentId { get; set; }
    public string ParticipantType { get; set; } = "Player";
    public Guid ParticipantId { get; set; }
    public int PlacementNumber { get; set; }
}

public class PointsRule
{
    [Key]
    public Guid Id { get; set; }
    public string Tier { get; set; } = string.Empty;
    public int PlacementNumber { get; set; }
    public int Points { get; set; }
}

public class PointsLedger
{
    [Key]
    public Guid Id { get; set; }
    public Guid SeasonId { get; set; }
    public Guid TournamentId { get; set; }
    public string ParticipantType { get; set; } = "Player";
    public Guid ParticipantId { get; set; }
    public int Points { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
