using Dapper;
using Ideageek.FightersArena.Core.Entities.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Ideageek.FightersArena.Services.Authorization
{
    public class UserStore : IUserStore<AspNetUser>, IUserPasswordStore<AspNetUser>, IUserRoleStore<AspNetUser>
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public UserStore(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("Default")
                ?? _configuration.GetConnectionString("CAFASuiteConnection")
                ?? throw new ArgumentNullException("Connection string not found");
        }

        private DbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IdentityResult> CreateAsync(AspNetUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var db = CreateConnection();
            string sql = @"
            INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, PhoneNumber)
            VALUES (@Id, @UserName, @NormalizedUserName, @Email, @NormalizedEmail, @EmailConfirmed, @PasswordHash, @PhoneNumber)";

            int rowsAffected = await db.ExecuteAsync(sql, user);
            return rowsAffected > 0 ? IdentityResult.Success : IdentityResult.Failed();
        }
        public async Task<IdentityResult> UpdateAsync(AspNetUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var db = CreateConnection();
            string sql = @"
                UPDATE AspNetUsers 
                SET UserName = @UserName, 
                    NormalizedUserName = @NormalizedUserName, 
                    Email = @Email, 
                    NormalizedEmail = @NormalizedEmail, 
                    EmailConfirmed = @EmailConfirmed, 
                    PasswordHash = @PasswordHash,
                    PhoneNumber = @PhoneNumber
                WHERE Id = @Id";

            int rowsAffected = await db.ExecuteAsync(sql, user);
            return rowsAffected > 0 ? IdentityResult.Success : IdentityResult.Failed();
        }
        public async Task<IdentityResult> DeleteAsync(AspNetUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var db = CreateConnection();
            string sql = "DELETE FROM AspNetUsers WHERE Id = @Id";

            int rowsAffected = await db.ExecuteAsync(sql, new { user.Id });
            return rowsAffected > 0 ? IdentityResult.Success : IdentityResult.Failed();
        }

        [return: MaybeNull]
        public async Task<AspNetUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var db = CreateConnection();
            return (await db.QueryFirstOrDefaultAsync<AspNetUser>(
                "SELECT * FROM AspNetUsers WHERE Id = @Id", new { Id = Guid.Parse(userId) }))!;
        }

        [return: MaybeNull]
        public async Task<AspNetUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var db = CreateConnection();
            const string sql = @"
                SELECT TOP 1 *
                FROM AspNetUsers
                WHERE NormalizedUserName = @NormalizedUserName
                   OR LOWER(UserName) = LOWER(@NormalizedUserName)
                   OR LOWER(Email) = LOWER(@NormalizedUserName)";

            return (await db.QueryFirstOrDefaultAsync<AspNetUser>(sql,
                new { NormalizedUserName = normalizedUserName }))!;
        }

        public Task<string> GetUserIdAsync(AspNetUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.Id.ToString());

        public Task<string> GetUserNameAsync(AspNetUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.UserName);

        public Task SetUserNameAsync(AspNetUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(AspNetUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.NormalizedUserName);

        public Task SetNormalizedUserNameAsync(AspNetUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public async Task SetPasswordHashAsync(AspNetUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var db = CreateConnection();
            string sql = "UPDATE AspNetUsers SET PasswordHash = @PasswordHash WHERE Id = @Id";

            await db.ExecuteAsync(sql, new { user.Id, PasswordHash = passwordHash });
        }

        public async Task<string?> GetPasswordHashAsync(AspNetUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var db = CreateConnection();
            return await db.QueryFirstOrDefaultAsync<string>(
                "SELECT PasswordHash FROM AspNetUsers WHERE Id = @Id", new { user.Id });
        }

        public Task<bool> HasPasswordAsync(AspNetUser user, CancellationToken cancellationToken)
        => Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));

        #region Role Methods (For Role-Based Authentication)
        public async Task AddToRoleAsync(AspNetUser user, string roleName, CancellationToken cancellationToken)
        {
            using var db = CreateConnection();
            var role = await db.QuerySingleOrDefaultAsync<AspNetRole>(
                "SELECT * FROM AspNetRoles WHERE NormalizedName = @NormalizedName",
                new { NormalizedName = roleName.ToUpperInvariant() });

            if (role == null)
            {
                role = new AspNetRole
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant()
                };

                const string insertRoleSql = @"
                    INSERT INTO AspNetRoles (Id, Name, NormalizedName)
                    VALUES (@Id, @Name, @NormalizedName)";

                await db.ExecuteAsync(insertRoleSql, role);
            }

            const string sql = "INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)";
            await db.ExecuteAsync(sql, new { UserId = user.Id, RoleId = role.Id });
        }

        public async Task RemoveFromRoleAsync(AspNetUser user, string roleName, CancellationToken cancellationToken)
        {
            using var db = CreateConnection();
            const string sql = "DELETE FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = (SELECT Id FROM AspNetRoles WHERE Name = @RoleName)";
            await db.ExecuteAsync(sql, new { UserId = user.Id, RoleName = roleName });
        }

        public async Task<IList<string>> GetRolesAsync(AspNetUser user, CancellationToken cancellationToken)
        {
            using var db = CreateConnection();
            const string sql = @"
            SELECT r.Name 
            FROM AspNetRoles r 
            INNER JOIN AspNetUserRoles ur ON ur.RoleId = r.Id
            WHERE ur.UserId = @UserId"
            ;

            var roles = await db.QueryAsync<string>(sql, new { UserId = user.Id });
            return roles.ToList();
        }

        public async Task<bool> IsInRoleAsync(AspNetUser user, string roleName, CancellationToken cancellationToken)
        {
            using var db = CreateConnection();
            const string sql = @"
            SELECT COUNT(1) 
            FROM AspNetUserRoles ur 
            INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
            WHERE ur.UserId = @UserId AND r.Name = @RoleName"
            ;

            return await db.ExecuteScalarAsync<bool>(sql, new { UserId = user.Id, RoleName = roleName });
        }

        public async Task<IList<AspNetUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            using var db = CreateConnection();
            const string sql = @"
            SELECT u.* 
            FROM AspNetUsers u 
            INNER JOIN AspNetUserRoles ur ON ur.UserId = u.Id
            INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
            WHERE r.Name = @RoleName"
            ;

            var users = await db.QueryAsync<AspNetUser>(sql, new { RoleName = roleName });
            return users.ToList();
        }
        #endregion
        public void Dispose() { }
    }
}
