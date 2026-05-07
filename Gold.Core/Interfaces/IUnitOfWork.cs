namespace Gold.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepairOrderRepository Orders { get; }
    ICustomerRepository Customers { get; }
    IBranchRepository Branches { get; }
    IWorkshopRepository Workshops { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
