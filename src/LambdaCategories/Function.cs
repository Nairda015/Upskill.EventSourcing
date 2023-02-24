using System.Text.Json;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Contracts.Messages;
using MediatR;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaCategories;

public class Function
{
    private readonly IMediator _mediator;
    private readonly SnsMessagesDictionary _snsMessagesDictionary;
    
    public Function(IMediator mediator, SnsMessagesDictionary snsMessagesDictionary)
    {
        _mediator = mediator;
        _snsMessagesDictionary = snsMessagesDictionary;
    }

    [LambdaFunction]
    public async Task<SQSBatchResponse> FunctionHandler(SQSEvent evnt, ILambdaContext context)
    {
        var batchItemFailures = new List<SQSBatchResponse.BatchItemFailure>();
        foreach (var message in evnt.Records)
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
        var messageType = message.MessageAttributes["MessageType"].StringValue;
        var type = _snsMessagesDictionary.GetMessageType(messageType);

        if (type is null)
        {
            context.Logger.LogWarning($"Unknown message type: {messageType}");
            return false;
        }
        
        var genericType = typeof(SnsMessage<>).MakeGenericType(type);
        
        try
        {
            var eventFromStore = (ISnsMessage)JsonSerializer.Deserialize(message.Body, genericType)!;
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