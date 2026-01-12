using Ideageek.FightersArena.Core.Dtos;
using Ideageek.FightersArena.Core.Entities;
using Ideageek.FightersArena.Core.Entities.Authorization;
using Ideageek.FightersArena.Core.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ideageek.FightersArena.Core.Services;

public interface IAuthService
{
    Task<AuthResponse?> RegisterAsync(AuthRegisterRequest request);
    Task<AuthResponse?> LoginAsync(AuthLoginRequest request);
}

public class AuthService : IAuthService
{
    private readonly IUserStore<AspNetUser> _userStore;
    private readonly IPasswordHasher<AspNetUser> _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly PlayerRepository _playerRepository;

    public AuthService(IUserStore<AspNetUser> userStore,
        IPasswordHasher<AspNetUser> passwordHasher,
        IConfiguration configuration,
        PlayerRepository playerRepository)
    {
        _userStore = userStore;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _playerRepository = playerRepository;
    }

    public async Task<AuthResponse?> RegisterAsync(AuthRegisterRequest request)
    {
        var user = new AspNetUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            NormalizedUserName = request.Email.ToUpperInvariant(),
            Email = request.Email,
            NormalizedEmail = request.Email.ToUpperInvariant(),
            FullName = request.DisplayName
        };

        var hash = _passwordHasher.HashPassword(user, request.Password);
        user.PasswordHash = hash;

        var result = await _userStore.CreateAsync(user, CancellationToken.None);
        if (!result.Succeeded) return null;

        var player = new Player
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            GamerTag = request.GamerTag,
            DisplayName = request.DisplayName,
            CreatedAt = DateTime.UtcNow
        };
        await _playerRepository.InsertAsync(player);

        return BuildToken(user);
    }

    public async Task<AuthResponse?> LoginAsync(AuthLoginRequest request)
    {
        var normalized = request.Email.ToUpperInvariant();
        var user = await _userStore.FindByNameAsync(normalized, CancellationToken.None);
        if (user is null || string.IsNullOrEmpty(user.PasswordHash))
        {
            return null;
        }

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verification == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return BuildToken(user);
    }

    private AuthResponse BuildToken(AspNetUser user)
    {
        var issuer = _configuration["Jwt:Issuer"] ?? "Ideageek.FightersArena";
        var audience = _configuration["Jwt:Audience"] ?? "Ideageek.FightersArena.Api";
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
        var expiryMinutes = int.TryParse(_configuration["Jwt:ExpiryMinutes"], out var parsed) ? parsed : 120;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return new AuthResponse(user.Id, tokenValue, expires);
    }
}

public interface IGameService
{
    Task<IEnumerable<Game>> GetAllAsync();
    Task<Game?> GetAsync(Guid id);
    Task<Guid> CreateAsync(CreateGameRequest request);
    Task UpdateAsync(Guid id, CreateGameRequest request);
    Task DeleteAsync(Guid id);
}

public class GameService : IGameService
{
    private readonly GameRepository _repository;

    public GameService(GameRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Game>> GetAllAsync() => _repository.GetAllAsync();
    public Task<Game?> GetAsync(Guid id) => _repository.GetByIdAsync(id);

    public async Task<Guid> CreateAsync(CreateGameRequest request)
    {
        var entity = new Game
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = request.Slug,
            CoverUrl = request.CoverUrl,
            CreatedAt = DateTime.UtcNow
        };
        return await _repository.InsertAsync(entity);
    }

    public Task UpdateAsync(Guid id, CreateGameRequest request) =>
        _repository.UpdateAsync(id, new { request.Name, request.Slug, request.CoverUrl });

    public Task DeleteAsync(Guid id) => _repository.DeleteAsync(id);
}

public interface ISponsorService
{
    Task<IEnumerable<Sponsor>> GetAllAsync();
    Task<Guid> CreateAsync(CreateSponsorRequest request);
    Task UpdateAsync(Guid id, CreateSponsorRequest request);
    Task DeleteAsync(Guid id);
}

