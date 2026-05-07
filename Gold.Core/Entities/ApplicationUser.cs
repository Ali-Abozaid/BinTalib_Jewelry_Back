using Microsoft.AspNetCore.Identity;

namespace Gold.Core.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid? WorkshopId { get; set; }
    public Workshop? Workshop { get; set; }
}

public class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole() { }
    public ApplicationRole(string name) : base(name) { }
}
