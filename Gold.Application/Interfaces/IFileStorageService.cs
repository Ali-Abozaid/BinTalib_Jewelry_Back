namespace Gold.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveAsync(Stream content, string originalFileName, string subfolder, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string relativeUrl, CancellationToken cancellationToken = default);
}
