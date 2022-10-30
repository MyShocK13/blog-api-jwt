using System;

namespace blog_api_jwt.Contracts.v1.Requests;

public class CreatePostRequest
{
    public string Name { get; set; } = string.Empty;
}