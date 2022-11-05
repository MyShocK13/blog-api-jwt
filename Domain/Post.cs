using System.Collections.Generic;

namespace blog_api_jwt.Domain;

public class Post
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int UserId { get; set; }
    public virtual List<PostTag>? Tags { get; set; }
}