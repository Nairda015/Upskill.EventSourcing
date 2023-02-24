namespace Contracts.Exceptions;

public abstract class UpskillException : Exception
{
    protected UpskillException(string message) : base(message)
    {
    }
}