# Ideageek.FightersArena — Codex Plan (.NET API + SQL Server + SqlKata)

> Target: Build a pure .NET Web API (no frontend) using SQL Server and SqlKata for all data access, following the existing stack (SqlKata + Dapper + Microsoft.Data.SqlClient; DI for QueryFactory; JWT/Identity preserved).

---

## 1) Current Repo State
- Solution: `Ideageek.FightersArena.sln`
- Projects under `Source/`:
  - `Ideageek.FightersArena.Api`
  - `Ideageek.FightersArena.Core`
  - `Ideageek.FightersArena.Tests`
- Goal: Controllers stay thin; business logic in Core; repositories use SqlKata QueryFactory (no stored procedures).

---

## 2) Stack & Standards (Must Use)
### Backend
- ASP.NET Core Web API (minimal hosting `Program.cs`)
- SQL Server via `Microsoft.Data.SqlClient`
- SqlKata + SqlKata.Execution for query building/execution
- Dapper for mapping (through SqlKata.Execution; occasional raw Dapper acceptable)

### Auth
- Preserve JWT configuration and Identity wiring consistent with reference approach.

### Testing
- Lightweight smoke/compile-time tests to ensure SqlKata query generation and DI wiring are valid.

---

## 3) Architecture
### Project Responsibilities
**Ideageek.FightersArena.Api**
- HTTP surface: Controllers, request/response models
- Authentication & authorization (JWT/Identity)
- DI composition root (DB + repositories + services)
- Exception handling + validation pipeline

**Ideageek.FightersArena.Core**
- Domain entities, DTOs
- Business services
- SqlKata repositories
- Shared utilities (pagination, results, errors)

**Ideageek.FightersArena.Tests**
- Smoke tests for DI container, QueryFactory wiring, and key query-building paths

### Layering Rules
- Controllers call Services only.
- Services use Repositories.
- Repositories use SqlKata QueryFactory (avoid stored procs).

---

## 4) Core Features Scope (MVP)
### Public/Player
- Player registration/login (JWT)
- Player profile (GamerTag unique, bio/avatar/socials optional, sponsor optional, games played many-to-many, season points/history)
- Public reads: upcoming tournaments/events, leaderboard, tournaments list/details, teams list/details

### Admin
- CRUD: Games, Sponsors, Players, Teams, Seasons, Tournaments
- Tournament builder: multi-stage (2–3 stages), Round Robin + Double Elimination
- Match generation + result entry
- Finalize tournament + compute placements + award points ledger
- Season management (yearly reset via new season)

---

## 5) Database Design (SQL Server, GUID PKs)
- Players, Sponsors, Games, PlayerGames
- Teams, TeamMembers (history-friendly)
- Seasons, Tournaments, TournamentStages, StageParticipants
- Matches, MatchResults, Placements
- PointsRules, PointsLedger

---

## 6) DI Setup (SqlKata + SQL Server)
In `Ideageek.FightersArena.Api/Program.cs` (via `AddIdeageekFightersArenaApi`):
- `IDbConnection` with `SqlConnection`
- `SqlServerCompiler`
- `QueryFactory`
- Repositories + services

---

## 7) Repository Pattern (SqlKata)
- BaseRepository wrapping QueryFactory
- Helpers: `Query()`, CRUD, pagination (`ForPage`), optional transaction support

---

## 8) API Endpoints (Proposed)
### Auth
- POST `/api/auth/register`
- POST `/api/auth/login`
- GET `/api/auth/me`

### Public
- GET `/api/home`
- GET `/api/players`
- GET `/api/players/{id}`
- GET `/api/teams`
- GET `/api/teams/{id}`
- GET `/api/tournaments`
- GET `/api/tournaments/{id}`
- GET `/api/leaderboards/current?type=players&top=10&gameId=...`

### Admin (JWT + role=Admin)
- POST/GET/PUT/DELETE `/api/admin/games`
- POST/GET/PUT/DELETE `/api/admin/sponsors`
- POST/GET/PUT/DELETE `/api/admin/players`
- POST/GET/PUT/DELETE `/api/admin/teams`
- POST/GET/PUT/DELETE `/api/admin/seasons`
- POST/GET/PUT/DELETE `/api/admin/tournaments`
- POST `/api/admin/tournaments/{id}/stages`
- POST `/api/admin/stages/{stageId}/generate-matches`
- POST `/api/admin/matches/{matchId}/result`
- POST `/api/admin/tournaments/{id}/finalize`

---

## 9) Tournament Builder Logic (Service Layer)
1. Create tournament (basic fields)
2. Add participants (players/teams)
3. Add ordered stages
4. Store stage rules as JSON
5. Generate matches for stage 1
6. Record results + update standings/brackets
7. Advance qualifiers to next stage and generate next matches
8. Finalize tournament + placements + points ledger

---

## 10) Validation & Testing
- `Ideageek.FightersArena.Tests`: DI build smoke test, QueryFactory wiring test, SqlKata query generation smoke test (compile-time focus)

---

## 11) Configuration
- `ConnectionStrings:Default`
- `Jwt:Issuer`, `Jwt:Audience`, `Jwt:Key`, `Jwt:ExpiryMinutes`
- Optional: `Cors:AllowedOrigins`

---

## 12) Implementation Milestones
1. Foundation: packages + DI wiring (SqlClient, SqlKata, Dapper)
2. Auth: JWT (+ Identity if required)
3. CRUD: Games/Sponsors/Players/Teams + public reads
4. Seasons & Points: PointsRules/Ledger + leaderboard
5. Tournament MVP: single stage then multi-stage Round Robin + Double Elim
6. Hardening: validation, logging, exception middleware

---

## 13) Acceptance Criteria (MVP)
- API connects to SQL Server using SqlClient + SqlKata DI wiring
- Admin CRUD works for core entities
- Multi-stage tournament (Round Robin + Double Elim) runs end-to-end
- Points ledger updates and leaderboard reflects totals
- Season reset supported by closing season and starting new one

---

## Appendix — Stage Config JSON Examples
### Round Robin
```json
{
  "type": "ROUND_ROBIN",
  "groups": 2,
  "bestOf": 1,
  "scoring": { "win": 3, "draw": 1, "loss": 0 },
  "tiebreakers": ["POINTS", "HEAD_TO_HEAD", "SCORE_DIFF"],
  "advance": { "fromEachGroup": 2 }
}
```

### Double Elimination
```json
{
  "type": "DOUBLE_ELIM",
  "bracketSize": 8,
  "bestOf": 3,
  "seeding": "MANUAL",
  "thirdPlaceMatch": false
}
```
