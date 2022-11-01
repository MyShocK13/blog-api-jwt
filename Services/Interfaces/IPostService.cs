using blog_api_jwt.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace blog_api_jwt.Services.Interfaces;

public interface IPostService
{
    Task<int> CreatePostAsync(Post post);
    Task<bool> DeletePostAsync(int id);
    Task<Post?> GetPostByIdAsync(int id);
    Task<List<Post>> GetPostsAsync();
    Task<bool> UserOwnsPostAsync(int postId, int userId);
    Task<bool> UpdatePostAsync(Post post);
}