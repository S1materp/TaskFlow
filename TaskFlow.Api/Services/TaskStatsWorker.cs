using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;

namespace TaskFlow.Api.Services;

public class TaskStatsWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TaskStatsWorker> _logger;

    public TaskStatsWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<TaskStatsWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            var incompleteTasks = await dbContext.Tasks
                .CountAsync(task => !task.IsCompleted, stoppingToken);

            _logger.LogInformation(
                "Incomplete tasks: {Count}",
                incompleteTasks);

            await Task.Delay(
                TimeSpan.FromMinutes(1),
                stoppingToken);
        }
    }
}