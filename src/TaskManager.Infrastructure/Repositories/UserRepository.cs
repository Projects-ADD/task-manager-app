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