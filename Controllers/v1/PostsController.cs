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

    [HttpPost(ApiRoutes.Posts.Create)]
    public IActionResult Create([FromBody] Post post)
    {
        if (string.IsNullOrEmpty(post.Id))
        {
            post.Id = Guid.NewGuid().ToString();
        }

        _posts.Add(post);

        var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
        var locationUrl = $"{baseUrl}/{ApiRoutes.Posts.Get.Replace("{id}", post.Id)}";

        return Created(locationUrl, post);
    }
}