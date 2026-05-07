using Gold.Application.Common;
using Gold.Application.DTOs.Auth;
using Gold.Application.Interfaces;
using Gold.Core.Entities;
using Gold.Core.Enums;
using Gold.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gold.Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly AppDbContext _db;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        ITokenService tokenService,
        AppDbContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _db = db;
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null || !user.IsActive)
        {
            return Result<AuthResponseDto>.Failure("Invalid email or password");
        }

        var passOk = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!passOk)
        {
            return Result<AuthResponseDto>.Failure("Invalid email or password");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var (token, expires) = _tokenService.CreateToken(user, roles);

        return Result<AuthResponseDto>.Success(new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expires,
            User = await BuildUserDtoAsync(user, roles)
        });
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default)
    {
        if (!UserRoles.All.Contains(dto.Role))
        {
            return Result<AuthResponseDto>.Failure($"Invalid role: {dto.Role}");
        }

        if (await _userManager.FindByEmailAsync(dto.Email) is not null)
        {
            return Result<AuthResponseDto>.Failure("Email already in use");
        }

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            PhoneNumber = dto.Phone,
            BranchId = dto.Role == UserRoles.Branch ? dto.BranchId : null,
            WorkshopId = dto.Role == UserRoles.Workshop ? dto.WorkshopId : null
        };

        var create = await _userManager.CreateAsync(user, dto.Password);
        if (!create.Succeeded)
        {
            return Result<AuthResponseDto>.Failure(create.Errors.Select(e => e.Description));
        }

        if (!await _roleManager.RoleExistsAsync(dto.Role))
        {
            await _roleManager.CreateAsync(new ApplicationRole(dto.Role));
        }
        await _userManager.AddToRoleAsync(user, dto.Role);

        var roles = await _userManager.GetRolesAsync(user);
        var (token, expires) = _tokenService.CreateToken(user, roles);

        return Result<AuthResponseDto>.Success(new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expires,
            User = await BuildUserDtoAsync(user, roles)
        });
    }

    public async Task<Result<UserDto>> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .Include(u => u.Branch)
            .Include(u => u.Workshop)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null) return Result<UserDto>.Failure("User not found");
        var roles = await _userManager.GetRolesAsync(user);
        return Result<UserDto>.Success(await BuildUserDtoAsync(user, roles));
    }

    private async Task<UserDto> BuildUserDtoAsync(ApplicationUser user, IList<string> roles)
    {
        string? branchName = null;
        string? workshopName = null;

        if (user.BranchId.HasValue)
        {
            branchName = (await _db.Branches.FindAsync(user.BranchId.Value))?.Name;
        }
        if (user.WorkshopId.HasValue)
        {
            workshopName = (await _db.Workshops.FindAsync(user.WorkshopId.Value))?.Name;
        }

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Role = roles.FirstOrDefault() ?? string.Empty,
            BranchId = user.BranchId,
            BranchName = branchName,
            WorkshopId = user.WorkshopId,
            WorkshopName = workshopName
        };
    }
}
