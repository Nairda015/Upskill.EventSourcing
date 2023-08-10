using System.Data;
using Contracts.Settings;
using Dapper;
using Microsoft.Extensions.Options;
using MiWrap;
using Npgsql;

namespace Queries.Features.Categories;

internal record GetCategory(Guid Id) : IHttpQuery;

public class GetCategoryEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapGet<GetCategory, GetCategoryHandler>("categories/{id}")
            .Produces<CategoryReadModel>()
            .Produces(400)
            .Produces(404);
}

internal class GetCategoryHandler : IHttpQueryHandler<GetCategory>
{
    private readonly IDbConnection _connection;
    public GetCategoryHandler(IOptionsMonitor<PostgresSettings> options) 
        => _connection = new NpgsqlConnection(options.CurrentValue.ConnectionString);

    public async Task<IResult> HandleAsync(GetCategory query, CancellationToken cancellationToken)
    {
        if (query.Id == Guid.Empty) return Results.BadRequest();
        var product = await _connection
            .QueryFirstAsync<CategoryReadModel>("""select * from "Categories" where "Id" = @Id""", query.Id);

        return product is null
            ? Results.NotFound()
            : Results.Ok(product);
    }
}