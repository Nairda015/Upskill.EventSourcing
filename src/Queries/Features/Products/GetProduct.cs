using Contracts.Constants;
using Contracts.Projections;
using MiWrap;
using OpenSearch.Client;

namespace Queries.Features.Products;

internal record GetProduct(Guid Id) : IHttpQuery;

public class GetProductEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapGet<GetProduct, GetProductHandler>("product/{id}")
            .Produces<ProductProjection>()
            .Produces(404);
}

internal class GetProductHandler : IHttpQueryHandler<GetProduct>
{
    private readonly IOpenSearchClient _client;

    public GetProductHandler(IOpenSearchClient client) => _client = client;

    public async Task<IResult> HandleAsync(GetProduct query, CancellationToken cancellationToken)
    {

        var product = await _client.GetAsync<ProductProjection>(
            query.Id,
            x => x.Index(Constants.ProductsIndexName),
            cancellationToken);

        return product is null ? Results.NotFound() : Results.Ok(product);
    }
}