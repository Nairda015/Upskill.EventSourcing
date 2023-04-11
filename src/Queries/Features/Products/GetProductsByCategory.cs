using System.Data;
using Contracts.Constants;
using Contracts.Projections;
using Dapper;
using MiWrap;
using OpenSearch.Client;

namespace Queries.Features.Products;

public record GetProductsByCategory(Guid CategoryId) : IHttpQuery;

public class GetProductsByCategoryEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapGet<GetProductsByCategory, GetProductsByCategoryHandler>("product/search-by-category/{categoryId}")
            .Produces<List<ProductProjection>>()
            .Produces(400);
}

internal class GetProductsByCategoryHandler : IHttpQueryHandler<GetProductsByCategory>
{
    private readonly IOpenSearchClient _client;

    public GetProductsByCategoryHandler(IOpenSearchClient client) => _client = client;

    public async Task<IResult> HandleAsync(GetProductsByCategory query, CancellationToken cancellationToken)
    {
        var product = await _client.SearchAsync<ProductProjection>(
            x => x
                .Index(Constants.ProductsIndexName)
                .Size(10)
                .Query(q => 
                    q.Match(m => m
                        .Field(f => f.CategoryId)
                        .Query(query.CategoryId.ToString()))),
            cancellationToken);

        return product.IsValid ? Results.Ok(product.Documents) : Results.BadRequest();
    }
}