using Gold.Core.Entities;

namespace Gold.Core.Interfaces;

public interface IRepairOrderRepository : IGenericRepository<RepairOrder>
{
    Task<IReadOnlyList<RepairOrder>> GetAllWithRelationsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RepairOrder>> GetForBranchAsync(Guid branchId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RepairOrder>> GetForWorkshopAsync(Guid workshopId, CancellationToken cancellationToken = default);
    Task<RepairOrder?> GetByIdWithRelationsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}
