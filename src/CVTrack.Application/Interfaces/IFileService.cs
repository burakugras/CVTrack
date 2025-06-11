namespace CVTrack.Application.Interfaces;

public interface IFileService
{
    Task<string> SaveFileAsync(byte[] content, string fileName);
    Task DeleteFileAsync(string fileName);
    Task<byte[]> GetFileAsync(string fileName);
}
