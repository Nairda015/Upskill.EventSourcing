using System.Text;
using System.Text.Json;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Contracts.Events;
using EventStore.Client;
using Google.Protobuf;
using MediatR;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaCategories;

public class Function
{
    private readonly IMediator _mediator;
    private readonly SqsMessagesDictionary _sqsMessagesDictionary;
    
    public Function(IMediator mediator, SqsMessagesDictionary sqsMessagesDictionary)
    {
        _mediator = mediator;
        _sqsMessagesDictionary = sqsMessagesDictionary;
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
        var type = _sqsMessagesDictionary.GetMessageType(messageType);

        if (type is null)
        {
            context.Logger.LogWarning($"Unknown message type: {messageType}");
            return false;
        }
        
        try
        {
            var eventFromStore = JsonSerializer.Deserialize<EventRecord>(message.Body)!;
            var typedMessage = (IMessage)JsonSerializer.Deserialize(Encoding.UTF8.GetString(eventFromStore.Data.Span), type)!;
            await _mediator.Send(typedMessage);
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Message failed during processing: {ex.Message}");
            return false;
        }
        
        return true;
    }
}