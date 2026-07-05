using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Features.Permissions;
using TaskManager.Application.Features.Roles;
using TaskManager.Application.Features.Users;
using TaskManager.Application.Features.Tasks;

namespace TaskManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITaskService, TaskService>();
        
        return services;
    }
}
