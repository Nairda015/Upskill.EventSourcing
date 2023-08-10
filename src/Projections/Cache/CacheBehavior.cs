using Contracts.Messages;
using MediatR;

namespace Projections.Cache;

internal class CacheBehavior<TRequest, TResponse> //: IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ISnsMessage
{
    private readonly CacheService _cache;

    public CacheBehavior(CacheService cache) => _cache = cache;

    /// <summary>
    /// Checks if event is the expected one, if not adds it to cache
    /// Try to process current event and all from cache
    /// </summary>
    /// <param name="request"></param>
    /// <param name="next"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Response from request or null if response was added to cache</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var eventsList = await _cache.GetCachedEvents(request.Metadata);

        if (request.Metadata.Number != eventsList.NextExpectedNumber)
        {
            eventsList.Add(request);
            await _cache.SaveEvent(request.CreateFromMessage());
            await _cache.UpdateStream(eventsList);
            return default!;
        }

        var response = await next();
        eventsList.NextExpectedNumber++;
        
        if (eventsList.ShouldStartProcessing())
        {
            foreach (var @event in eventsList.ExtractReadyToProcess())
            {
                //TODO: Get, Process, Delete event from cache
                
            }
        }
        
        _ = await _cache.UpdateStream(eventsList); //TODO: Add retry policy
        return response;
    }
}