public class SponsorService : ISponsorService
{
    private readonly SponsorRepository _repository;
    public SponsorService(SponsorRepository repository) => _repository = repository;

    public Task<IEnumerable<Sponsor>> GetAllAsync() => _repository.GetAllAsync();

    public async Task<Guid> CreateAsync(CreateSponsorRequest request)
    {
        var sponsor = new Sponsor
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            LogoUrl = request.LogoUrl,
            WebsiteUrl = request.WebsiteUrl,
            CreatedAt = DateTime.UtcNow
        };
        return await _repository.InsertAsync(sponsor);
    }

    public Task UpdateAsync(Guid id, CreateSponsorRequest request) =>
        _repository.UpdateAsync(id, new { request.Name, request.LogoUrl, request.WebsiteUrl });

    public Task DeleteAsync(Guid id) => _repository.DeleteAsync(id);
}

public interface IPlayerService
{
    Task<IEnumerable<Player>> GetAllAsync();
    Task<Player?> GetAsync(Guid id);
    Task<Guid> CreateAsync(CreatePlayerRequest request);
    Task UpdateAsync(Guid id, UpdatePlayerRequest request);
    Task DeleteAsync(Guid id);
}

public class PlayerService : IPlayerService
{
    private readonly PlayerRepository _playerRepository;

    public PlayerService(PlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public Task<IEnumerable<Player>> GetAllAsync() => _playerRepository.GetAllAsync();
    public Task<Player?> GetAsync(Guid id) => _playerRepository.GetByIdAsync(id);

    public async Task<Guid> CreateAsync(CreatePlayerRequest request)
    {
        var player = new Player
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            GamerTag = request.GamerTag,
            DisplayName = request.DisplayName,
            SponsorId = request.SponsorId,
            AvatarUrl = request.AvatarUrl,
            Bio = request.Bio,
            Country = request.Country,
            CreatedAt = DateTime.UtcNow
        };

        var id = await _playerRepository.InsertAsync(player);
        await _playerRepository.SetPlayerGamesAsync(id, request.GameIds);
        return id;
    }

    public async Task UpdateAsync(Guid id, UpdatePlayerRequest request)
    {
        await _playerRepository.UpdateAsync(id, new
        {
            request.DisplayName,
            request.SponsorId,
            request.AvatarUrl,
            request.Bio,
            request.Country
        });
        await _playerRepository.SetPlayerGamesAsync(id, request.GameIds);
    }

    public Task DeleteAsync(Guid id) => _playerRepository.DeleteAsync(id);
}

public interface ITeamService
{
    Task<IEnumerable<Team>> GetAllAsync();
    Task<Team?> GetAsync(Guid id);
    Task<Guid> CreateAsync(CreateTeamRequest request);
    Task UpdateAsync(Guid id, UpdateTeamRequest request);
    Task DeleteAsync(Guid id);
}

public class TeamService : ITeamService
{
    private readonly TeamRepository _teamRepository;

    public TeamService(TeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public Task<IEnumerable<Team>> GetAllAsync() => _teamRepository.GetAllAsync();
    public Task<Team?> GetAsync(Guid id) => _teamRepository.GetByIdAsync(id);

    public async Task<Guid> CreateAsync(CreateTeamRequest request)
    {
        var team = new Team
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Tag = request.Tag,
            LogoUrl = request.LogoUrl,
            CaptainPlayerId = request.CaptainPlayerId,
            CreatedAt = DateTime.UtcNow
        };
        var id = await _teamRepository.InsertAsync(team);
        await _teamRepository.SetMembersAsync(id, request.MemberIds);
        return id;
    }

    public async Task UpdateAsync(Guid id, UpdateTeamRequest request)
    {
        await _teamRepository.UpdateAsync(id, new
        {
            request.Name,
            request.Tag,
            request.LogoUrl,
            request.CaptainPlayerId
        });
        await _teamRepository.SetMembersAsync(id, request.MemberIds);
    }

