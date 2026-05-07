using Gold.Application.DTOs.Branches;
using Gold.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gold_API.Controllers;

[Authorize]
public class BranchesController : BaseApiController
{
    private readonly IBranchService _service;
    public BranchesController(IBranchService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => ToActionResult(await _service.GetAllAsync(ct));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateBranchDto dto, CancellationToken ct)
        => ToActionResult(await _service.CreateAsync(dto, ct));
}
