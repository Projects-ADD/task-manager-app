using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Infrastructure.Persistence;
using TaskManager.Infrastructure.Repositories;

namespace TaskManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        /* services.AddDbContext<TaskManagerDbContext>(options =>
            options.UseNpgsql(connectionString)); */
        
        services.AddDbContext<TaskManagerDbContext>(options =>
        {
            options.UseNpgsql(connectionString);

            options.EnableSensitiveDataLogging();

            options.LogTo(Console.WriteLine);
        });

        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        return services;
    }
}
