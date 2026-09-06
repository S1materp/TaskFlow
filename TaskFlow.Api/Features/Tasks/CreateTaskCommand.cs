using MediatR;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Features.Tasks;

public record CreateTaskCommand(
    string Title,
    bool IsCompleted
) : IRequest<TaskItem>;