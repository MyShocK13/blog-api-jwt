using blog_api_jwt.Domain;
using System;
using System.Collections.Generic;

namespace blog_api_jwt.Services.Interfaces;

public interface IPostService
{
    bool DeletePost(Guid id);
    Post? GetPostById(Guid id);
    List<Post> GetPosts();
    bool UpdatePost(Post post);
}