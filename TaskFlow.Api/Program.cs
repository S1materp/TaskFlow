using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TaskFlow.Api.Contracts;
using TaskFlow.Api.Data;
using TaskFlow.Api.Features.Tasks;
using TaskFlow.Api.Models;
using TaskFlow.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHostedService<TaskStatsWorker>();

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "TaskFlow_";
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("Frontend");

app.UseSwagger();
app.UseSwaggerUI();

// GET: all tasks
app.MapGet("/api/tasks", async (
    AppDbContext db,
    IDistributedCache cache) =>
{
    const string cacheKey = "tasks";

    var cachedTasks = await cache.GetStringAsync(cacheKey);

    if (cachedTasks is not null)
    {
        var tasksFromCache =
            JsonSerializer.Deserialize<List<TaskItem>>(cachedTasks);

        return Results.Ok(tasksFromCache);
    }

    var tasks = await db.Tasks.ToListAsync();

    var json = JsonSerializer.Serialize(tasks);

    await cache.SetStringAsync(
        cacheKey,
        json,
        new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow =
                TimeSpan.FromMinutes(5)
        });

    return Results.Ok(tasks);
});

// GET: task by id
app.MapGet("/api/tasks/{id}", async (
    int id,
    AppDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);

    if (task == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(task);
});

// POST: create task
app.MapPost("/api/tasks", async (
    CreateTaskRequest request,
    IMediator mediator,
    IDistributedCache cache) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest("Title is required");
    }

    var command = new CreateTaskCommand(
        request.Title,
        request.IsCompleted);

    var task = await mediator.Send(command);

    // Clear cached task list
    await cache.RemoveAsync("tasks");

    return Results.Created(
        $"/api/tasks/{task.Id}",
        task);
});

// PUT: update task
app.MapPut("/api/tasks/{id}", async (
    int id,
    TaskItem updatedTask,
    AppDbContext db,
    IDistributedCache cache) =>
{
    var task = await db.Tasks.FindAsync(id);

    if (task == null)
    {
        return Results.NotFound();
    }

    if (string.IsNullOrWhiteSpace(updatedTask.Title))
    {
        return Results.BadRequest("Title is required");
    }

    task.Title = updatedTask.Title;
    task.IsCompleted = updatedTask.IsCompleted;

    await db.SaveChangesAsync();

    // Clear cached task list
    await cache.RemoveAsync("tasks");

    return Results.Ok(task);
});

// DELETE: delete task
app.MapDelete("/api/tasks/{id}", async (
    int id,
    AppDbContext db,
    IDistributedCache cache) =>
{
    var task = await db.Tasks.FindAsync(id);

    if (task == null)
    {
        return Results.NotFound();
    }

    db.Tasks.Remove(task);

    await db.SaveChangesAsync();

    // Clear cached task list
    await cache.RemoveAsync("tasks");

    return Results.NoContent();
});

app.Run();