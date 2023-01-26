using System.Data;
using Dapper;
using Queries.Models;
using Shared.MiWrap;

namespace Queries.Features.Categories;

public record GetCategoryChildren(Guid Id) : IHttpQuery;

public class GetCategoryChildrenEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapGet<GetCategoryChildren, GetCategoryChildrenHandler>("")
            .Produces(200);
}

internal class GetCategoryChildrenHandler : IHttpQueryHandler<GetCategoryChildren>
{
    private readonly IDbConnection _connection;
    public GetCategoryChildrenHandler(IDbConnection connection) => _connection = connection;

    public async Task<IResult> HandleAsync(GetCategoryChildren query, CancellationToken cancellationToken)
    {
        var result = await _connection
            .QueryAsync<CategoryReadModel>("select * from categories c where c.parent = @Id", query.Id);

        return Results.Ok(new { CategoryChildren = result });
    }
}