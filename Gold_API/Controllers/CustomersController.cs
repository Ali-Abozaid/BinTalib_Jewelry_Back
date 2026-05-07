using Gold.Application.DTOs.Customers;
using Gold.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gold_API.Controllers;

[Authorize]
public class CustomersController : BaseApiController
{
    private readonly ICustomerService _service;
    public CustomersController(ICustomerService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => ToActionResult(await _service.GetAllAsync(ct));

    [HttpGet("by-phone")]
    public async Task<IActionResult> ByPhone([FromQuery] string phone, CancellationToken ct)
        => ToActionResult(await _service.SearchByPhoneAsync(phone, ct));

    [HttpPost]
    [Authorize(Roles = "Admin,Branch")]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto, CancellationToken ct)
        => ToActionResult(await _service.CreateAsync(dto, ct));
}
