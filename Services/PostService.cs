using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog_api_jwt.Domain;
using blog_api_jwt.Services.Interfaces;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace blog_api_jwt.Services;

class PostService : IPostService
{
    private readonly ITagService _tagService;
    private readonly string _connectionString;

    public PostService(ITagService tagService,
                       IConfiguration configuration)
    {
        _tagService = tagService;
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<int> CreatePostAsync(Post post)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var query = $@"INSERT INTO posts (name, userid)
                       VALUES (@Name, @UserId)
                       RETURNING id;";

        var id = await connection.QuerySingleAsync<int>(query, post);

        post.Id = id;

        await _tagService.AddNewTagsFromPostAsync(post);

        return id;
    }

    public async Task<bool> DeletePostAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);

        await connection.OpenAsync();

        var query = $@"DELETE FROM posts
                       WHERE id = @Id";

        var deleted = await connection.ExecuteAsync(query, new { Id = id });
        return deleted == 1;
    }

    public async Task<Post?> GetPostByIdAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);

        await connection.OpenAsync();

        var query = $@"SELECT id, name, userid
                       FROM posts
                       WHERE id = @Id";

        var post = await connection.QuerySingleAsync<Post>(query, new { Id = id });
        return post;
    }

    public async Task<List<Post>> GetPostsAsync()
    {
        using var connection = new SqliteConnection(_connectionString);

        await connection.OpenAsync();

        var query = $@"SELECT id, name, userid
                       FROM posts";

        var categories = await connection.QueryAsync<Post>(query);
        return categories.ToList();
    }

    public async Task<bool> UserOwnsPostAsync(int postId, int userId)
    {
        var post = await GetPostByIdAsync(postId);

        if (post is null)
        {
            return false;
        }

        if (post.UserId != userId)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> UpdatePostAsync(Post post)
    {
        using var connection = new SqliteConnection(_connectionString);

        await connection.OpenAsync();

        var query = $@"UPDATE posts SET
                       name = @Name
                       WHERE id = @Id";

        var updated = await connection.ExecuteAsync(query, post);
        return updated > 0;
    }
}