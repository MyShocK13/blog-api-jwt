using blog_api_jwt.Domain;
using blog_api_jwt.Options;
using blog_api_jwt.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
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

    public IdentityService(UserManager<User> userManager, JwtSettings jwtSettings)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings;
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

        return GenerateAuthenticationResultForUser(user);
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

        return GenerateAuthenticationResultForUser(newUser);
    }

    private AuthenticationResult GenerateAuthenticationResultForUser(User user)
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
                    new Claim("id", user.Id)
                }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new AuthenticationResult
        {
            Success = true,
            Token = tokenHandler.WriteToken(token)
        };
    }
}
