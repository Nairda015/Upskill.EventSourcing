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
        //key01:value01,key02:value02
        var metadata = query.Pairs
            .Split(",")
            .Select(x => x.Split(':', 2))
            .ToDictionary(x => x[0], x => x[1]);

        var response = await _client.SearchAsync<ProductProjection>(
            x => x
                .Index(Constants.ProductsIndexName)
                .Size(10)
                .Query(q =>
                    q.Bool(b => b
                        .MinimumShouldMatch(new MinimumShouldMatch(1))
                        .Should(metadata
                            .Select(s => new Func<QueryContainerDescriptor<ProductProjection>, QueryContainer>(d => d
                                .Term(t => t
                                    .Field(f => f.Metadata[s.Key])
                                    .Value(s.Value))))))),
            cancellationToken);

        return response.IsValid ? Results.Ok(response.Documents) : Results.BadRequest();
    }
}