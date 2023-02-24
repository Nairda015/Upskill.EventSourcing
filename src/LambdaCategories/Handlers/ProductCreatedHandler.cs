using Contracts.Events.Products;
using Contracts.Messages;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LambdaCategories.Handlers;

public class ProductCreatedHandler : IRequestHandler<SnsMessage<ProductCreated>>
{
    private readonly ILogger<ProductCreatedHandler> _logger;

    public ProductCreatedHandler(ILogger<ProductCreatedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SnsMessage<ProductCreated> request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Product created message received with id: {Id}", request.Data.Id.ToString());
        return Unit.Task;
    }
}