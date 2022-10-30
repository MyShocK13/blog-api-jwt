using System;
using System.Collections.Generic;
using System.Linq;
using blog_api_jwt.Domain;
using blog_api_jwt.Services.Interfaces;

namespace blog_api_jwt.Services;

class PostService : IPostService
{
    private readonly List<Post> _posts;

    public PostService()
    {
        _posts = new List<Post>();

        for (int i = 0; i < 5; i++)
        {
            _posts.Add(new Post
            {
                Id = Guid.NewGuid(),
                Name = $"Post Name {i}"
            });
        }
    }

    public Post? GetPostById(Guid id)
    {
        return _posts.SingleOrDefault(p => p.Id == id);
    }

    public List<Post> GetPosts()
    {
        return _posts;
    }

    public bool UpdatePost(Post post)
    {
        var exists = GetPostById(post.Id) != null;

        if (!exists)
            return false;

        var index = _posts.FindIndex(p => p.Id == post.Id);
        _posts[index] = post;

        return true;
    }
}