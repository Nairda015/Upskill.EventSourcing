using Contracts.Constants;
using Contracts.Events.Products;
using Contracts.Messages;
using Contracts.Projections;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenSearch.Net;

namespace Projections.Handlers;

public class ProductCreatedHandler : IRequestHandler<SnsMessage<ProductCreated>>
{
    private readonly ILogger<ProductCreatedHandler> _logger;
    private readonly IOpenSearchLowLevelClient _client;

    public ProductCreatedHandler(ILogger<ProductCreatedHandler> logger, IOpenSearchLowLevelClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task Handle(SnsMessage<ProductCreated> request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("ProductCreated message received with id: {Id}", request.Metadata.StreamId);

        var data = ProductProjection.Create(request.Data);
        var response = await _client.IndexAsync<StringResponse>(Constants.ProductsIndexName, request.Metadata.StreamId,
            PostData.Serializable(data), new IndexRequestParameters(), cancellationToken);

        _logger.LogInformation("Response for message {Id} was: {Body}", request.Data.Id.ToString(), response.Body);
        if (!response.Success) throw new Exception("Failed to index product");
    }
}