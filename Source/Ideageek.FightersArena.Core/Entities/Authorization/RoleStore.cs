using Dapper;
using Ideageek.FightersArena.Core.Entities.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace Ideageek.FightersArena.Services.Authorization
{
    public class RoleStore : IRoleStore<AspNetRole>
    {
        private readonly IDbConnection _dbConnection;

        public RoleStore(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IdentityResult> CreateAsync(AspNetRole role, CancellationToken cancellationToken)
        {
            string query = "INSERT INTO AspNetRoles (Id, Name, NormalizedName) VALUES (@Id, @Name, @NormalizedName)";
            await _dbConnection.ExecuteAsync(query, role);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(AspNetRole role, CancellationToken cancellationToken)
        {
            string query = "DELETE FROM AspNetRoles WHERE Id = @Id";
            await _dbConnection.ExecuteAsync(query, new { role.Id });
            return IdentityResult.Success;
        }

        public async Task<AspNetRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            string query = "SELECT * FROM AspNetRoles WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<AspNetRole>(query, new { Id = roleId });
        }

        public async Task<AspNetRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            string query = "SELECT * FROM AspNetRoles WHERE NormalizedName = @NormalizedName";
            return await _dbConnection.QueryFirstOrDefaultAsync<AspNetRole>(query, new { NormalizedName = normalizedRoleName });
        }

        public Task<string?> GetRoleIdAsync(AspNetRole role, CancellationToken cancellationToken)
            => Task.FromResult<string?>(role.Id.ToString());

        public Task<string?> GetRoleNameAsync(AspNetRole role, CancellationToken cancellationToken)
            => Task.FromResult<string?>(role.Name);

        public Task SetRoleNameAsync(AspNetRole role, string? roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public Task<string?> GetNormalizedRoleNameAsync(AspNetRole role, CancellationToken cancellationToken)
            => Task.FromResult<string?>(role.NormalizedName);

        public Task SetNormalizedRoleNameAsync(AspNetRole role, string? normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(AspNetRole role, CancellationToken cancellationToken)
        {
            string query = "UPDATE AspNetRoles SET Name = @Name, NormalizedName = @NormalizedName WHERE Id = @Id";
            return _dbConnection.ExecuteAsync(query, role).ContinueWith(_ => IdentityResult.Success);
        }

        public void Dispose() { }
    }
}
