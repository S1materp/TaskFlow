using MediatR;
using TaskFlow.Api.Features.Tasks;

using TaskFlow.Api.Contracts;

using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
 

using TaskFlow.Api.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();




app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/api/tasks", async (AppDbContext db) =>
{
    var tasks = await db.Tasks.ToListAsync();

    return Results.Ok(tasks);
});

app.MapGet("/api/tasks/{id}", async (int id, AppDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);

    if (task == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(task);
});

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

    task.Title = updatedTask.Title;
    task.IsCompleted = updatedTask.IsCompleted;

    await db.SaveChangesAsync();

    return Results.Ok(task);
});

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

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
