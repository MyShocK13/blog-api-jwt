using System;

namespace blog_api_jwt.Options;

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public TimeSpan TokenLifeTime { get; set; }
}