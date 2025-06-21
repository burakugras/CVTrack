using CVTrack.Application.Interfaces;
using Microsoft.Extensions.Configuration;


namespace CVTrack.Infrastructure.Services;

public class LocalFileService : IFileService
{
    private readonly string _storagePath;

    public LocalFileService(IConfiguration config)
    {
        _storagePath = config["FileSettings:StoragePath"]
                       ?? Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        if (!Directory.Exists(_storagePath))
            Directory.CreateDirectory(_storagePath);
    }

    public async Task<string> SaveFileAsync(byte[] content, string fileName)
    {
        var filePath = Path.Combine(_storagePath, fileName);
        await File.WriteAllBytesAsync(filePath, content);
        return fileName;
    }

    public Task DeleteFileAsync(string fileName)
    {
        var filePath = Path.Combine(_storagePath, fileName);
        if (File.Exists(filePath))
            File.Delete(filePath);
        return Task.CompletedTask;
    }

    public async Task<byte[]> GetFileAsync(string fileName)
    {
        var filePath = Path.Combine(_storagePath, fileName);
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {fileName}");
        return await File.ReadAllBytesAsync(filePath);
    }
}