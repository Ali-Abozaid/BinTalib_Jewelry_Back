using Gold.Application.DTOs.Branches;
using Gold.Application.DTOs.Customers;
using Gold.Application.DTOs.Orders;
using Gold.Application.DTOs.Workshops;
using Gold.Core.Entities;

namespace Gold.Application.Mapping;

public static class Mapper
{
    public static CustomerDto ToDto(this Customer c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Phone = c.Phone,
        Email = c.Email
    };

    public static BranchDto ToDto(this Branch b) => new()
    {
        Id = b.Id,
        Name = b.Name,
        Address = b.Address,
        Phone = b.Phone,
        IsActive = b.IsActive
    };

    public static WorkshopDto ToDto(this Workshop w) => new()
    {
        Id = w.Id,
        Name = w.Name,
        Phone = w.Phone,
        Address = w.Address,
        IsActive = w.IsActive
    };

    public static OrderStatusHistoryDto ToDto(this OrderStatusHistory h) => new()
    {
        Status = h.Status,
        Note = h.Note,
        ActorUserName = h.ActorUserName,
        ActorRole = h.ActorRole,
        OccurredAt = h.OccurredAt
    };

    public static OrderDto ToDto(this RepairOrder o) => new()
    {
        Id = o.Id,
        Code = o.Code,
        Customer = o.Customer is null ? new CustomerDto() : o.Customer.ToDto(),
        Branch = o.Branch is null ? new BranchDto() : o.Branch.ToDto(),
        Workshop = o.Workshop?.ToDto(),
        ReceivingEmployeeName = o.ReceivingEmployeeName,
        WorkshopCourierName = o.WorkshopCourierName,
        WeightBefore = o.WeightBefore,
        WeightAfter = o.WeightAfter,
        ImageBeforeUrl = o.ImageBeforeUrl,
        ImageAfterUrl = o.ImageAfterUrl,
        PricingType = o.PricingType,
        Price = o.Price,
        ReceivedAt = o.ReceivedAt,
        DeliveryToWorkshopDate = o.DeliveryToWorkshopDate,
        CompletedAt = o.CompletedAt,
        DeliveredToCustomerAt = o.DeliveredToCustomerAt,
        Status = o.Status,
        IsExternal = o.IsExternal,
        ExternalProviderName = o.ExternalProviderName,
        Notes = o.Notes,
        CreatedAt = o.CreatedAt,
        StatusHistory = o.StatusHistory?
            .OrderBy(h => h.OccurredAt)
            .Select(h => h.ToDto())
            .ToList() ?? new List<OrderStatusHistoryDto>()
    };
}
