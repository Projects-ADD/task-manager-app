using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence;

using TaskAsync = System.Threading.Tasks.Task;

namespace TaskManager.Infrastructure.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly TaskManagerDbContext _db;

    public PermissionRepository(TaskManagerDbContext db)
    {
        _db = db;
    }

    public async TaskAsync AddAsync(Permission permission)
    {
        /*
        * Add a new permission.
        * @param permission The permission to add.
        * In PostgreSQL the query will be:
        * INSERT INTO "Permissions" ("Id", "Name", "Description", "CreatedAt", "UpdatedAt") VALUES ({permission.Id}, {permission.Name}, {permission.Description}, {permission.CreatedAt}, {permission.UpdatedAt});
        */
        await _db.Permissions.AddAsync(permission);
    }

    public async System.Threading.Tasks.Task<List<Permission>> GetAllAsync()
    {
        /**
         * Get all permissions that are not deleted.
         * 
         * @return List of permissions.
         * In PostgreSQL the query will be:
         * SELECT * FROM "Permissions" WHERE "DeletedAt" IS NULL;
         */
        return await _db.Permissions
            .Where(p => p.DeletedAt == null)
            .ToListAsync();
    }

    public async System.Threading.Tasks.Task<Permission?> GetByIdAsync(Guid id)
    {
        /**
         * Get a permission by id that is not deleted.
         * 
         * @param id The id of the permission.
         * @return The permission or null if not found.
         * In PostgreSQL the query will be:
         * SELECT * FROM "Permissions" WHERE "Id" = {id} AND "DeletedAt" IS NULL;
         */
        return await _db.Permissions
            .FirstOrDefaultAsync(
                p => p.Id == id && p.DeletedAt == null);
    }

    public async System.Threading.Tasks.Task<Permission?> GetOneWithRolesAsync(Guid permissionId)
    {
        /**
         * Get a permission by id that is not deleted, including its associated roles.
         * 
         * @param permissionId The id of the permission.
         * @return The permission with its associated roles or null if not found.
         * 
         * In PostgreSQL the query will be:
         * SELECT * FROM "Permissions" p
         * LEFT JOIN "RolePermissions" rp ON p."Id" = rp."PermissionId"
         * LEFT JOIN "Roles" r ON rp."RoleId" = r."Id"
         * WHERE p."Id" = {permissionId} AND p."DeletedAt" IS NULL;
         */
        return await _db.Permissions
            .Include(p => p.RolePermissions)
                .ThenInclude(rp => rp.Role)
            .FirstOrDefaultAsync(
                p => p.Id == permissionId && p.DeletedAt == null);
    }

    public TaskAsync UpdateAsync(Permission permission)
    {
        /*
        * Update a permission.
        * @param permission The permission to update.
        * In PostgreSQL the query will be:
        * UPDATE "Permissions" SET "Name" = {permission.Name}, "Description" = {permission.Description}, "UpdatedAt" = {permission.UpdatedAt} WHERE "Id" = {permission.Id};
        */
        _db.Permissions.Update(permission);

        return TaskAsync.CompletedTask;
    }

    public async TaskAsync SaveChangesAsync()
    {
        /*
        * Save changes to the database.
        * In PostgreSQL the query will be:
        * COMMIT;
        */
        await _db.SaveChangesAsync();
    }
}