    public Task DeleteAsync(Guid id) => _teamRepository.DeleteAsync(id);
}

public interface ISeasonService
{
    Task<IEnumerable<Season>> GetAllAsync();
    Task<Season?> GetAsync(Guid id);
    Task<Guid> CreateAsync(CreateSeasonRequest request);
    Task UpdateAsync(Guid id, CreateSeasonRequest request);
    Task DeleteAsync(Guid id);
    Task<Season?> GetActiveAsync();
}

public class SeasonService : ISeasonService
{
    private readonly SeasonRepository _seasonRepository;

    public SeasonService(SeasonRepository seasonRepository)
    {
        _seasonRepository = seasonRepository;
    }

    public Task<IEnumerable<Season>> GetAllAsync() => _seasonRepository.GetAllAsync();
    public Task<Season?> GetAsync(Guid id) => _seasonRepository.GetByIdAsync(id);
    public Task<Season?> GetActiveAsync() => _seasonRepository.GetActiveAsync();

    public async Task<Guid> CreateAsync(CreateSeasonRequest request)
    {
        var season = new Season
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsActive = request.IsActive
        };
        return await _seasonRepository.InsertAsync(season);
    }

    public Task UpdateAsync(Guid id, CreateSeasonRequest request) =>
        _seasonRepository.UpdateAsync(id, new
        {
            request.Name,
            request.StartDate,
            request.EndDate,
            request.IsActive
        });

    public Task DeleteAsync(Guid id) => _seasonRepository.DeleteAsync(id);
}

public interface ITournamentService
{
    Task<IEnumerable<Tournament>> GetAllAsync();
    Task<Tournament?> GetAsync(Guid id);
    Task<Guid> CreateAsync(CreateTournamentRequest request);
    Task UpdateAsync(Guid id, CreateTournamentRequest request);
    Task DeleteAsync(Guid id);
    Task AddStageAsync(Guid tournamentId, AddStageRequest request);
    Task AddParticipantsAsync(Guid stageId, AddStageParticipantsRequest request);
    Task<IEnumerable<Match>> GenerateMatchesAsync(Guid stageId, GenerateMatchesRequest request);
    Task RecordResultAsync(Guid matchId, RecordMatchResultRequest request);
    Task FinalizeAsync(Guid tournamentId, FinalizeTournamentRequest request);
}

public class TournamentService : ITournamentService
{
    private readonly TournamentRepository _tournamentRepository;
    private readonly TournamentStageRepository _stageRepository;
    private readonly StageParticipantRepository _participantRepository;
    private readonly MatchRepository _matchRepository;
    private readonly MatchResultRepository _matchResultRepository;
    private readonly PlacementRepository _placementRepository;
    private readonly PointsRuleRepository _pointsRuleRepository;
    private readonly PointsLedgerRepository _pointsLedgerRepository;

    public TournamentService(
        TournamentRepository tournamentRepository,
        TournamentStageRepository stageRepository,
        StageParticipantRepository participantRepository,
        MatchRepository matchRepository,
        MatchResultRepository matchResultRepository,
        PlacementRepository placementRepository,
        PointsRuleRepository pointsRuleRepository,
        PointsLedgerRepository pointsLedgerRepository)
    {
        _tournamentRepository = tournamentRepository;
        _stageRepository = stageRepository;
        _participantRepository = participantRepository;
        _matchRepository = matchRepository;
        _matchResultRepository = matchResultRepository;
        _placementRepository = placementRepository;
        _pointsRuleRepository = pointsRuleRepository;
        _pointsLedgerRepository = pointsLedgerRepository;
    }

    public Task<IEnumerable<Tournament>> GetAllAsync() => _tournamentRepository.GetAllAsync();
    public Task<Tournament?> GetAsync(Guid id) => _tournamentRepository.GetByIdAsync(id);

