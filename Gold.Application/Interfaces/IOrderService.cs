using Gold.Application.Common;
using Gold.Application.DTOs.Orders;

namespace Gold.Application.Interfaces;

public interface IOrderService
{
    Task<Result<List<OrderDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<List<OrderDto>>> GetForBranchAsync(Guid branchId, CancellationToken cancellationToken = default);
    Task<Result<List<OrderDto>>> GetForWorkshopAsync(Guid workshopId, CancellationToken cancellationToken = default);
    Task<Result<OrderDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<OrderStatsDto>> GetStatsAsync(CancellationToken cancellationToken = default);

    Task<Result<OrderDto>> CreateAsync(CreateOrderDto dto, CancellationToken cancellationToken = default);
    Task<Result<OrderDto>> WorkshopUpdateAsync(Guid orderId, WorkshopUpdateDto dto, CancellationToken cancellationToken = default);
    Task<Result<OrderDto>> ReceiveFromWorkshopAsync(Guid orderId, ReceiveFromWorkshopDto dto, CancellationToken cancellationToken = default);
    Task<Result<OrderDto>> MoveToExternalAsync(Guid orderId, MoveToExternalDto dto, CancellationToken cancellationToken = default);
    Task<Result<OrderDto>> ReceiveFromExternalAsync(Guid orderId, ReceiveFromExternalDto dto, CancellationToken cancellationToken = default);
    Task<Result<OrderDto>> VerifyOtpAndDeliverAsync(Guid orderId, VerifyOtpDto dto, CancellationToken cancellationToken = default);
    Task<Result<OrderDto>> CancelAsync(Guid orderId, string? note, CancellationToken cancellationToken = default);
}
