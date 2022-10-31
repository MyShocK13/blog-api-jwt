namespace blog_api_jwt.Contracts.v1.Requests;

public class UserRegistrationRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
