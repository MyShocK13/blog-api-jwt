namespace blog_api_jwt.Domain;

public class PostTag
{
    public virtual Tag Tag { get; set; } = new Tag();
    public string TagName { get; set; } = string.Empty;
    public virtual Post Post { get; set; } = new Post();
    public int PostId { get; set; }
}
