using Gold.Core.Common;
using Gold.Core.Enums;

namespace Gold.Core.Entities;

public class RepairOrder : BaseEntity
{
    public string Code { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public Guid BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public Guid? WorkshopId { get; set; }
    public Workshop? Workshop { get; set; }

    public string ReceivingEmployeeName { get; set; } = string.Empty;
    public string? WorkshopCourierName { get; set; }

    public decimal WeightBefore { get; set; }
    public decimal? WeightAfter { get; set; }

    public string? ImageBeforeUrl { get; set; }
    public string? ImageAfterUrl { get; set; }

    public PricingType PricingType { get; set; }
    public decimal? Price { get; set; }

    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeliveryToWorkshopDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? DeliveredToCustomerAt { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Created;

    public bool IsExternal { get; set; }
    public string? ExternalProviderName { get; set; }

    public string? Notes { get; set; }

    public string? CustomerOtpCode { get; set; }
    public DateTime? CustomerOtpSentAt { get; set; }
    public DateTime? CustomerOtpVerifiedAt { get; set; }

    public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
}
