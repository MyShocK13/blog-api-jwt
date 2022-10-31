using blog_api_jwt.Contracts.v1;
using blog_api_jwt.Contracts.v1.Requests;
using blog_api_jwt.Contracts.v1.Responses;
using blog_api_jwt.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace blog_api_jwt.Controllers.v1;

public class IdentityController : Controller
{
    private readonly IIdentityService _identityService;

    public IdentityController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost(ApiRoutes.Identity.Register)]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
    {
        var authResponse = await _identityService.RegisterAsync(request.Email, request.Password);

        if (!authResponse.Success)
        {
            return BadRequest(new AuthFailedResponse
            {
                Errors = authResponse.Errors
            });
        }

        return Ok(new AuthSuccessResponse
        {
            Token = authResponse.Token
        });
    }
}