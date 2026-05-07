namespace Gold.Core.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? UserName { get; }
    string? Role { get; }
    Guid? BranchId { get; }
    Guid? WorkshopId { get; }
    bool IsAuthenticated { get; }
}
