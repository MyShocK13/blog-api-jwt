using blog_api_jwt.Domain;
using System.Threading.Tasks;

namespace blog_api_jwt.Services.Interfaces;

public interface IIdentityService
{
    Task<AuthenticationResult> RegisterAsync(string email, string password);
}