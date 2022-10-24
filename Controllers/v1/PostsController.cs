using blog_api_jwt.Contracts.v1;
using blog_api_jwt.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace blog_api_jwt.Controllers.v1;

public class PostsController : Controller
{
    private readonly List<Post> _posts;

    public PostsController()
    {
        _posts = new List<Post>();

        for (int i = 0; i < 5; i++)
        {
            _posts.Add(new Post
            {
                Id = Guid.NewGuid().ToString()
            });
        }
    }

    [HttpGet(ApiRoutes.Posts.GetAll)]
    public IActionResult GetAll()
    {
        return Ok(_posts);
    }
}