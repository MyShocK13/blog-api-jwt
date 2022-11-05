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

class TagService : ITagService
{
    private readonly string _connectionString;

    public TagService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<bool> CreateTagAsync(Tag tag)
    {
        using var connection = new SqliteConnection(_connectionString);

        await connection.OpenAsync();

        tag.Name = tag.Name.ToLower();

        var existingTag = await GetTagByNameAsync(tag.Name);

        if (existingTag is not null)
        {
            return true;
        }

        var query = $@"INSERT INTO tags (name, creatorid, createdon)
                       VALUES (@Name, @CreatorId, @CreatedOn)
                       RETURNING name;";

        var created = await connection.QuerySingleAsync<string>(query, tag);
        return created is not null;
    }

    public async Task<Tag?> GetTagByNameAsync(string name)
    {
        using var connection = new SqliteConnection(_connectionString);

        await connection.OpenAsync();

        var query = $@"SELECT *
                       FROM tags
                       WHERE name = @Name";

        var tag = await connection.QuerySingleOrDefaultAsync<Tag>(query, new { Name = name });

        return tag;
    }
}