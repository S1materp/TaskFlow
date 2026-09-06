namespace TaskFlow.Api.Contracts;

public class CreateTaskRequest
{
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}