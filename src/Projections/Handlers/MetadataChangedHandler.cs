using System.Text.Json;
using Contracts.Constants;
using Contracts.Events.Products;
using Contracts.Messages;
using Contracts.Projections;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenSearch.Client;

namespace Projections.Handlers;

public class MetadataChangedHandler : IRequestHandler<SnsMessage<MetadataAdded>>, IRequestHandler<SnsMessage<MetadataRemoved>>
{
    private readonly ILogger<MetadataChangedHandler> _logger;
    private readonly IOpenSearchClient _client;

    public MetadataChangedHandler(ILogger<MetadataChangedHandler> logger, IOpenSearchClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task Handle(SnsMessage<MetadataAdded> request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MetadataAdded message received with id: {Id}", request.Metadata.StreamId);

        var productResponse = await _client.GetAsync<ProductProjection>(request.Metadata.StreamId, x => x, cancellationToken);
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

    public async Task Handle(SnsMessage<MetadataRemoved> request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MetadataRemoved message received with id: {Id}", request.Metadata.StreamId);

        var productResponse = await _client.GetAsync<ProductProjection>(
            request.Metadata.StreamId, 
            x => x,
            cancellationToken);
        _logger.LogVersion(productResponse, request.Metadata);

        var productProjection = productResponse.Source;
        productProjection.Apply(request.Data);

        var updateRequest = new UpdateRequest<ProductProjection, ProductProjection>(request.Metadata.StreamId)
        {
            Script = new InlineScript("ctx._source.metadata = params.new_state;ctx._source.version = params.new_version;")
            {
                Params = new Dictionary<string, object>
                {
                    { "new_state", productProjection.Metadata },
                    { "new_version", productProjection.Version },
                }
            }
        };

        var response = await _client.UpdateAsync(updateRequest, cancellationToken);

        _logger.LogInformation("Response for message {Id} was: {Body}", request.Metadata.StreamId, response.Result);
        if (response.Result != Result.Updated) throw new Exception("Failed to update product");
    }
}