    public async Task<Guid> CreateAsync(CreateTournamentRequest request)
    {
        var tournament = new Tournament
        {
            Id = Guid.NewGuid(),
            SeasonId = request.SeasonId,
            GameId = request.GameId,
            Name = request.Name,
            Tier = request.Tier,
            BannerUrl = request.BannerUrl,
            RulesText = request.RulesText,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            CreatedAt = DateTime.UtcNow,
            Status = "Draft"
        };
        return await _tournamentRepository.InsertAsync(tournament);
    }

    public Task UpdateAsync(Guid id, CreateTournamentRequest request) =>
        _tournamentRepository.UpdateAsync(id, new
        {
            request.SeasonId,
            request.GameId,
            request.Name,
            request.Tier,
            request.StartDate,
            request.EndDate,
            request.BannerUrl,
            request.RulesText
        });

    public Task DeleteAsync(Guid id) => _tournamentRepository.DeleteAsync(id);

    public async Task AddStageAsync(Guid tournamentId, AddStageRequest request)
    {
        var stage = new TournamentStage
        {
            Id = Guid.NewGuid(),
            TournamentId = tournamentId,
            StageOrder = request.StageOrder,
            Name = request.Name,
            Format = request.Format,
            ConfigJson = request.ConfigJson,
            CreatedAt = DateTime.UtcNow
        };
        await _stageRepository.InsertAsync(stage);
        await _tournamentRepository.SetStatusAsync(tournamentId, "Configured");
    }

    public async Task AddParticipantsAsync(Guid stageId, AddStageParticipantsRequest request)
    {
        var participants = request.Participants.Select(p => new StageParticipant
        {
            Id = Guid.NewGuid(),
            StageId = stageId,
            ParticipantType = p.ParticipantType,
            ParticipantId = p.ParticipantId,
            Seed = p.Seed,
            GroupId = p.GroupId
        });

        await _participantRepository.ReplaceAsync(stageId, participants);
    }

    public async Task<IEnumerable<Match>> GenerateMatchesAsync(Guid stageId, GenerateMatchesRequest request)
    {
        var participants = (await _participantRepository.GetByStageAsync(stageId))
            .OrderBy(p => p.Seed)
            .ToList();

        var matches = new List<Match>();
        for (int i = 0; i < participants.Count - 1; i += 2)
        {
            var a = participants[i];
            var b = participants[i + 1];
            matches.Add(new Match
            {
                Id = Guid.NewGuid(),
                StageId = stageId,
                RoundNumber = 1,
                BracketType = request.Strategy,
                GroupId = a.GroupId ?? b.GroupId,
                AId = a.ParticipantId,
                BId = b.ParticipantId,
                ScheduledAt = DateTime.UtcNow.AddHours(1),
                Status = "Scheduled"
            });
        }

        if (matches.Count > 0)
        {
            await _matchRepository.InsertManyAsync(matches);
        }
        return matches;
    }

    public async Task RecordResultAsync(Guid matchId, RecordMatchResultRequest request)
    {
        var result = new MatchResult
        {
            MatchId = matchId,
            WinnerId = request.WinnerId,
            ScoreA = request.ScoreA,
            ScoreB = request.ScoreB,
            DetailsJson = request.DetailsJson
        };

        await _matchResultRepository.UpsertAsync(result);
        await _matchRepository.UpdateAsync(matchId, new { Status = "Completed" });
    }

