using System.Data;
using Dapper;
using Queries.Models;
using Shared.MiWrap;

namespace Queries.Features.Categories;

internal record GetCategory(Guid Id) : IHttpQuery;

public class GetCategoryEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapGet<GetCategory, GetCategoryHandler>("{id}")
            .Produces(200)
            .Produces(400)
            .Produces(404);
}

internal class GetCategoryHandler : IHttpQueryHandler<GetCategory>
{
    private readonly IDbConnection _connection;

    public GetCategoryHandler(IDbConnection connection) => _connection = connection;

    public async Task<IResult> HandleAsync(GetCategory query, CancellationToken cancellationToken)
    {
        if (query.Id == Guid.Empty) return Results.BadRequest();
        var product = await _connection
            .QueryFirstAsync<CategoryReadModel>("select * from Categories where id = @Id", query.Id);

        return product is null
            ? Results.NotFound()
            : Results.Ok(product);
    }
}