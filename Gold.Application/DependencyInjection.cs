using Gold.Application.Interfaces;
using Gold.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Gold.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IBranchService, BranchService>();
        services.AddScoped<IWorkshopService, WorkshopService>();
        services.AddScoped<ICustomerService, CustomerService>();
        return services;
    }
}
