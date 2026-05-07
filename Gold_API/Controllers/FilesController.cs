using Gold.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gold_API.Controllers;

[Authorize]
public class FilesController : BaseApiController
{
    private readonly IFileStorageService _storage;
    public FilesController(IFileStorageService storage) => _storage = storage;

    [HttpPost("upload")]
    [Authorize(Roles = "Admin,Branch,Workshop")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string subfolder = "orders", CancellationToken ct = default)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { error = "No file provided" });
        }
        await using var stream = file.OpenReadStream();
        var url = await _storage.SaveAsync(stream, file.FileName, subfolder, ct);
        return Ok(new { url });
    }
}
