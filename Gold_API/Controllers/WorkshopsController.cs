using Gold.Application.DTOs.Workshops;
using Gold.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gold_API.Controllers;

[Authorize]
public class WorkshopsController : BaseApiController
{
    private readonly IWorkshopService _service;
    public WorkshopsController(IWorkshopService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => ToActionResult(await _service.GetAllAsync(ct));

    [HttpPost]
    [Authorize(Roles = "Admin,Branch")]
    public async Task<IActionResult> Create([FromBody] CreateWorkshopDto dto, CancellationToken ct)
        => ToActionResult(await _service.CreateAsync(dto, ct));
}
