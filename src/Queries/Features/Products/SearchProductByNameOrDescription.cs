using Contracts.Constants;
using Contracts.Projections;
using MiWrap;
using OpenSearch.Client;

namespace Queries.Features.Products;

public record SearchProductByNameOrDescription(string Value) : IHttpQuery;

public class SearchProductByNameOrDescriptionEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapGet<SearchProductByNameOrDescription, SearchProductByNameOrDescriptionHandler>("product/search/{value}")
            .Produces<List<ProductProjection>>()
            .Produces(400);
}

internal class SearchProductByNameOrDescriptionHandler : IHttpQueryHandler<SearchProductByNameOrDescription>
{
    private readonly IOpenSearchClient _client;

    public SearchProductByNameOrDescriptionHandler(IOpenSearchClient client) => _client = client;

    public async Task<IResult> HandleAsync(SearchProductByNameOrDescription query, CancellationToken cancellationToken)
    {
        var product = await _client.SearchAsync<ProductProjection>(
            x => x
                .Index(Constants.ProductsIndexName)
                .Size(10)
                .Query(q => 
                    q.Match(m => m
                        .Field(f => f.Name)
                        .Query(query.Value)) ||
                    q.Match(m => m
                        .Field(f => f.Description)
                        .Query(query.Value))),
            cancellationToken);

        return product.IsValid ? Results.Ok(product.Documents) : Results.BadRequest();
    }
}