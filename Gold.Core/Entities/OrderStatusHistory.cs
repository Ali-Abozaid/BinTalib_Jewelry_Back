using Gold.Core.Common;
using Gold.Core.Enums;

namespace Gold.Core.Entities;

public class OrderStatusHistory : BaseEntity
{
    public Guid RepairOrderId { get; set; }
    public RepairOrder RepairOrder { get; set; } = null!;

    public OrderStatus Status { get; set; }
    public string? Note { get; set; }

    public Guid? ActorUserId { get; set; }
    public string? ActorUserName { get; set; }
    public string? ActorRole { get; set; }

    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}
