using MediatR;
using TaskFlow.Api.Data;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Features.Tasks;

public class CreateTaskCommandHandler
    : IRequestHandler<CreateTaskCommand, TaskItem>
{
    private readonly AppDbContext _db;

    public CreateTaskCommandHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<TaskItem> Handle(
        CreateTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task = new TaskItem
        {
            Title = request.Title,
            IsCompleted = request.IsCompleted
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync(cancellationToken);

        return task;
    }
}