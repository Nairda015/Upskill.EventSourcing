namespace Contracts.Events.Products;

//Important: name for this events should be MetadataChanged
public abstract record MetadataChanged(Dictionary<string, string> Metadata) : IEvent
{
    public abstract MetadataChanged Apply(Dictionary<string, string> metadata);
}

public record MetadataAdded(Dictionary<string, string> Metadata) : MetadataChanged(Metadata)
{
    public override MetadataChanged Apply(Dictionary<string, string> metadata)
    {
        foreach (var (key, value) in metadata)
        {
            if (Metadata.ContainsKey(key))
            {
                Metadata[key] = value;
            }
            else
            {
                Metadata.Add(key, value);
            }
        }

        return this;
    }
}

public record MetadataRemoved(Dictionary<string, string> Metadata) : MetadataChanged(Metadata)
{
    public override MetadataChanged Apply(Dictionary<string, string> metadata)
    {
        foreach (var (key, _) in metadata) Metadata.Remove(key);
        return this;
    }
}
