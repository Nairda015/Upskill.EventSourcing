using Contracts.Constants;
using Contracts.Projections;
using MiWrap;
using OpenSearch.Client;

namespace Queries.Features.Products;

public record SearchProductByPriceRange(double Min, double Max) : IHttpQuery;

public class SearchProductByPriceRangeEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapGet<SearchProductByPriceRange, SearchProductByPriceRangeHandler>("product/min/{min}/max/{max}")
            .Produces<List<ProductProjection>>()
            .Produces(400);
}

internal class SearchProductByPriceRangeHandler : IHttpQueryHandler<SearchProductByPriceRange>
{
    private readonly IOpenSearchClient _client;

    public SearchProductByPriceRangeHandler(IOpenSearchClient client) => _client = client;

    public async Task<IResult> HandleAsync(SearchProductByPriceRange query, CancellationToken cancellationToken)
    {
        var product = await _client.SearchAsync<ProductProjection>(
            x => x
                .Index(Constants.ProductsIndexName)
                .Size(10)
                .Query(q => 
                    q.Range(r => r
                        .Field(f => f.Price)
                        .GreaterThanOrEquals(query.Min)
                        .LessThanOrEquals(query.Max))),
            cancellationToken);

        return product.IsValid ? Results.Ok(product.Documents) : Results.BadRequest();
    }
}