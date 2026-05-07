using Gold.Application.Common;
using Gold.Application.DTOs.Orders;
using Gold.Application.Interfaces;
using Gold.Application.Mapping;
using Gold.Core.Entities;
using Gold.Core.Enums;
using Gold.Core.Interfaces;

namespace Gold.Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly IOtpService _otpService;

    public OrderService(IUnitOfWork uow, ICurrentUserService currentUser, IOtpService otpService)
    {
        _uow = uow;
        _currentUser = currentUser;
        _otpService = otpService;
    }

    public async Task<Result<List<OrderDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await _uow.Orders.GetAllWithRelationsAsync(cancellationToken);
        return Result<List<OrderDto>>.Success(list.Select(o => o.ToDto()).ToList());
    }

    public async Task<Result<List<OrderDto>>> GetForBranchAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        var list = await _uow.Orders.GetForBranchAsync(branchId, cancellationToken);
        return Result<List<OrderDto>>.Success(list.Select(o => o.ToDto()).ToList());
    }

    public async Task<Result<List<OrderDto>>> GetForWorkshopAsync(Guid workshopId, CancellationToken cancellationToken = default)
    {
        var list = await _uow.Orders.GetForWorkshopAsync(workshopId, cancellationToken);
        return Result<List<OrderDto>>.Success(list.Select(o => o.ToDto()).ToList());
    }

    public async Task<Result<OrderDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _uow.Orders.GetByIdWithRelationsAsync(id, cancellationToken);
        if (order is null)
        {
            return Result<OrderDto>.Failure("Order not found");
        }
        return Result<OrderDto>.Success(order.ToDto());
    }

    public async Task<Result<OrderStatsDto>> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _uow.Orders.GetAllAsync(cancellationToken);
        var stats = new OrderStatsDto
        {
            Total = orders.Count,
            Created = orders.Count(o => o.Status == OrderStatus.Created),
            InProgress = orders.Count(o => o.Status == OrderStatus.InProgress || o.Status == OrderStatus.SentToWorkshop),
            Completed = orders.Count(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.ReceivedFromWorkshop || o.Status == OrderStatus.ReceivedFromExternal),
            Delivered = orders.Count(o => o.Status == OrderStatus.DeliveredToCustomer),
            External = orders.Count(o => o.Status == OrderStatus.SentToExternal || o.IsExternal)
        };
        return Result<OrderStatsDto>.Success(stats);
    }

    public async Task<Result<OrderDto>> CreateAsync(CreateOrderDto dto, CancellationToken cancellationToken = default)
    {
        var branch = await _uow.Branches.GetByIdAsync(dto.BranchId, cancellationToken);
        if (branch is null)
        {
            return Result<OrderDto>.Failure("Branch not found");
        }

        var customer = await _uow.Customers.GetByPhoneAsync(dto.CustomerPhone, cancellationToken);
        if (customer is null)
        {
            customer = new Customer
            {
                Name = dto.CustomerName,
                Phone = dto.CustomerPhone,
                Email = dto.CustomerEmail
            };
            await _uow.Customers.AddAsync(customer, cancellationToken);
        }

        Workshop? workshop = null;
        if (dto.WorkshopId.HasValue)
        {
            workshop = await _uow.Workshops.GetByIdAsync(dto.WorkshopId.Value, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(dto.NewWorkshopName))
        {
            workshop = await _uow.Workshops.GetByNameAsync(dto.NewWorkshopName.Trim(), cancellationToken);
            if (workshop is null)
            {
                workshop = new Workshop { Name = dto.NewWorkshopName.Trim() };
                await _uow.Workshops.AddAsync(workshop, cancellationToken);
            }
        }

        var totalCount = await _uow.Orders.CountAsync(cancellationToken);
        var code = $"JR-{DateTime.UtcNow:yyyy}-{(totalCount + 1):D4}";

        var order = new RepairOrder
        {
            Code = code,
            Customer = customer,
            CustomerId = customer.Id,
            Branch = branch,
            BranchId = branch.Id,
            Workshop = workshop,
            WorkshopId = workshop?.Id,
            ReceivingEmployeeName = dto.ReceivingEmployeeName,
            WorkshopCourierName = dto.WorkshopCourierName,
            WeightBefore = dto.WeightBefore,
            ImageBeforeUrl = dto.ImageBeforeUrl,
            PricingType = dto.PricingType,
            Price = dto.PricingType == PricingType.Workshop ? null : dto.Price,
            DeliveryToWorkshopDate = dto.DeliveryToWorkshopDate,
            Notes = dto.Notes,
            Status = OrderStatus.Created
        };

        order.StatusHistory.Add(BuildHistory(order, OrderStatus.Created, "Order created at branch"));

        if (workshop is not null)
        {
            order.Status = OrderStatus.SentToWorkshop;
            order.StatusHistory.Add(BuildHistory(order, OrderStatus.SentToWorkshop,
                $"Sent to workshop {workshop.Name} (courier: {dto.WorkshopCourierName ?? "-"})"));
        }

        await _uow.Orders.AddAsync(order, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var saved = await _uow.Orders.GetByIdWithRelationsAsync(order.Id, cancellationToken);
        return Result<OrderDto>.Success(saved!.ToDto());
    }

    public async Task<Result<OrderDto>> WorkshopUpdateAsync(Guid orderId, WorkshopUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var order = await _uow.Orders.GetByIdWithRelationsAsync(orderId, cancellationToken);
        if (order is null)
        {
            return Result<OrderDto>.Failure("Order not found");
        }

        if (order.Status is OrderStatus.DeliveredToCustomer or OrderStatus.Cancelled or OrderStatus.SentToExternal)
        {
            return Result<OrderDto>.Failure("Order is no longer in workshop flow");
        }

        if (dto.Status is not (OrderStatus.InProgress or OrderStatus.Completed))
        {
            return Result<OrderDto>.Failure("Workshop can only set status to InProgress or Completed");
        }

        if (order.PricingType == PricingType.Workshop && dto.Price.HasValue)
        {
            order.Price = dto.Price;
        }

        if (dto.WeightAfter.HasValue) order.WeightAfter = dto.WeightAfter;
        if (!string.IsNullOrWhiteSpace(dto.ImageAfterUrl)) order.ImageAfterUrl = dto.ImageAfterUrl;

        order.Status = dto.Status;
        if (dto.Status == OrderStatus.Completed)
        {
            order.CompletedAt = DateTime.UtcNow;
        }

        order.StatusHistory.Add(BuildHistory(order, dto.Status,
            dto.Note ?? (dto.Status == OrderStatus.InProgress ? "Workshop started repair" : "Workshop completed repair")));

        _uow.Orders.Update(order);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<OrderDto>.Success(order.ToDto());
    }

    public async Task<Result<OrderDto>> ReceiveFromWorkshopAsync(Guid orderId, ReceiveFromWorkshopDto dto, CancellationToken cancellationToken = default)
    {
        var order = await _uow.Orders.GetByIdWithRelationsAsync(orderId, cancellationToken);
        if (order is null) return Result<OrderDto>.Failure("Order not found");

        if (order.Status is not (OrderStatus.Completed or OrderStatus.InProgress))
        {
            return Result<OrderDto>.Failure("Order is not ready to receive from workshop");
        }

        if (dto.WeightAfter.HasValue) order.WeightAfter = dto.WeightAfter;
        if (!string.IsNullOrWhiteSpace(dto.ImageAfterUrl)) order.ImageAfterUrl = dto.ImageAfterUrl;

        order.Status = OrderStatus.ReceivedFromWorkshop;
        order.StatusHistory.Add(BuildHistory(order, OrderStatus.ReceivedFromWorkshop,
            dto.Note ?? "Branch received the piece back from workshop"));

        var otp = _otpService.GenerateOtp();
        order.CustomerOtpCode = otp;
        order.CustomerOtpSentAt = DateTime.UtcNow;
        await _otpService.SendWhatsAppOtpAsync(order.Customer.Phone, order.Customer.Name, order.Code, otp, cancellationToken);

        _uow.Orders.Update(order);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<OrderDto>.Success(order.ToDto());
    }

    public async Task<Result<OrderDto>> MoveToExternalAsync(Guid orderId, MoveToExternalDto dto, CancellationToken cancellationToken = default)
    {
        var order = await _uow.Orders.GetByIdWithRelationsAsync(orderId, cancellationToken);
        if (order is null) return Result<OrderDto>.Failure("Order not found");

        if (order.Status is OrderStatus.DeliveredToCustomer or OrderStatus.Cancelled)
        {
            return Result<OrderDto>.Failure("Order is closed");
        }

        var wasInWorkshop = order.Status is OrderStatus.SentToWorkshop or OrderStatus.InProgress or OrderStatus.Completed;

        order.IsExternal = true;
        order.ExternalProviderName = dto.ExternalProviderName;
        order.Status = OrderStatus.SentToExternal;

        var note = wasInWorkshop
            ? $"Workshop process cancelled and item moved to external provider: {dto.ExternalProviderName}"
            : $"Item moved to external provider: {dto.ExternalProviderName}";

        if (!string.IsNullOrWhiteSpace(dto.Note))
        {
            note = $"{note}. {dto.Note}";
        }

        order.StatusHistory.Add(BuildHistory(order, OrderStatus.SentToExternal, note));

        _uow.Orders.Update(order);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<OrderDto>.Success(order.ToDto());
    }

    public async Task<Result<OrderDto>> ReceiveFromExternalAsync(Guid orderId, ReceiveFromExternalDto dto, CancellationToken cancellationToken = default)
    {
        var order = await _uow.Orders.GetByIdWithRelationsAsync(orderId, cancellationToken);
        if (order is null) return Result<OrderDto>.Failure("Order not found");

        if (order.Status != OrderStatus.SentToExternal)
        {
            return Result<OrderDto>.Failure("Order is not in external repair flow");
        }

        if (dto.WeightAfter.HasValue) order.WeightAfter = dto.WeightAfter;
        if (!string.IsNullOrWhiteSpace(dto.ImageAfterUrl)) order.ImageAfterUrl = dto.ImageAfterUrl;
        if (dto.Price.HasValue) order.Price = dto.Price;

        order.Status = OrderStatus.ReceivedFromExternal;
        order.CompletedAt = DateTime.UtcNow;
        order.StatusHistory.Add(BuildHistory(order, OrderStatus.ReceivedFromExternal,
            dto.Note ?? "Branch received the piece back from external provider"));

        var otp = _otpService.GenerateOtp();
        order.CustomerOtpCode = otp;
        order.CustomerOtpSentAt = DateTime.UtcNow;
        await _otpService.SendWhatsAppOtpAsync(order.Customer.Phone, order.Customer.Name, order.Code, otp, cancellationToken);

        _uow.Orders.Update(order);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<OrderDto>.Success(order.ToDto());
    }

    public async Task<Result<OrderDto>> VerifyOtpAndDeliverAsync(Guid orderId, VerifyOtpDto dto, CancellationToken cancellationToken = default)
    {
        var order = await _uow.Orders.GetByIdWithRelationsAsync(orderId, cancellationToken);
        if (order is null) return Result<OrderDto>.Failure("Order not found");

        if (order.Status is not (OrderStatus.ReceivedFromWorkshop or OrderStatus.ReceivedFromExternal))
        {
            return Result<OrderDto>.Failure("Order is not ready for customer pickup");
        }

        if (string.IsNullOrEmpty(order.CustomerOtpCode) || order.CustomerOtpCode != dto.Otp)
        {
            return Result<OrderDto>.Failure("Invalid OTP");
        }

        order.CustomerOtpVerifiedAt = DateTime.UtcNow;
        order.DeliveredToCustomerAt = DateTime.UtcNow;
        order.Status = OrderStatus.DeliveredToCustomer;
        order.StatusHistory.Add(BuildHistory(order, OrderStatus.DeliveredToCustomer, "Customer received item (OTP verified)"));

        _uow.Orders.Update(order);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<OrderDto>.Success(order.ToDto());
    }

    public async Task<Result<OrderDto>> CancelAsync(Guid orderId, string? note, CancellationToken cancellationToken = default)
    {
        var order = await _uow.Orders.GetByIdWithRelationsAsync(orderId, cancellationToken);
        if (order is null) return Result<OrderDto>.Failure("Order not found");

        order.Status = OrderStatus.Cancelled;
        order.StatusHistory.Add(BuildHistory(order, OrderStatus.Cancelled, note ?? "Order cancelled"));

        _uow.Orders.Update(order);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<OrderDto>.Success(order.ToDto());
    }

    private OrderStatusHistory BuildHistory(RepairOrder order, OrderStatus status, string note) => new()
    {
        RepairOrder = order,
        Status = status,
        Note = note,
        ActorUserId = _currentUser.UserId,
        ActorUserName = _currentUser.UserName,
        ActorRole = _currentUser.Role,
        OccurredAt = DateTime.UtcNow
    };
}
