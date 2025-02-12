using Vasitos.Whisper.Infrastructure.FileValidator.Exceptions;

namespace Vasitos.Whisper.Infrastructure.FileValidator;

public class FileValidator : IFileValidator
{
    public bool FileExists(string? filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty.");
        return File.Exists(filePath);
    }

    public (string[] inputs, string outputPath) ValidatePaths(string[] inputs, string output, bool overrideFile)
    {
        var validatedOutput = ValidateOutputPath(output, overrideFile);
        var validatedInputs = ValidateInputPaths(inputs, validatedOutput);
        return (validatedInputs, validatedOutput);
    }

    public string ValidateOutputPath(string? output, bool overrideFile)
    {
        if (string.IsNullOrEmpty(output))
            throw new ArgumentNullException(nameof(output), "Output file path cannot be null or empty.");

        output = Path.GetFullPath(output);

        EnsureDirectoryPathExists(output);

        if (!overrideFile && FileExists(output)) throw new FileAlreadyExistsException(output);

        return output;
    }

    public string ValidateFileExists(string? filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty.");
        var fileToVerify = Path.GetFullPath(filePath);
        if (!FileExists(fileToVerify))
            throw new FileNotFoundException($"The specified file does not exist: {filePath}");
        return fileToVerify;
    }

    public void EnsureDirectoryExists(string directoryPath)
    {
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
    }

    public void EnsureDirectoryPathExists(string? filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty.");

        var directoryPath = Path.GetDirectoryName(filePath);
        if (string.IsNullOrEmpty(directoryPath))
            throw new ArgumentNullException(nameof(directoryPath), $"Directory path for {filePath} is null or empty.");

        EnsureDirectoryExists(directoryPath);
        ValidateDirectoryPermissions(directoryPath);
    }


    public void DeleteFile(string? filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty.");

        var fullPath = Path.GetFullPath(filePath);

        if (!FileExists(fullPath))
            throw new FileNotFoundException($"The specified file does not exist: {fullPath}");

        try
        {
            File.Delete(fullPath);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UnauthorizedAccessException($"Insufficient permissions to delete the file: {fullPath}", ex);
        }
        catch (IOException ex)
        {
            throw new IOException($"An I/O error occurred while deleting the file: {fullPath}", ex);
        }
    }

    private string[] ValidateInputPaths(string[] inputs, string output)
    {
        if (inputs == null || inputs.Length == 0)
            throw new ArgumentNullException(nameof(inputs), "Input file paths cannot be null or empty.");

        for (var i = 0; i < inputs.Length; i++)
        {
            if (string.IsNullOrEmpty(inputs[i]))
                throw new ArgumentNullException($"Input file path at index {i} is null or empty.");

            var parsedInput = ValidateFileExists(inputs[i]);
            if (parsedInput.Equals(output, StringComparison.InvariantCultureIgnoreCase))
                throw new InputAndOutputFileSameException(parsedInput);
            inputs[i] = parsedInput;
        }

        return inputs;
    }

    private void ValidateDirectoryPermissions(string? directoryPath)
    {
        if (!Directory.Exists(directoryPath))
            throw new DirectoryNotFoundException($"Directory {directoryPath} not found.");

        if (!HasWritePermission(directoryPath))
            throw new UnauthorizedAccessException($"Insufficient permissions to write to directory {directoryPath}.");
    }

    private bool HasWritePermission(string directoryPath)
    {
        try
        {
            var tempFilePath = Path.Combine(directoryPath, Guid.NewGuid() + ".tmp");
            using (var fs = File.Create(tempFilePath))
            {
            }

            File.Delete(tempFilePath);
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }
}