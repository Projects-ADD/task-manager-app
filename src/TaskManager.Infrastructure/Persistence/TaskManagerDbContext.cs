using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence.Configurations;

namespace TaskManager.Infrastructure.Persistence;

public class TaskManagerDbContext : DbContext
{
    public TaskManagerDbContext(
        DbContextOptions<TaskManagerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<User> Users => Set<User>();

    public DbSet<UserRoles> UserRoles => Set<UserRoles>();

    public DbSet<TaskManager.Domain.Entities.Task> Tasks => Set<TaskManager.Domain.Entities.Task>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(
            new RoleConfiguration()
        );

        modelBuilder.ApplyConfiguration(
            new PermissionConfiguration()
        );

        modelBuilder.ApplyConfiguration(
            new RolePermissionConfiguration()
        );

        modelBuilder.ApplyConfiguration(
            new UserConfiguration()
        );

        modelBuilder.ApplyConfiguration(
            new TaskConfiguration()
        );

        modelBuilder.ApplyConfiguration(
            new UserRoleConfiguration()
        );
    }

    /*
        //más adelante:
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(TaskManagerDbContext).Assembly
            );
        }
    */
}