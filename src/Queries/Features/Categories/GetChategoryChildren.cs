using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using MiWrap;
using Npgsql;
using Queries.Models;
using Settings;

namespace Queries.Features.Categories;

public record GetCategoryChildren(Guid Id) : IHttpQuery;

public class GetCategoryChildrenEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapGet<GetCategoryChildren, GetCategoryChildrenHandler>("categories/{id}/children")
            .Produces(200);
}

internal class GetCategoryChildrenHandler : IHttpQueryHandler<GetCategoryChildren>
{
    private readonly IDbConnection _connection;
    public GetCategoryChildrenHandler(IOptionsMonitor<PostgresSettings> options) 
        => _connection = new NpgsqlConnection(options.CurrentValue.ConnectionString);

    public async Task<IResult> HandleAsync(GetCategoryChildren query, CancellationToken cancellationToken)
    {
        var result = await _connection
            .QueryAsync<CategoryReadModel>("select * from categories c where c.parent = @Id", query.Id);

        return Results.Ok(new { CategoryChildren = result });
    }
}