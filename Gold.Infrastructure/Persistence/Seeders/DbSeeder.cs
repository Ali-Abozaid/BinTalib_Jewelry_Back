using Gold.Core.Entities;
using Gold.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gold.Infrastructure.Persistence.Seeders;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;
        var db = sp.GetRequiredService<AppDbContext>();
        var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = sp.GetRequiredService<RoleManager<ApplicationRole>>();
        var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");

        await db.Database.MigrateAsync();

        foreach (var role in UserRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new ApplicationRole(role));
            }
        }

        Branch? mainBranch = null;
        if (!await db.Branches.AnyAsync())
        {
            mainBranch = new Branch { Name = "Bin Talib - Nasr City", Address = "Cairo, Nasr City", Phone = "+201000000001" };
            var b2 = new Branch { Name = "Bin Talib - Heliopolis", Address = "Cairo, Heliopolis", Phone = "+201000000002" };
            var b3 = new Branch { Name = "Bin Talib - Alexandria", Address = "Alexandria", Phone = "+201000000003" };
            await db.Branches.AddRangeAsync(mainBranch, b2, b3);
            await db.SaveChangesAsync();
        }
        else
        {
            mainBranch = await db.Branches.FirstAsync();
        }

        Workshop? mainWorkshop = null;
        if (!await db.Workshops.AnyAsync())
        {
            mainWorkshop = new Workshop { Name = "Bin Talib Master Workshop", Address = "Cairo Workshop Hub", Phone = "+201111111111" };
            var w2 = new Workshop { Name = "Al-Nour Jewelry Workshop", Address = "Cairo", Phone = "+201111111112" };
            await db.Workshops.AddRangeAsync(mainWorkshop, w2);
            await db.SaveChangesAsync();
        }
        else
        {
            mainWorkshop = await db.Workshops.FirstAsync();
        }

        await EnsureUserAsync(userManager, logger,
            email: "admin@bintalib.com",
            password: "Admin@12345",
            fullName: "System Administrator",
            role: UserRoles.Admin);

        await EnsureUserAsync(userManager, logger,
            email: "branch@bintalib.com",
            password: "Branch@12345",
            fullName: "Branch Manager",
            role: UserRoles.Branch,
            branchId: mainBranch?.Id);

        await EnsureUserAsync(userManager, logger,
            email: "workshop@bintalib.com",
            password: "Workshop@12345",
            fullName: "Workshop Lead",
            role: UserRoles.Workshop,
            workshopId: mainWorkshop?.Id);

        if (!await db.Customers.AnyAsync())
        {
            await db.Customers.AddRangeAsync(
                new Customer { Name = "Mona Adel", Phone = "+201111001001", Email = "mona@example.com" },
                new Customer { Name = "Omar Khaled", Phone = "+201222442211" },
                new Customer { Name = "Nour Hassan", Phone = "+201099833221" }
            );
            await db.SaveChangesAsync();
        }
    }

    private static async Task EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        ILogger logger,
        string email,
        string password,
        string fullName,
        string role,
        Guid? branchId = null,
        Guid? workshopId = null)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null)
        {
            return;
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            EmailConfirmed = true,
            BranchId = branchId,
            WorkshopId = workshopId,
            IsActive = true
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            logger.LogError("Failed to seed user {Email}: {Errors}", email,
                string.Join(", ", result.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(user, role);
        logger.LogInformation("Seeded user {Email} with role {Role}", email, role);
    }
}
