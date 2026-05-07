using Gold.Application.Common;
using Gold.Application.DTOs.Branches;
using Gold.Application.DTOs.Customers;
using Gold.Application.DTOs.Workshops;

namespace Gold.Application.Interfaces;

public interface IBranchService
{
    Task<Result<List<BranchDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<BranchDto>> CreateAsync(CreateBranchDto dto, CancellationToken cancellationToken = default);
}

public interface IWorkshopService
{
    Task<Result<List<WorkshopDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<WorkshopDto>> CreateAsync(CreateWorkshopDto dto, CancellationToken cancellationToken = default);
}

public interface ICustomerService
{
    Task<Result<List<CustomerDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<CustomerDto>> SearchByPhoneAsync(string phone, CancellationToken cancellationToken = default);
    Task<Result<CustomerDto>> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken = default);
}
