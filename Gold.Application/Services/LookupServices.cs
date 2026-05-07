using Gold.Application.Common;
using Gold.Application.DTOs.Branches;
using Gold.Application.DTOs.Customers;
using Gold.Application.DTOs.Workshops;
using Gold.Application.Interfaces;
using Gold.Application.Mapping;
using Gold.Core.Entities;
using Gold.Core.Interfaces;

namespace Gold.Application.Services;

public class BranchService : IBranchService
{
    private readonly IUnitOfWork _uow;
    public BranchService(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<List<BranchDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await _uow.Branches.GetAllAsync(cancellationToken);
        return Result<List<BranchDto>>.Success(list.Select(b => b.ToDto()).ToList());
    }

    public async Task<Result<BranchDto>> CreateAsync(CreateBranchDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Branch { Name = dto.Name, Address = dto.Address, Phone = dto.Phone };
        await _uow.Branches.AddAsync(entity, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return Result<BranchDto>.Success(entity.ToDto());
    }
}

public class WorkshopService : IWorkshopService
{
    private readonly IUnitOfWork _uow;
    public WorkshopService(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<List<WorkshopDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await _uow.Workshops.GetAllAsync(cancellationToken);
        return Result<List<WorkshopDto>>.Success(list.Select(w => w.ToDto()).ToList());
    }

    public async Task<Result<WorkshopDto>> CreateAsync(CreateWorkshopDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _uow.Workshops.GetByNameAsync(dto.Name, cancellationToken);
        if (existing is not null)
        {
            return Result<WorkshopDto>.Success(existing.ToDto());
        }
        var entity = new Workshop { Name = dto.Name, Phone = dto.Phone, Address = dto.Address };
        await _uow.Workshops.AddAsync(entity, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return Result<WorkshopDto>.Success(entity.ToDto());
    }
}

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _uow;
    public CustomerService(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<List<CustomerDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await _uow.Customers.GetAllAsync(cancellationToken);
        return Result<List<CustomerDto>>.Success(list.Select(c => c.ToDto()).ToList());
    }

    public async Task<Result<CustomerDto>> SearchByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        var c = await _uow.Customers.GetByPhoneAsync(phone, cancellationToken);
        if (c is null) return Result<CustomerDto>.Failure("Customer not found");
        return Result<CustomerDto>.Success(c.ToDto());
    }

    public async Task<Result<CustomerDto>> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _uow.Customers.GetByPhoneAsync(dto.Phone, cancellationToken);
        if (existing is not null)
        {
            return Result<CustomerDto>.Success(existing.ToDto());
        }
        var entity = new Customer { Name = dto.Name, Phone = dto.Phone, Email = dto.Email };
        await _uow.Customers.AddAsync(entity, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return Result<CustomerDto>.Success(entity.ToDto());
    }
}
