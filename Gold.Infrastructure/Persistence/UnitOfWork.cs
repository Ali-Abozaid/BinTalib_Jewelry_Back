using Gold.Core.Interfaces;
using Gold.Infrastructure.Persistence.Repositories;

namespace Gold.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Orders = new RepairOrderRepository(context);
        Customers = new CustomerRepository(context);
        Branches = new BranchRepository(context);
        Workshops = new WorkshopRepository(context);
    }

    public IRepairOrderRepository Orders { get; }
    public ICustomerRepository Customers { get; }
    public IBranchRepository Branches { get; }
    public IWorkshopRepository Workshops { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}
