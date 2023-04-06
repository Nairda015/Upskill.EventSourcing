namespace Contracts.Events.Products;

public abstract record MetadataChanged(Dictionary<string, string> Value) : IEvent
{
    public abstract MetadataChanged Apply(Dictionary<string, string> metadata);
}

public record MetadataAdded(Dictionary<string, string> Value) : MetadataChanged(Value)
{
    public override MetadataChanged Apply(Dictionary<string, string> metadata)
    {
        foreach (var (key, value) in metadata)
        {
            if (Value.ContainsKey(key))
            {
                Value[key] = value;
            }
            else
            {
                Value.Add(key, value);
            }
        }

        return this;
    }
}

public record MetadataRemoved(Dictionary<string, string> Value) : MetadataChanged(Value)
{
    public override MetadataChanged Apply(Dictionary<string, string> metadata)
    {
        foreach (var (key, _) in metadata) Value.Remove(key);
        return this;
    }
}
