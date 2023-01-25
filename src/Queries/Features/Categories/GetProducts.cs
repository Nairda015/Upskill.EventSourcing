using System.Data;
using Dapper;
using Queries.Models;
using Shared.MiWrap;

namespace Queries.Features.Categories;

public record GetCategories : IHttpQuery;


public class GetProductsEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapGet<GetCategories, GetCategoriesHandler>("")
            .Produces(200);
}

internal class GetCategoriesHandler : IHttpQueryHandler<GetCategories>
{
    private readonly IDbConnection _connection;
    public GetCategoriesHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IResult> HandleAsync(GetCategories query, CancellationToken cancellationToken)
    {
        //TODO: Add pagination
        var result = await _connection
            .QueryAsync<CategoryReadModel>("select * from Categories");
        
        return Results.Ok(result ?? Enumerable.Empty<CategoryReadModel>());
    }
}