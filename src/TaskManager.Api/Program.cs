using TaskManager.Application;
using TaskManager.Infrastructure;
using TaskManager.Api.Middlewares;

using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(
                        new JsonStringEnumConverter(allowIntegerValues: false)
                    );
                });
builder.Services.AddEndpointsApiExplorer();

// Add Swagger/OpenAPI support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Task Manager API",
        Description = "An ASP.NET Core Web API for managing tasks.",
        Contact = new OpenApiContact
        {
            Name = "Your Name",
            Email = "your.email@example.com"
        }
    });

    //include XML comments if available
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    if (File.Exists(xmlFile)) options.IncludeXmlComments(xmlFile);

    var xmlContracts = $"{typeof(TaskManager.Contracts.Class1).Assembly.GetName().Name}.xml";
    if (File.Exists(xmlContracts)) options.IncludeXmlComments(xmlContracts);

    //options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
    //options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlContracts));

});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Manager API v1");
    options.RoutePrefix = "docs/swagger";
});
/* if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Manager API v1");
        options.RoutePrefix = "docs/swagger";
    });
} */
app.MapControllers();

app.MapGet("/", () =>
{
    return Results.Ok("Task Manager API running");
});

app.Run();