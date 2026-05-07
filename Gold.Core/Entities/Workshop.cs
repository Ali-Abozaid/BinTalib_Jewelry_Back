using Gold.Core.Common;

namespace Gold.Core.Entities;

public class Workshop : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    public ICollection<RepairOrder> Orders { get; set; } = new List<RepairOrder>();
}
