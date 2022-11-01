using System.Linq;
using Microsoft.AspNetCore.Http;

namespace blog_api_jwt.Extensions;

public static class GeneralExtensions
{
    public static string GetUserId(this HttpContext httpContext)
    {
        if (httpContext.User == null)
        {
            return string.Empty;
        }

        var userId = httpContext.User.Claims.SingleOrDefault(u => u.Type == "id");
        if (userId == null)
        {
            return string.Empty;
        }

        return userId.Value;
    }
}