    public async Task FinalizeAsync(Guid tournamentId, FinalizeTournamentRequest request)
    {
        var stages = (await _stageRepository.GetByTournamentAsync(tournamentId)).ToList();
        var finalStage = stages.OrderByDescending(s => s.StageOrder).FirstOrDefault();
        var participants = (finalStage != null
            ? await _participantRepository.GetByStageAsync(finalStage.Id)
            : await _participantRepository.GetByTournamentAsync(tournamentId)).ToList();

        int placementNumber = 1;
        foreach (var participant in participants.OrderBy(p => p.Seed))
        {
            var placement = new Placement
            {
                Id = Guid.NewGuid(),
                TournamentId = tournamentId,
                ParticipantType = participant.ParticipantType,
                ParticipantId = participant.ParticipantId,
                PlacementNumber = placementNumber++
            };
            await _placementRepository.UpsertAsync(placement);
        }

        if (request.AwardPoints)
        {
            var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
            if (tournament is not null)
            {
                var rules = (await _pointsRuleRepository.GetByTierAsync(tournament.Tier)).ToList();
                foreach (var placement in participants.OrderBy(p => p.Seed).Select((p, index) => new { Participant = p, Number = index + 1 }))
                {
                    var rule = rules.FirstOrDefault(r => r.PlacementNumber == placement.Number);
                    if (rule != null)
                    {
                        var ledger = new PointsLedger
                        {
                            Id = Guid.NewGuid(),
                            SeasonId = tournament.SeasonId,
                            TournamentId = tournamentId,
                            ParticipantType = placement.Participant.ParticipantType,
                            ParticipantId = placement.Participant.ParticipantId,
                            Points = rule.Points,
                            Reason = $"Placement {placement.Number}",
                            CreatedAt = DateTime.UtcNow
                        };
                        await _pointsLedgerRepository.InsertAsync(ledger);
                    }
                }
            }
        }

        await _tournamentRepository.SetStatusAsync(tournamentId, "Finalized");
    }
}

public interface ILeaderboardService
{
    Task<IEnumerable<LeaderboardEntryDto>> GetCurrentAsync(string type, int top, Guid? gameId);
}

public class LeaderboardService : ILeaderboardService
{
    private readonly PointsLedgerRepository _ledgerRepository;
    private readonly SeasonRepository _seasonRepository;

    public LeaderboardService(PointsLedgerRepository ledgerRepository, SeasonRepository seasonRepository)
    {
        _ledgerRepository = ledgerRepository;
        _seasonRepository = seasonRepository;
    }

    public async Task<IEnumerable<LeaderboardEntryDto>> GetCurrentAsync(string type, int top, Guid? gameId)
    {
        var season = await _seasonRepository.GetActiveAsync();
        if (season == null) return Enumerable.Empty<LeaderboardEntryDto>();

        var entries = await _ledgerRepository.GetBySeasonAsync(season.Id);
        var filtered = entries.Where(e => e.ParticipantType.Equals(type, StringComparison.OrdinalIgnoreCase));
        var grouped = filtered.GroupBy(e => e.ParticipantId)
            .Select(g => new LeaderboardEntryDto(g.Key, type, g.Sum(x => x.Points)))
            .OrderByDescending(x => x.Points)
            .Take(top);
        return grouped;
    }
}

public interface IHomeService
{
    Task<HomeSummaryDto> GetSummaryAsync();
}

public class HomeService : IHomeService
{
    private readonly TournamentRepository _tournamentRepository;
    private readonly ILeaderboardService _leaderboardService;
    private readonly PlayerRepository _playerRepository;
    private readonly TeamRepository _teamRepository;

    public HomeService(
        TournamentRepository tournamentRepository,
        ILeaderboardService leaderboardService,
        PlayerRepository playerRepository,
        TeamRepository teamRepository)
    {
        _tournamentRepository = tournamentRepository;
        _leaderboardService = leaderboardService;
        _playerRepository = playerRepository;
        _teamRepository = teamRepository;
    }

    public async Task<HomeSummaryDto> GetSummaryAsync()
    {
        var upcoming = await _tournamentRepository.GetUpcomingAsync();
        var topPlayers = (await _leaderboardService.GetCurrentAsync("Player", 5, null))
            .Join(await _playerRepository.GetAllAsync(), l => l.ParticipantId, p => p.Id, (l, p) => p)
            .ToList();

        var topTeams = (await _leaderboardService.GetCurrentAsync("Team", 5, null))
            .Join(await _teamRepository.GetAllAsync(), l => l.ParticipantId, t => t.Id, (l, t) => t)
            .ToList();

        return new HomeSummaryDto(upcoming, topPlayers, topTeams);
    }
}
