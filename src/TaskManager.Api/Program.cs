var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen();

var app = builder.Build();

//app.UseSwagger();

//app.UseSwaggerUI();

app.MapGet("/", () =>
{
    return Results.Ok("Task Manager API running");
});

app.Run();