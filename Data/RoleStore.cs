using blog_api_jwt.Domain;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace blog_api_jwt.Data;

public class RoleStore : IQueryableRoleStore<Role>, IRoleStore<Role>
{
    private readonly string _connectionString;

    public RoleStore(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public IQueryable<Role> Roles
    {
        get
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var query = $@"SELECT * FROM Roles";

                return connection.Query<Role>(query).AsQueryable();
            }
        }
    }

    public Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public void Dispose()
    {
    }
}