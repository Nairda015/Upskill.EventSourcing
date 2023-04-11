using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using MiWrap;
using Npgsql;
using Settings;

namespace Queries.Features.Categories;

internal record GetCategoriesByFragment(string Fragment) : IHttpQuery;

public class GetCategoriesByFragmentEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapGet<GetCategoriesByFragment, GetCategoriesByFragmentHandler>("categories/{fragment}")
            .Produces<List<CategoryReadModel>>()
            .Produces(400)
            .Produces(404);
}

internal class GetCategoriesByFragmentHandler : IHttpQueryHandler<GetCategoriesByFragment>
{
    private readonly IDbConnection _connection;
    public GetCategoriesByFragmentHandler(IOptionsMonitor<PostgresSettings> options) 
        => _connection = new NpgsqlConnection(options.CurrentValue.ConnectionString);

    public async Task<IResult> HandleAsync(GetCategoriesByFragment query, CancellationToken cancellationToken)
    {
        if (query.Fragment.Length < 3) return Results.BadRequest();
        var product = await _connection
            .QueryFirstAsync<List<CategoryReadModel>>("""select * from "Categories" c where c."Name" ilike @Fragment limit 10""", query.Fragment);

        return product is null
            ? Results.NotFound()
            : Results.Ok(product);
    }
}