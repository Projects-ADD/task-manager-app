using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence;

using TaskAsync = System.Threading.Tasks.Task;

namespace TaskManager.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TaskManagerDbContext _db;

    public UserRepository(TaskManagerDbContext db)
    {
        _db = db;
    }

    public async TaskAsync AddAsync(User user)
    {
        /*
        * Add a new user.
        * @param user The user to add.
        * In PostgreSQL the query will be:
        * INSERT INTO "Users" ("Id", "Username", "FullName", "Email", "PasswordHash", "CreatedAt", "UpdatedAt") VALUES ({user.Id}, {user.Username}, {user.FullName}, {user.Email}, {user.PasswordHash}, {user.CreatedAt}, {user.UpdatedAt});
        */
        await _db.Users.AddAsync(user);
    }

    public async System.Threading.Tasks.Task<List<User>> GetAllAsync()
    {
        /**
         * Get all users that are not deleted.
         * 
         * @return List of users.
         * In PostgreSQL the query will be:
         * SELECT * FROM "Users" WHERE "DeletedAt" IS NULL;
         */
        return await _db.Users
            .Where(u => u.DeletedAt == null)
            .ToListAsync();
    }

    public async System.Threading.Tasks.Task<User?> GetByIdAsync(Guid id)
    {
        /**
         * Get a user by id that is not deleted.
         * 
         * @param id The id of the user.
         * @return The user or null if not found.
         * In PostgreSQL the query will be:
         * SELECT * FROM "Users" WHERE "Id" = {id} AND "DeletedAt" IS NULL;
        */
        return await _db.Users
            .FirstOrDefaultAsync(
                u => u.Id == id && u.DeletedAt == null);
    }

    public async System.Threading.Tasks.Task<User?> GetOneWithPermissionsAsync(Guid userId)
    {
        /**
         * Get a user by id that is not deleted, including their roles and permissions.
         * 
         * @param userId The id of the user.
         * @return The user with roles and permissions or null if not found.
         *
         * In PostgreSQL the query will be:
         * SELECT * FROM "Users" u
         * LEFT JOIN "UserRoles" ur ON u."Id" = ur."UserId"
         * LEFT JOIN "Roles" r ON ur."RoleId" = r."Id"
         * LEFT JOIN "RolePermissions" rp ON r."Id" = rp."RoleId"
         * LEFT JOIN "Permissions" p ON rp."PermissionId" = p."Id"
         * WHERE u."Id" = {userId} AND u."DeletedAt" IS NULL;
        */
        return await _db.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId && u.DeletedAt == null);
    }

    public TaskAsync UpdateAsync(User user)
    {   
        /*
        * Update a user.
        * 
        * @param user The user to update.
        * In PostgreSQL the query will be:
        * UPDATE "Users" SET "Username" = {user.Username}, "FullName" = {user.FullName}, "Email" = {user.Email}, "PasswordHash" = {user.PasswordHash}, "UpdatedAt" = {user.UpdatedAt} WHERE "Id" = {user.Id};
        */
        _db.Users.Update(user);

        return TaskAsync.CompletedTask;
    }

    public async TaskAsync SaveChangesAsync()
    {
        /*
        * Save changes to the database.
        *
        * In PostgreSQL the query will be:
        * COMMIT;
        */
        await _db.SaveChangesAsync();
    }
}