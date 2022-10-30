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
    private readonly string _connectionString;

    public PostService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<int> CreatePostAsync(Post post)
    {
        using var connection = new SqliteConnection(_connectionString);

        await connection.OpenAsync();

        var query = $@"INSERT INTO posts (name)
                       VALUES (@Name)
                       RETURNING id;";

        var id = await connection.QuerySingleAsync<int>(query, post);
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

        var query = $@"SELECT id, name
                       FROM posts
                       WHERE id = @Id";

        var post = await connection.QuerySingleAsync<Post>(query, new { Id = id });
        return post;
    }

    public async Task<List<Post>> GetPostsAsync()
    {
        using var connection = new SqliteConnection(_connectionString);

        await connection.OpenAsync();

        var query = $@"SELECT id, name
                       FROM posts";

        var categories = await connection.QueryAsync<Post>(query);
        return categories.ToList();
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