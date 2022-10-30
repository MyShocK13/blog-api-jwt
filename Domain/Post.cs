using System;

namespace blog_api_jwt.Domain;

public class Post
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}