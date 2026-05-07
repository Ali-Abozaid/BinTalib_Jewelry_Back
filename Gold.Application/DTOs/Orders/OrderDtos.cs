using Gold.Application.DTOs.Branches;
using Gold.Application.DTOs.Customers;
using Gold.Application.DTOs.Workshops;
using Gold.Core.Enums;

namespace Gold.Application.DTOs.Orders;

public class OrderDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;

    public CustomerDto Customer { get; set; } = new();
    public BranchDto Branch { get; set; } = new();
    public WorkshopDto? Workshop { get; set; }

    public string ReceivingEmployeeName { get; set; } = string.Empty;
    public string? WorkshopCourierName { get; set; }

    public decimal WeightBefore { get; set; }
    public decimal? WeightAfter { get; set; }
    public string? ImageBeforeUrl { get; set; }
    public string? ImageAfterUrl { get; set; }

    public PricingType PricingType { get; set; }
    public decimal? Price { get; set; }

    public DateTime ReceivedAt { get; set; }
    public DateTime? DeliveryToWorkshopDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? DeliveredToCustomerAt { get; set; }

    public OrderStatus Status { get; set; }
    public string StatusName => Status.ToString();

    public bool IsExternal { get; set; }
    public string? ExternalProviderName { get; set; }
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
    public List<OrderStatusHistoryDto> StatusHistory { get; set; } = new();
}

public class OrderStatusHistoryDto
{
    public OrderStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public string? Note { get; set; }
    public string? ActorUserName { get; set; }
    public string? ActorRole { get; set; }
    public DateTime OccurredAt { get; set; }
}

public class CreateOrderDto
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }

    public Guid BranchId { get; set; }
    public string ReceivingEmployeeName { get; set; } = string.Empty;

    public Guid? WorkshopId { get; set; }
    public string? NewWorkshopName { get; set; }
    public string? WorkshopCourierName { get; set; }

    public decimal WeightBefore { get; set; }
    public string? ImageBeforeUrl { get; set; }

    public PricingType PricingType { get; set; }
    public decimal? Price { get; set; }

    public DateTime? DeliveryToWorkshopDate { get; set; }
    public string? Notes { get; set; }
}

public class WorkshopUpdateDto
{
    public OrderStatus Status { get; set; }
    public decimal? Price { get; set; }
    public decimal? WeightAfter { get; set; }
    public string? ImageAfterUrl { get; set; }
    public string? Note { get; set; }
}

public class ReceiveFromWorkshopDto
{
    public decimal? WeightAfter { get; set; }
    public string? ImageAfterUrl { get; set; }
    public string? Note { get; set; }
}

public class MoveToExternalDto
{
    public string ExternalProviderName { get; set; } = string.Empty;
    public string? Note { get; set; }
}

public class ReceiveFromExternalDto
{
    public decimal? WeightAfter { get; set; }
    public string? ImageAfterUrl { get; set; }
    public decimal? Price { get; set; }
    public string? Note { get; set; }
}

public class VerifyOtpDto
{
    public string Otp { get; set; } = string.Empty;
}

public class OrderStatsDto
{
    public int Total { get; set; }
    public int Created { get; set; }
    public int InProgress { get; set; }
    public int Completed { get; set; }
    public int Delivered { get; set; }
    public int External { get; set; }
}
