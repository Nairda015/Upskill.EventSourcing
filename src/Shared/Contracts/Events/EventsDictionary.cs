using System.Reflection;

namespace Contracts.Events;

public class EventsDictionary
{
    private readonly Dictionary<string, Type> _messageTypes;

    public EventsDictionary() : this(typeof(EventsDictionary).Assembly) { }
    public EventsDictionary(Assembly assembly) => _messageTypes = assembly.DefinedTypes
        .Where(x => typeof(IEvent).IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false })
        .ToDictionary(type => type.Name, type => type.AsType());

    public Type? GetMessageType(string messageType) => _messageTypes.GetValueOrDefault(messageType);
}