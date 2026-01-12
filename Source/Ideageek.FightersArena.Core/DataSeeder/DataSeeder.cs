using Dapper;
using Ideageek.FightersArena.Core.Entities.Authorization;
using Ideageek.FightersArena.Core.Entities.Setting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace Ideageek.FightersArena.Core.DataSeeder
{
    public class DataSeeder
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DataSeeder> _logger;
        private readonly IPasswordHasher<AspNetUser> _passwordHasher;

        public DataSeeder(IServiceProvider serviceProvider, IPasswordHasher<AspNetUser> passwordHasher, ILogger<DataSeeder> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            using var dbConnection = scope.ServiceProvider.GetRequiredService<IDbConnection>();

            try
            {
                await SeedRolesAsync(dbConnection);
                await SeedUsersAsync(dbConnection);
                await SeedGeneralAsync(dbConnection);
                _logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error seeding data: {ex.Message}");
            }
        }

        private async Task SeedRolesAsync(IDbConnection dbConnection)
        {
            string[] roles = { "SuperAdmin", "Admin", "User" };

            foreach (var role in roles)
            {
                var existingRole = await dbConnection.QueryFirstOrDefaultAsync<string>(
                    "SELECT Name FROM AspNetRoles WHERE Name = @Name", new { Name = role });

                if (existingRole == null)
                {
                    await dbConnection.ExecuteAsync("INSERT INTO AspNetRoles (Id, Name, NormalizedName) VALUES (@Id, @Name, @NormalizedName)",
                        new
                        {
                            Id = Guid.NewGuid(),
                            Name = role,
                            NormalizedName = role.ToUpper()
                        });
                    _logger.LogInformation($"Role '{role}' inserted.");
                }
            }
        }

        private async Task SeedUsersAsync(IDbConnection dbConnection)
        {
            var users = new[]
            {
        new { FullName = "SuperAdmin", UserName = "superadmin@ideageek.pk", Email = "superadmin@ideageek.pk", Role = "SuperAdmin", IsAdmin = false },
        new { FullName = "Admin", UserName = "admin@ideageek.pk", Email = "admin@ideageek.pk", Role = "Admin", IsAdmin = true },
        new { FullName = "User", UserName = "user@ideageek.pk", Email = "user@ideageek.pk", Role = "User", IsAdmin = false }
    };

            foreach (var user in users)
            {
                var existingUser = await dbConnection.QueryFirstOrDefaultAsync<string>(
                    "SELECT Email FROM AspNetUsers WHERE UserName = @UserName", new { UserName = user.UserName });

                if (existingUser == null)
                {
                    var userId = Guid.NewGuid();

                    var appUser = new AspNetUser
                    {
                        Id = userId,
                        FullName = user.FullName,
                        UserName = user.UserName,
                        NormalizedUserName = user.UserName.ToUpper(),
                        Email = user.Email,
                        NormalizedEmail = user.Email.ToUpper(),
                        EmailConfirmed = true
                    };

                    // Hash the password securely
                    appUser.PasswordHash = _passwordHasher.HashPassword(appUser, "Ideageek123");

                    await dbConnection.ExecuteAsync(
                        "INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash) " +
                        "VALUES (@Id, @UserName, @NormalizedUserName, @Email, @NormalizedEmail, @EmailConfirmed, @PasswordHash)",
                        appUser);

                    await dbConnection.ExecuteAsync(
                        "INSERT INTO AspNetUserRoles (UserId, RoleId) " +
                        "VALUES (@UserId, (SELECT Id FROM AspNetRoles WHERE Name = @Role))",
                        new { UserId = userId, Role = user.Role });

                    _logger.LogInformation($"User '{user.Email}' created and assigned role '{user.Role}'.");
                }
            }
        }

        private async Task SeedGeneralAsync(IDbConnection dbConnection)
        {
            var user = await dbConnection.QueryFirstOrDefaultAsync<Guid>(
                    "SELECT Id FROM AspNetUsers WHERE UserName = @UserName", new { UserName = "admin@ideageek.pk" });

            var generals = new[]
            {
            new { Name = "Active", Group = "Status", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "InActive", Group = "Status", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},

            new { Name = "Sindh", Group = "Province", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "Punjab", Group = "Province", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "Balochistan", Group = "Province", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "KPK", Group = "Province", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},

            new { Name = "Current", Group = "AccountType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "Saving", Group = "AccountType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},

            new { Name = "Pdf", Group = "DocumentList", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},

            new { Name = "Transportation", Group = "PayOrderPurpose", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},

            new { Name = "Filer", Group = "ATLStatus", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "Non-Filer", Group = "ATLStatus", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},

            new { Name = "Processing", Group = "BillingType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "Examination", Group = "BillingType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "Payment", Group = "BillingType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "Delivery", Group = "BillingType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "De-Blocking", Group = "BillingType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "Misc. Transportation", Group = "BillingType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},

            new { Name = "Import", Group = "JobType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "Export", Group = "JobType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},

            new { Name = "By Air", Group = "ShippmentMode", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "By Sea", Group = "ShippmentMode", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},

            new { Name = "LCL", Group = "CargoType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "FCL", Group = "CargoType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "Breakbulk", Group = "CargoType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},

            new { Name = "CFR", Group = "ValueType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "CIF", Group = "ValueType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "FOB", Group = "ValueType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "EXW", Group = "ValueType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "DDP", Group = "ValueType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},

            new { Name = "Arrived", Group = "ShipmentStatus", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "ETA", Group = "ShipmentStatus", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "Cut-off", Group = "ShipmentStatus", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},

            new { Name = "Importer", Group = "ImporterType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "Intermediary", Group = "ImporterType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},

            new { Name = "Custom Duty", Group = "BeneficiaryType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "Excise", Group = "BeneficiaryType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "Shipping Companies", Group = "BeneficiaryType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "Freight Forwarder", Group = "BeneficiaryType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "Terminal", Group = "BeneficiaryType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},

            new { Name = "Government", Group = "TenderType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "Private", Group = "TenderType", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},

            new { Name = "Shipping", Group = "TenderKeyword", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "Freight", Group = "TenderKeyword", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},

            new { Name = "Asset", Group = "AccountNature", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "Liabilities", Group = "AccountNature", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "Expense", Group = "AccountNature", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "Capital", Group = "AccountNature", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "Cash", Group = "AccountNature", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "Bank", Group = "AccountNature", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},

            new { Name = "Meezan Bank", Group = "Bank", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            new { Name = "Hbl Bank", Group = "Bank", Status = true, CreatedBy = user, CreatedOn = DateTime.Now},
            };

            foreach (var general in generals)
            {
                var existingGeneral = await dbConnection.QueryFirstOrDefaultAsync<string>(
                    "SELECT Name FROM General WHERE Name = @Name", new { Name = general.Name });

                if (existingGeneral == null)
                {
                    await dbConnection.ExecuteAsync(
                    "INSERT INTO General (Name, [Group], Status, CreatedBy, CreatedOn) " +
                    "VALUES (@Name, @Group, @Status, @CreatedBy, @CreatedOn)",
                    new General()
                    {
                        Name = general.Name,
                        Group = general.Group,
                        Status = general.Status,
                        CreatedBy = general.CreatedBy,
                        CreatedOn = general.CreatedOn
                    });
                }
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashBytes);
        }
    }
}
