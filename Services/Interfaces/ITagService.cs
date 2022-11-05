using blog_api_jwt.Domain;
using System.Threading.Tasks;

namespace blog_api_jwt.Services.Interfaces;

public interface ITagService
{
    Task<bool> CreateTagAsync(Tag tag);
    Task<Tag?> GetTagByNameAsync(string name);
}