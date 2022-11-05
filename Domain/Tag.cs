using System;

namespace blog_api_jwt.Domain;

public class Tag
{
    public string Name { get; set; } = string.Empty;
    public int CreatorId { get; set; }
    public DateTime CreatedOn { get; set; }
}