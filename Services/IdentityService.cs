using blog_api_jwt.Domain;
using blog_api_jwt.Options;
using blog_api_jwt.Services.Interfaces;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace blog_api_jwt.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;
    private readonly JwtSettings _jwtSettings;
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly string _connectionString;

    public IdentityService(UserManager<User> userManager,
                           JwtSettings jwtSettings,
                           TokenValidationParameters tokenValidationParameters,
                           IConfiguration configuration)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings;
        _tokenValidationParameters = tokenValidationParameters;
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<AuthenticationResult> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return new AuthenticationResult
            {
                Errors = new[] { "User does not exist" }
            };
        }

        var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);

        if (!userHasValidPassword)
        {
            return new AuthenticationResult
            {
                Errors = new[] { "Password is not correct" }
            };
        }

        return await GenerateAuthenticationResultForUserAsync(user);
    }

    public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
    {
        var validatedToken = GetPrincipalFromToken(token);
        if (validatedToken is null)
        {
            return new AuthenticationResult { Errors = new[] { "Invalid token" } };
        }

        var expiryDateUnix = long.Parse(validatedToken.Claims.Single(v => v.Type == JwtRegisteredClaimNames.Exp).Value);
        var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiryDateUnix);
        if (expiryDateTimeUtc > DateTime.UtcNow)
        {
            return new AuthenticationResult { Errors = new[] { "This token hasn't expired yet" } };
        }

        var jti = validatedToken.Claims.Single(v => v.Type == JwtRegisteredClaimNames.Jti).Value;

        var storedRefreshToken = await GetStoredRefreshTokenAsync(refreshToken);

        if (storedRefreshToken is null)
        {
            return new AuthenticationResult { Errors = new[] { "This refresh token does not exist" } };
        }

        if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
        {
            return new AuthenticationResult { Errors = new[] { "This refresh token has expired" } };
        }

        if (storedRefreshToken.Invalidated)
        {
            return new AuthenticationResult { Errors = new[] { "This refresh token has been invalidated" } };
        }

        if (storedRefreshToken.Used)
        {
            return new AuthenticationResult { Errors = new[] { "This refresh token has been used" } };
        }

        if (storedRefreshToken.JwtId != jti)
        {
            return new AuthenticationResult { Errors = new[] { "This refresh token does not match this JWT" } };
        }

        storedRefreshToken.Used = true;

        await UpdateStoredRefreshTokenAsync(storedRefreshToken);

        var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(v => v.Type == "id").Value);
        return await GenerateAuthenticationResultForUserAsync(user);
    }

    public async Task<AuthenticationResult> RegisterAsync(string email, string password)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            return new AuthenticationResult
            {
                Errors = new[] { "User already exists" }
            };
        }

        var newUser = new User
        {
            Email = email,
            UserName = email
        };

        var createdUser = await _userManager.CreateAsync(newUser, password);

        if (!createdUser.Succeeded)
        {
            return new AuthenticationResult
            {
                Errors = createdUser.Errors.Select(e => e.Description)
            };
        }

        return await GenerateAuthenticationResultForUserAsync(newUser);
    }

    private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("id", user.Id.ToString())
                }),
            Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifeTime),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            JwtId = token.Id,
            UserId = user.Id,
            CreationDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddMonths(6)
        };

        await CreateRefreshTokenAsync(refreshToken);

        return new AuthenticationResult
        {
            Success = true,
            Token = tokenHandler.WriteToken(token),
            RefreshToken = refreshToken.Token
        };
    }

    private ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var tokenValidationParameters = _tokenValidationParameters.Clone();
            tokenValidationParameters.ValidateLifetime = false;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

            if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }

    private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
    {
        return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase);
    }

    private async Task<bool> CreateRefreshTokenAsync(RefreshToken refreshToken)
    {
        using var connection = new SqliteConnection(_connectionString);

        await connection.OpenAsync();

        var query = $@"INSERT INTO refreshtoken (token, jwtid, creationdate, expirydate, used, invalidated, userid)
                       VALUES (@Token, @JwtId, @CreationDate, @ExpiryDate, @Used, @Invalidated, @UserId)
                       returning Token";

        var created = await connection.QuerySingleAsync<string>(query, refreshToken);

        return created is not null;
    }

    private async Task<RefreshToken> GetStoredRefreshTokenAsync(string refreshToken)
    {
        using var connection = new SqliteConnection(_connectionString);
        
        await connection.OpenAsync();
        
        var query = $@"SELECT *
                       FROM refreshtoken
                       WHERE token = @RefreshToken";

        var storedRefreshToken = await connection.QuerySingleOrDefaultAsync<RefreshToken>(query, new { RefreshToken = refreshToken });
        
        return storedRefreshToken;
    }

    private async Task<bool> UpdateStoredRefreshTokenAsync(RefreshToken storedRefreshToken)
    {
        using var connection = new SqliteConnection(_connectionString);

        await connection.OpenAsync();

        var query = $@"UPDATE refreshtoken SET
                       used = @Used
                       WHERE token = @Token";

        var updated = await connection.ExecuteAsync(query, storedRefreshToken);
        return updated > 0;
    }


}
