using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly TaskManagerDbContext _db;

    public RoleRepository(TaskManagerDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task AddAsync(Role role)
    {
        /*
        * Add a new role.
        * @param role The role to add.
        * In PostgreSQL the query will be:
        * INSERT INTO "Roles" ("Id", "Name", "Description", "CreatedAt", "UpdatedAt") VALUES ({role.Id}, {role.Name}, {role.Description}, {role.CreatedAt}, {role.UpdatedAt});
        */
        await _db.Roles.AddAsync(role);
    }

    public async System.Threading.Tasks.Task<List<Role>> GetAllAsync()
    {
        /**
         * Get all roles that are not deleted.
         * 
         * @return List of roles.
         * In PostgreSQL the query will be:
         * SELECT * FROM "Roles" WHERE "DeletedAt" IS NULL;
         */
        return await _db.Roles
            .Where(r => r.DeletedAt == null)
            .ToListAsync();
    }

    public async System.Threading.Tasks.Task<Role?> GetByIdAsync(Guid id)
    {
        /**
         * Get a role by id that is not deleted.
         * 
         * @param id The id of the role.
         * @return The role or null if not found.
         * In PostgreSQL the query will be:
         * SELECT * FROM "Roles" WHERE "Id" = {id} AND "DeletedAt" IS NULL;
        */
        return await _db.Roles
            .FirstOrDefaultAsync(
                p => p.Id == id && p.DeletedAt == null);
    }

    public System.Threading.Tasks.Task UpdateAsync(Role role)
    {
        /*
        * Update a role.
        * @param role The role to update.
        * In PostgreSQL the query will be:
        * UPDATE "Roles" SET "Name" = {role.Name}, "Description" = {role.Description}, "UpdatedAt" = {role.UpdatedAt} WHERE "Id" = {role.Id};
        */
        _db.Roles.Update(role);

        return System.Threading.Tasks.Task.CompletedTask;
    }

    public async System.Threading.Tasks.Task SaveChangesAsync()
    {
        /*
        * Save changes to the database.
        *
        * In PostgreSQL the query will be:
        * COMMIT;
        */
        await _db.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task<Role?> GetOneWithUsersAsync(Guid roleId)
    {
        /**
         * Get a role by id that is not deleted, including its associated users.
         * 
         * @param roleId The id of the role.
         * @return The role with its associated users or null if not found.
         *
         * In PostgreSQL the query will be:
         * SELECT r.*, u.* FROM "Roles" r
         * LEFT JOIN "UserRoles" ur ON r."Id" = ur."RoleId"
         * LEFT JOIN "Users" u ON ur."UserId" = u."Id"
         * WHERE r."Id" = {roleId} AND r."DeletedAt" IS NULL;
        */
        return await _db.Roles
            .Include(r => r.UserRoles)
            .ThenInclude(ur => ur.User)
            .FirstOrDefaultAsync(r => r.Id == roleId && r.DeletedAt == null);
    }

    public async System.Threading.Tasks.Task<Role?> GetByIdWithPermissionsAsync(Guid roleId)
    {
        /**
         * Get a role by id that is not deleted, including its associated permissions.
         * 
         * @param roleId The id of the role.
         * @return The role with its associated permissions or null if not found.
         * In PostgreSQL the query will be:
         * SELECT r.*, rp.*, p.* FROM "Roles" r
         * LEFT JOIN "RolePermissions" rp ON r."Id" = rp."RoleId"
         * LEFT JOIN "Permissions" p ON rp."PermissionId" = p."Id"
         * WHERE r."Id" = {roleId} AND r."DeletedAt" IS NULL;
        */
        return await _db.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == roleId && r.DeletedAt == null);
    }

    public async System.Threading.Tasks.Task<List<Role>> GetAllWithPermissionsAsync()
    {
        /**
         * Get all roles that are not deleted, including their associated permissions.
         * 
         * @return List of roles with their associated permissions.
         * In PostgreSQL the query will be:
         * SELECT r.*, rp.*, p.* FROM "Roles" r
         * LEFT JOIN "RolePermissions" rp ON r."Id" = rp."RoleId"
         * LEFT JOIN "Permissions" p ON rp."PermissionId" = p."Id"
         * WHERE r."DeletedAt" IS NULL;
        */
        return await _db.Roles
            .Where(r => r.DeletedAt == null)
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .ToListAsync();
    }

    /* public async System.Threading.Tasks.Task AddRolePermissionAsync(RolePermission rolePermission)
    {
        await _db.RolePermissions.AddAsync(rolePermission);
    } */
}