using System.Collections.Generic;

namespace blog_api_jwt.Contracts.v1.Requests;

public class CreatePostRequest
{
    public string Name { get; set; } = string.Empty;
    public IEnumerable<string>? Tags { get; set; }
}