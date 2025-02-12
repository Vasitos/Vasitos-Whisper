namespace Vasitos.Whisper.Infrastructure.FileValidator.Exceptions;

public class FileAlreadyExistsException : FileException
{
    public FileAlreadyExistsException()
    {
    }

    public FileAlreadyExistsException(string message) : base(message)
    {
    }

    public FileAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
    {
    }
}