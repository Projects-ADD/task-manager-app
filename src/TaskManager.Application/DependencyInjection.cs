using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Features.Permissions;

namespace TaskManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPermissionService, PermissionService>();

        return services;
    }
}
