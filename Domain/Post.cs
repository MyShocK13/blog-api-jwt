namespace blog_api_jwt.Domain;

public class Post
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int UserId { get; set; }
}