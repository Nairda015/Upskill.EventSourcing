using Contracts.Events.Categories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LambdaCategories.Handlers;

public class CategoryCreatedHandler : IRequestHandler<CategoryCreated>
{
    private readonly ILogger<CategoryCreatedHandler> _logger;

    public CategoryCreatedHandler(ILogger<CategoryCreatedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(CategoryCreated request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(request.Name);
        return Unit.Task;
    }
}