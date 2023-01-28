using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using Queries.Models;
using Shared.MiWrap;
using Shared.Settings;

namespace Queries.Features.Categories;

internal record GetCategory(Guid Id) : IHttpQuery;

public class GetCategoryEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapGet<GetCategory, GetCategoryHandler>("categories/{id}")
            .Produces(200)
            .Produces(400)
            .Produces(404);
}

internal class GetCategoryHandler : IHttpQueryHandler<GetCategory>
{
    private readonly IDbConnection _connection;
    public GetCategoryHandler(IOptionsMonitor<PostgresOptions> options) 
        => _connection = new NpgsqlConnection(options.CurrentValue.ConnectionString);

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