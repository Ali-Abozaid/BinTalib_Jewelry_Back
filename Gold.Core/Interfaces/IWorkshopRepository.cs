using Gold.Core.Entities;

namespace Gold.Core.Interfaces;

public interface IWorkshopRepository : IGenericRepository<Workshop>
{
    Task<Workshop?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
