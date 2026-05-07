using Gold.Core.Common;

namespace Gold.Core.Entities;

public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? NationalId { get; set; }

    public ICollection<RepairOrder> Orders { get; set; } = new List<RepairOrder>();
}
