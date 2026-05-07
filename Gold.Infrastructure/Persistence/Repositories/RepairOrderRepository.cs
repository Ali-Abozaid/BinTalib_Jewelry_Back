using Gold.Core.Entities;
using Gold.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gold.Infrastructure.Persistence.Repositories;

public class RepairOrderRepository : GenericRepository<RepairOrder>, IRepairOrderRepository
{
    public RepairOrderRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<RepairOrder>> GetAllWithRelationsAsync(CancellationToken cancellationToken = default)
        => await _set
            .Include(o => o.Customer)
            .Include(o => o.Branch)
            .Include(o => o.Workshop)
            .Include(o => o.StatusHistory)
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<RepairOrder>> GetForBranchAsync(Guid branchId, CancellationToken cancellationToken = default)
        => await _set
            .Where(o => o.BranchId == branchId)
            .Include(o => o.Customer)
            .Include(o => o.Branch)
            .Include(o => o.Workshop)
            .Include(o => o.StatusHistory)
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<RepairOrder>> GetForWorkshopAsync(Guid workshopId, CancellationToken cancellationToken = default)
        => await _set
            .Where(o => o.WorkshopId == workshopId)
            .Include(o => o.Customer)
            .Include(o => o.Branch)
            .Include(o => o.Workshop)
            .Include(o => o.StatusHistory)
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<RepairOrder?> GetByIdWithRelationsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _set
            .Include(o => o.Customer)
            .Include(o => o.Branch)
            .Include(o => o.Workshop)
            .Include(o => o.StatusHistory)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await _set.CountAsync(cancellationToken);
}

public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context) { }

    public Task<Customer?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default)
        => _set.FirstOrDefaultAsync(c => c.Phone == phone, cancellationToken);
}

public class BranchRepository : GenericRepository<Branch>, IBranchRepository
{
    public BranchRepository(AppDbContext context) : base(context) { }
}

public class WorkshopRepository : GenericRepository<Workshop>, IWorkshopRepository
{
    public WorkshopRepository(AppDbContext context) : base(context) { }

    public Task<Workshop?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => _set.FirstOrDefaultAsync(w => w.Name == name, cancellationToken);
}
