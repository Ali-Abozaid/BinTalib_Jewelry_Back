using System.Security.Claims;
using Gold.Core.Interfaces;

namespace Gold_API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var raw = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(raw, out var id) ? id : null;
        }
    }

    public string? UserName => User?.FindFirstValue("fullName") ?? User?.Identity?.Name;
    public string? Role => User?.FindFirstValue(ClaimTypes.Role);

    public Guid? BranchId
    {
        get
        {
            var raw = User?.FindFirstValue("branchId");
            return Guid.TryParse(raw, out var id) ? id : null;
        }
    }

    public Guid? WorkshopId
    {
        get
        {
            var raw = User?.FindFirstValue("workshopId");
            return Guid.TryParse(raw, out var id) ? id : null;
        }
    }

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}
