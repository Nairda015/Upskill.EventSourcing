using Contracts.Events.Products;
using Contracts.Messages;
using Contracts.Projections;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenSearch.Client;

namespace Projections.Handlers;

public class CategoryChangedHandler : IRequestHandler<SnsMessage<CategoryChanged>>
{
    private readonly ILogger<CategoryChangedHandler> _logger;
    private readonly IOpenSearchClient _client;

    public CategoryChangedHandler(ILogger<CategoryChangedHandler> logger, IOpenSearchClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task Handle(SnsMessage<CategoryChanged> request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CategoryChanged message received with id: {Id}", request.Metadata.StreamId);

        var productResponse = await _client.GetAsync<ProductProjection>(
            request.Metadata.StreamId, 
            x => x,
            cancellationToken);
        _logger.LogVersion(productResponse, request.Metadata);

        var productProjection = productResponse.Source;
        productProjection.Apply(request.Data);

        var response = await _client.UpdateAsync<ProductProjection>(
            request.Metadata.StreamId,
            x => x.Doc(productProjection),
            cancellationToken);

        _logger.LogInformation("Response for message {Id} was: {Body}", request.Metadata.StreamId, response.Result);
        if (response.Result != Result.Updated) throw new Exception("Failed to update product");
    }
}