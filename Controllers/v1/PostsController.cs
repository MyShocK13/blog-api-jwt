using blog_api_jwt.Contracts.v1;
using blog_api_jwt.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using blog_api_jwt.Contracts.v1.Requests;
using blog_api_jwt.Contracts.v1.Responses;
using blog_api_jwt.Services.Interfaces;
using System.Threading.Tasks;

namespace blog_api_jwt.Controllers.v1;

public class PostsController : Controller
{
    private readonly IPostService _postService;

    public PostsController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpPost(ApiRoutes.Posts.Create)]
    public async Task<IActionResult> Create([FromBody] CreatePostRequest request)
    {
        var post = new Post
        {
            Name = request.Name
        };

        var id = await _postService.CreatePostAsync(post);

        var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
        var locationUrl = $"{baseUrl}/{ApiRoutes.Posts.Get.Replace("{id}", id.ToString())}";

        var response = new PostResponse
        {
            Id = id
        };

        return Created(locationUrl, response);
    }

    [HttpGet(ApiRoutes.Posts.Get)]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        var post = await _postService.GetPostByIdAsync(id);

        if (post is null)
        {
            return NotFound();
        }

        return Ok(post);
    }

    [HttpGet(ApiRoutes.Posts.GetAll)]
    public async Task<IActionResult> GetAll()
    {
        var posts = await _postService.GetPostsAsync();

        return Ok(posts);
    }

    [HttpPut(ApiRoutes.Posts.Update)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePostRequest request)
    {
        var post = new Post
        {
            Id = id,
            Name = request.Name
        };

        var updated = await _postService.UpdatePostAsync(post);

        if (!updated)
        {
            return NotFound();
        }

        return Ok(post);
    }

    [HttpDelete(ApiRoutes.Posts.Delete)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var deleted = await _postService.DeletePostAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}