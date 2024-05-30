namespace Common.Exceptions;

public class Unauthorized : Exception
{
    public Unauthorized(string? message) : base(message) { }

    public Unauthorized(string? message, Exception? innerException) : base(message, innerException) { }
}