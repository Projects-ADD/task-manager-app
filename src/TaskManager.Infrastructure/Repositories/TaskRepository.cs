using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence;

using TaskAsync = System.Threading.Tasks.Task;
using Task = TaskManager.Domain.Entities.Task;

public class TaskRepository : ITaskRepository
{
    private readonly TaskManagerDbContext _db;

    public TaskRepository(TaskManagerDbContext db)
    {
        _db = db;
    }

    public async TaskAsync AddAsync(Task task)
    {
        /**
         * Add a new task.
         * @param task The task to add.
         * In PostgreSQL the query will be:
         * INSERT INTO "Tasks" ("Id", "Title", "Description", "Status", "DueAt", "CreatedAt", "UpdatedAt") VALUES ({task.Id}, {task.Title}, {task.Description}, {task.Status}, {task.DueAt}, {task.CreatedAt}, {task.UpdatedAt});
         */
        await _db.Tasks.AddAsync(task);
    }

    public async System.Threading.Tasks.Task<List<Task>> GetAllAsync()
    {
        /**
         * Get all tasks.
         * 
         * @return List of tasks.
         * In PostgreSQL the query will be:
         * SELECT * FROM "Tasks" WHERE "DeletedAt" IS NULL;
         */
        return await _db.Tasks
            .Where(t => t.DeletedAt == null)
            .ToListAsync();
    }

    public async System.Threading.Tasks.Task<Task?> GetByIdAsync(Guid id)
    {
        /**
         * Get a task by id that is not deleted.
         * 
         * @param id The id of the task.
         * @return The task or null if not found.
         * In PostgreSQL the query will be:
         * SELECT * FROM "Tasks" WHERE "Id" = {id} AND "DeletedAt" IS NULL;
        */
        return await _db.Tasks
            .FirstOrDefaultAsync(
                t => t.Id == id && t.DeletedAt == null);
    }

    public async TaskAsync UpdateAsync(Task task)
    {
        /**
         * Update a task.
         *
         * @param task The task to update.
         * In PostgreSQL the query will be:
         * UPDATE "Tasks" SET "Title" = {task.Title}, "Description" = {task.Description}, "Status" = {task.Status}, "DueAt" = {task.DueAt}, "UpdatedAt" = {task.UpdatedAt} WHERE "Id" = {task.Id};
        */
        _db.Tasks.Update(task);

        await TaskAsync.CompletedTask;
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