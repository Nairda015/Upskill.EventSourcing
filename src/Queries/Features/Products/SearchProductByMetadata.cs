using Contracts.Constants;
using Contracts.Projections;
using MiWrap;
using OpenSearch.Client;

namespace Queries.Features.Products;

public record SearchProductByMetadata(string Pairs) : IHttpQuery;

public class SearchProductByMetadataEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapGet<SearchProductByMetadata, SearchProductByMetadataHandler>("product/search/metadata/{pairs}")
            .Produces<List<ProductProjection>>()
            .Produces(400);
}

internal class SearchProductByMetadataHandler : IHttpQueryHandler<SearchProductByMetadata>
{
    private readonly IOpenSearchClient _client;

    public SearchProductByMetadataHandler(IOpenSearchClient client) => _client = client;

    public async Task<IResult> HandleAsync(SearchProductByMetadata query, CancellationToken cancellationToken)
    {
        //example: key01:value01,key02:value02
        var filters = query.Pairs
            .Split(",")
            .Select(x => x.Split(':', 2))
            .Select(x => (QueryContainer)new MatchQuery
            {
                Field = $"metadata.{x[0]}",
                Query = x[1]
            })
            .ToList();

        var searchRequest = new SearchRequest
        {
            Query = new BoolQuery
            {
                MinimumShouldMatch = 1,
                Should = filters
            }
        };

        var response = await _client.SearchAsync<ProductProjection>(searchRequest, cancellationToken);

        return response.IsValid ? Results.Ok(response.Documents) : Results.BadRequest();
    }
}