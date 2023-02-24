using Contracts.Events.Products;
using Contracts.Messages;
using MediatR;
using Microsoft.Extensions.Logging;

namespace SqsLambda.Handlers;

public class ProductCreatedHandler : IRequestHandler<SnsMessage<ProductCreated>>
{
    private readonly ILogger<ProductCreatedHandler> _logger;

    public ProductCreatedHandler(ILogger<ProductCreatedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SnsMessage<ProductCreated> request, CancellationToken cancellationToken)
    {
        //check cache for next expected event number
        
        //if next expected event number is not equal to event number from message
        //add event to cache and return
        
        //if next expected event number is equal to event number from message
        //add cache management to mediatR pipeline
        
        _logger.LogInformation("Product created message received with id: {Id}", request.Data.Id.ToString());
        return Unit.Task;
    }
}