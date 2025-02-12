namespace Vasitos.Whisper.Infrastructure.FileValidator.Exceptions;

public class InputAndOutputFileSameException : FileException
{
    public InputAndOutputFileSameException()
    {
    }

    public InputAndOutputFileSameException(string message) : base(message)
    {
    }

    public InputAndOutputFileSameException(string message, Exception innerException) : base(message, innerException)
    {
    }
}