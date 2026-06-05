using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Infrastructure.Persistence;
using TaskManager.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<TaskManagerDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();

var app = builder.Build();

//app.UseSwagger();

//app.UseSwaggerUI();

app.MapGet("/", () =>
{
    return Results.Ok("Task Manager API running");
});

app.Run();