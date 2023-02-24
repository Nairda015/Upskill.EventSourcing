using System.Reflection;

namespace Contracts.Messages;

public class SnsMessagesDictionary
{
    private readonly Dictionary<string, Type> _messageTypes;

    public SnsMessagesDictionary() : this(typeof(SnsMessagesDictionary).Assembly) { }
    public SnsMessagesDictionary(Assembly assembly) => _messageTypes = assembly.DefinedTypes
        .Where(x => typeof(ISnsMessage).IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false })
        .ToDictionary(type => type.Name, type => type.AsType());

    public Type? GetMessageType(string messageType) => _messageTypes.GetValueOrDefault(messageType);
}