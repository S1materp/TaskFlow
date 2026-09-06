using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Contracts;
using TaskFlow.Api.Data;
using TaskFlow.Api.Features.Tasks;
using TaskFlow.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// GET: all tasks
app.MapGet("/api/tasks", async (AppDbContext db) =>
{
    var tasks = await db.Tasks.ToListAsync();

    return Results.Ok(tasks);
});

// GET: task by id
app.MapGet("/api/tasks/{id}", async (int id, AppDbContext db) =>
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
    IMediator mediator) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest("Title is required");
    }

    var command = new CreateTaskCommand(
        request.Title,
        request.IsCompleted);

    var task = await mediator.Send(command);

    return Results.Created($"/api/tasks/{task.Id}", task);
});

// PUT: update task
app.MapPut("/api/tasks/{id}", async (
    int id,
    TaskItem updatedTask,
    AppDbContext db) =>
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

    return Results.Ok(task);
});

// DELETE: delete task
app.MapDelete("/api/tasks/{id}", async (
    int id,
    AppDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);

    if (task == null)
    {
        return Results.NotFound();
    }

    db.Tasks.Remove(task);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();