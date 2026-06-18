using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Features.Permissions;
using TaskManager.Application.Features.Roles;

namespace TaskManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IRoleService, RoleService>();

        return services;
    }
}
