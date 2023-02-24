using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using MiWrap;
using Npgsql;
using Queries.Models;
using Settings;

namespace Queries.Features.Categories;

public record GetMainCategories : IHttpQuery;

public class GetMainCategoriesEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapGet<GetMainCategories, GetMainCategoriesHandler>("categories/main")
            .Produces(200);
}

internal class GetMainCategoriesHandler : IHttpQueryHandler<GetMainCategories>
{
    private readonly IDbConnection _connection;
    public GetMainCategoriesHandler(IOptionsMonitor<PostgresSettings> options) 
        => _connection = new NpgsqlConnection(options.CurrentValue.ConnectionString);

    public async Task<IResult> HandleAsync(GetMainCategories query, CancellationToken cancellationToken)
    {
        var result = await _connection
            .QueryAsync<CategoryReadModel>("select * from categories c where c.parent = null");
        
        return Results.Ok(new { MainCategories = result.Select(x => new { x.Id, x.Name })});
    }
}