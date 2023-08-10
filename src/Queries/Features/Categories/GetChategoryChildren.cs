using System.Data;
using Contracts.Settings;
using Dapper;
using Microsoft.Extensions.Options;
using MiWrap;
using Npgsql;

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
            .QueryAsync<List<CategoryReadModel>>("""select * from "Categories" c where c."ParentId" = @Id""", query.Id);

        return Results.Ok(new { CategoryChildren = result });
    }
}