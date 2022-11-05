using blog_api_jwt.Contracts.v1;
using blog_api_jwt.Contracts.v1.Requests;
using blog_api_jwt.Contracts.v1.Responses;
using blog_api_jwt.Domain;
using blog_api_jwt.Extensions;
using blog_api_jwt.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using webapi.Contracts.V1.Responses;

namespace blog_api_jwt.Controllers.v1;

public class TagsController : Controller
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpPost(ApiRoutes.Tags.Create)]
    public async Task<IActionResult> Create([FromBody] CreateTagRequest request)
    {
        var userId = HttpContext.GetUserId();
        if (userId == string.Empty)
        {
            return BadRequest(new { error = "You need to be logged in order to create tags" });
        }

        var tag = new Tag
        {
            Name = request.Name,
            CreatorId = int.Parse(userId),
            CreatedOn = DateTime.UtcNow
        };

        var created = await _tagService.CreateTagAsync(tag);

        created = false;
        
        if (!created)
        {
            return BadRequest(
                new ErrorResponse(
                    new ErrorModel {
                        Message = "Unable to create tag"
                    }
                )
            );
        }

        var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
        var locationUrl = $"{baseUrl}/{ApiRoutes.Tags.Get.Replace("{name}", tag.Name)}";

        var response = new TagResponse
        {
            Name = tag.Name
        };

        return Created(locationUrl, response);
    }
}