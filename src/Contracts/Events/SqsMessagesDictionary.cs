using System.Reflection;

namespace Contracts.Events;

public class SqsMessagesDictionary
{
    private readonly Dictionary<string, Type> _messageTypes;

    public SqsMessagesDictionary() : this(typeof(SqsMessagesDictionary).Assembly) { }
    public SqsMessagesDictionary(Assembly assembly) => _messageTypes = assembly.DefinedTypes
        .Where(x => typeof(ISqsMessage).IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false })
        .ToDictionary(type => type.Name, type => type.AsType());

    public Type? GetMessageType(string messageType) => _messageTypes.GetValueOrDefault(messageType);
}