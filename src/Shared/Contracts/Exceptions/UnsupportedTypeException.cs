namespace Contracts.Exceptions;

public class UnsupportedTypeException : UpskillException
{
    public UnsupportedTypeException(string type)
        : base($"Event {type} is not supported")
    {
    }
}