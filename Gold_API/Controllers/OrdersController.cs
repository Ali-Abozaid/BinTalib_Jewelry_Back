using Gold.Application.DTOs.Orders;
using Gold.Application.Interfaces;
using Gold.Core.Enums;
using Gold.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gold_API.Controllers;

[Authorize]
public class OrdersController : BaseApiController
{
    private readonly IOrderService _orders;
    private readonly ICurrentUserService _currentUser;

    public OrdersController(IOrderService orders, ICurrentUserService currentUser)
    {
        _orders = orders;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var role = _currentUser.Role;
        if (role == UserRoles.Admin)
        {
            return ToActionResult(await _orders.GetAllAsync(ct));
        }
        if (role == UserRoles.Branch && _currentUser.BranchId.HasValue)
        {
            return ToActionResult(await _orders.GetForBranchAsync(_currentUser.BranchId.Value, ct));
        }
        if (role == UserRoles.Workshop && _currentUser.WorkshopId.HasValue)
        {
            return ToActionResult(await _orders.GetForWorkshopAsync(_currentUser.WorkshopId.Value, ct));
        }
        return Forbid();
    }

    [HttpGet("stats")]
    public async Task<IActionResult> Stats(CancellationToken ct)
        => ToActionResult(await _orders.GetStatsAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => ToActionResult(await _orders.GetByIdAsync(id, ct));

    [HttpPost]
    [Authorize(Roles = "Admin,Branch")]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto, CancellationToken ct)
    {
        if (_currentUser.Role == UserRoles.Branch && _currentUser.BranchId.HasValue)
        {
            dto.BranchId = _currentUser.BranchId.Value;
        }
        return ToActionResult(await _orders.CreateAsync(dto, ct));
    }

    [HttpPut("{id:guid}/assign-workshop")]
    [Authorize(Roles = "Admin,Branch")]
    public async Task<IActionResult> AssignWorkshop(Guid id, [FromBody] AssignWorkshopDto dto, CancellationToken ct)
        => ToActionResult(await _orders.AssignWorkshopAsync(id, dto, ct));

    [HttpPut("{id:guid}/workshop")]
    [Authorize(Roles = "Admin,Workshop")]
    public async Task<IActionResult> WorkshopUpdate(Guid id, [FromBody] WorkshopUpdateDto dto, CancellationToken ct)
        => ToActionResult(await _orders.WorkshopUpdateAsync(id, dto, ct));

    [HttpPut("{id:guid}/receive-from-workshop")]
    [Authorize(Roles = "Admin,Branch")]
    public async Task<IActionResult> ReceiveFromWorkshop(Guid id, [FromBody] ReceiveFromWorkshopDto dto, CancellationToken ct)
        => ToActionResult(await _orders.ReceiveFromWorkshopAsync(id, dto, ct));

    [HttpPut("{id:guid}/move-to-external")]
    [Authorize(Roles = "Admin,Branch")]
    public async Task<IActionResult> MoveToExternal(Guid id, [FromBody] MoveToExternalDto dto, CancellationToken ct)
        => ToActionResult(await _orders.MoveToExternalAsync(id, dto, ct));

    [HttpPut("{id:guid}/receive-from-external")]
    [Authorize(Roles = "Admin,Branch")]
    public async Task<IActionResult> ReceiveFromExternal(Guid id, [FromBody] ReceiveFromExternalDto dto, CancellationToken ct)
        => ToActionResult(await _orders.ReceiveFromExternalAsync(id, dto, ct));

    [HttpPost("{id:guid}/verify-otp")]
    [Authorize(Roles = "Admin,Branch")]
    public async Task<IActionResult> VerifyOtp(Guid id, [FromBody] VerifyOtpDto dto, CancellationToken ct)
        => ToActionResult(await _orders.VerifyOtpAndDeliverAsync(id, dto, ct));

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Cancel(Guid id, [FromQuery] string? note, CancellationToken ct)
        => ToActionResult(await _orders.CancelAsync(id, note, ct));
}
