using blog_api_jwt.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace blog_api_jwt.Services.Interfaces;

public interface ITagService
{
    Task AddNewTagsFromPostAsync(Post post);
    Task<bool> CreateTagAsync(Tag tag);
    Task<Tag?> GetTagByNameAsync(string name);
    Task<List<Tag>> GetTagsAsync();
}