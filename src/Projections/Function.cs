using System.Text.Json;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Contracts.Constants;
using Contracts.Events;
using Contracts.Exceptions;
using Contracts.Messages;
using MediatR;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Projections;

public class Function
{
    private readonly IMediator _mediator;
    private readonly EventsDictionary _eventsDictionary;
    
    public Function(IMediator mediator, EventsDictionary eventsDictionary)
    {
        _mediator = mediator;
        _eventsDictionary = eventsDictionary;
    }

    [LambdaFunction]
    public async Task<SQSBatchResponse> FunctionHandler(SQSEvent @event, ILambdaContext context)
    {
        var batchItemFailures = new List<SQSBatchResponse.BatchItemFailure>();
        foreach (var message in @event.Records)
        {
            context.Logger.LogInformation($"Processed message {message.Body}");
            var isSuccess = await ProcessMessageAsync(message, context);
            if (isSuccess) continue;
            
            batchItemFailures.Add(new SQSBatchResponse.BatchItemFailure
            {
                ItemIdentifier = message.MessageId
            });
        }
        return new SQSBatchResponse(batchItemFailures);
    }

    private async Task<bool> ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
    {
        var messageType = message.MessageAttributes[AttributesNames.MessageType].StringValue;
        var type = _eventsDictionary.GetMessageType(messageType);

        if (type is null)
        {
            context.Logger.LogWarning($"Unknown message type: {messageType}");
            return false;
        }
        
        var genericType = typeof(SnsMessage<>).MakeGenericType(type);
        
        try
        {
            if (JsonSerializer.Deserialize(message.Body, genericType) is not ISnsMessage eventFromStore)
                throw new UnsupportedTypeException(messageType);
            
            await _mediator.Send(eventFromStore);
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Message failed during processing: {ex.Message}");
            return false;
        }
        
        return true;
    }
}