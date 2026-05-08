using Gold.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Gold.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _rootPath;
    private readonly string _publicBaseUrl;

    public LocalFileStorageService(IConfiguration configuration, IWebHostEnvironment environment)
    {
        var defaultRoot = Path.Combine(
            string.IsNullOrEmpty(environment.WebRootPath)
                ? Path.Combine(environment.ContentRootPath, "wwwroot")
                : environment.WebRootPath,
            "uploads");

        _rootPath = configuration["Storage:RootPath"] ?? defaultRoot;
        _publicBaseUrl = configuration["Storage:PublicBaseUrl"] ?? "/uploads";

        try
        {
            Directory.CreateDirectory(_rootPath);
        }
        catch
        {
            // Ignored: directory may be read-only on shared hosts; uploads will fail gracefully when used.
        }
    }

    public async Task<string> SaveAsync(Stream content, string originalFileName, string subfolder, CancellationToken cancellationToken = default)
    {
        var folder = Path.Combine(_rootPath, subfolder);
        Directory.CreateDirectory(folder);

        var ext = Path.GetExtension(originalFileName);
        var name = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(folder, name);

        await using (var fs = File.Create(fullPath))
        {
            await content.CopyToAsync(fs, cancellationToken);
        }

        var url = $"{_publicBaseUrl.TrimEnd('/')}/{subfolder}/{name}".Replace("\\", "/");
        return url;
    }

    public Task<bool> DeleteAsync(string relativeUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var trimmed = relativeUrl.Replace(_publicBaseUrl, string.Empty).TrimStart('/', '\\');
            var full = Path.Combine(_rootPath, trimmed);
            if (File.Exists(full))
            {
                File.Delete(full);
                return Task.FromResult(true);
            }
        }
        catch
        {
        }
        return Task.FromResult(false);
    }
}
