using TaskFlow.Api.Contracts;
using Xunit;

namespace TaskFlow.Tests;

public class CreateTaskTests
{
    [Fact]
    public void CreateTaskRequest_ShouldStoreValues()
    {
        var request = new CreateTaskRequest
        {
            Title = "Изучить xUnit",
            IsCompleted = false
        };

        Assert.Equal("Изучить xUnit", request.Title);
        Assert.False(request.IsCompleted);
    }
}