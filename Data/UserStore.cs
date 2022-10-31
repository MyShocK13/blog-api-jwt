using blog_api_jwt.Domain;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace blog_api_jwt.Data;

public class UserStore : IQueryableUserStore<User>, IUserStore<User>, IUserEmailStore<User>, IUserPhoneNumberStore<User>,
    IUserTwoFactorStore<User>, IUserPasswordStore<User>, IUserRoleStore<User>
{
    private readonly string _connectionString;

    public UserStore(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public IQueryable<User> Users
    {
        get
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var query = $@"SELECT * FROM Users";

                return connection.Query<User>(query).AsQueryable();
            }
        }
    }

    public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);

            var query = $@"INSERT INTO Users (UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled)
                        VALUES(@{nameof(user.UserName)}, @{nameof(user.NormalizedUserName)}, @{nameof(user.Email)}, @{nameof(user.NormalizedEmail)}, @{nameof(user.EmailConfirmed)}, @{nameof(user.PasswordHash)}, @{nameof(user.PhoneNumber)}, @{nameof(user.PhoneNumberConfirmed)}, @{nameof(user.TwoFactorEnabled)})
                        RETURNING Id;";

            user.Id = await connection.QuerySingleAsync<string>(query, user);
        }

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);

            var query = $@"DELETE FROM Users
                        WHERE Id = @{nameof(user.Id)}";

            await connection.ExecuteAsync(query, user);
        }

        return IdentityResult.Success;
    }

    public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);

            var query = $@"SELECT * FROM Users 
                        WHERE Id = @{nameof(userId)}";

            return await connection.QuerySingleOrDefaultAsync<User>(query, new { userId });
        }
    }

    public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);

            var query = $@"SELECT * FROM Users 
                        WHERE NormalizedUserName = @{nameof(normalizedUserName)}";

            return await connection.QuerySingleOrDefaultAsync<User>(query, new { normalizedUserName });
        }
    }

    public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Id.ToString());
    }

    public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.UserName);
    }

    public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName;
        return Task.FromResult(0);
    }

    public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);

            var query = $@"UPDATE Users SET
                        UserName = @{nameof(user.UserName)},
                        NormalizedUserName = @{nameof(user.NormalizedUserName)},
                        Email = @{nameof(user.Email)},
                        NormalizedEmail = @{nameof(user.NormalizedEmail)}, 
                        EmailConfirmed = @{nameof(user.EmailConfirmed)},
                        PasswordHash = @{nameof(user.PasswordHash)},
                        PhoneNumber = @{nameof(user.PhoneNumber)},
                        PhoneNumberConfirmed = @{nameof(user.PhoneNumberConfirmed)},
                        TwoFactorEnabled = @{nameof(user.TwoFactorEnabled)}
                        WHERE Id = @{nameof(user.Id)};";

            await connection.ExecuteAsync(query, user);
        }

        return IdentityResult.Success;
    }

    public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Email);
    }

    public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);

            var query = $@"SELECT * FROM Users 
                        WHERE NormalizedEmail = @{nameof(normalizedEmail)}";

            return await connection.QuerySingleOrDefaultAsync<User>(query, new { normalizedEmail });
        }
    }

    public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
    {
        user.NormalizedEmail = normalizedEmail;
        return Task.FromResult(0);
    }

    public Task SetPhoneNumberAsync(User user, string phoneNumber, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<string> GetPhoneNumberAsync(User user, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<bool> GetPhoneNumberConfirmedAsync(User user, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task SetPhoneNumberConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task SetTwoFactorEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<bool> GetTwoFactorEnabledAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.TwoFactorEnabled);
    }

    public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
    {
        user.PasswordHash = passwordHash;
        return Task.FromResult(0);
    }

    public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash);
    }

    public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash != null);
    }

    public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);

            roleName = roleName.Substring(0, 1) + roleName.Substring(1).ToLower();
            var normalizedName = roleName.ToUpper();
            var query = $@"SELECT Id FROM Roles
                        WHERE NormalizedName = @normalizedName";
            var roleId = await connection.ExecuteScalarAsync<string>(query, new { normalizedName });

            if (roleId == null)
            {
                query = $@"INSERT INTO Roles (Name, NormalizedName)
                        VALUES (@{nameof(roleName)}, @{nameof(normalizedName)})
                        RETURNING Id;";
                roleId = await connection.QuerySingleAsync<string>(query, new { roleName, normalizedName });
            }

            query = $@"INSERT INTO UserRoles (UserId, RoleId)
                    SELECT @userId, @{nameof(roleId)}
                    WHERE NOT EXISTS(
                        SELECT 1 FROM UserRoles
                        WHERE UserId = @userId AND RoleId = @{nameof(roleId)} 
                    );";
            await connection.ExecuteAsync(query, new { userId = user.Id, roleId });
        }
    }

    public Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);

            var query = $@"SELECT r.Name FROM Roles r
                        INNER JOIN UserRoles ur ON ur.RoleId = r.Id
                        WHERE ur.UserId = @userId";

            var roles = await connection.QueryAsync<string>(query, new { userId = user.Id });

            return roles.ToList();
        }
    }

    public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);

            var query = $@"SELECT Id FROM Roles
                        WHERE NormalizedName = @normalizedName";

            var roleId = await connection.ExecuteScalarAsync<string>(query, new { normalizedName = roleName.ToUpper() });

            if (roleId == null)
                return false;

            query = $@"SELECT COUNT(*) FROM UserRoles
                    WHERE UserId = @userId AND RoleId = @{nameof(roleId)}";

            var matchingRoles = await connection.ExecuteScalarAsync<int>(query, new { userId = user.Id, roleId });

            return matchingRoles > 0;
        }
    }

    public Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public void Dispose()
    {
    }
}