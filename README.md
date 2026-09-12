# TaskFlow

TaskFlow is a backend REST API for task management built with **C# and ASP.NET Core**.

The project demonstrates backend development with PostgreSQL, Redis caching, Docker, Entity Framework Core, MediatR and automated tests.

## Features

- Create tasks
- Get all tasks
- Get task by ID
- Update tasks
- Delete tasks
- PostgreSQL data persistence
- Redis distributed caching
- Cache invalidation after data changes
- Background task processing
- REST API documentation with Swagger
- Unit tests

## Tech Stack

- C#
- .NET 9
- ASP.NET Core
- REST API
- Entity Framework Core
- PostgreSQL
- Redis
- MediatR
- Docker / Docker Compose
- Swagger / OpenAPI
- xUnit
- Git

## Architecture

```text
Client
  |
  v
ASP.NET Core REST API
  |
  +------> Redis Cache
  |
  +------> PostgreSQL
  |
  +------> BackgroundService
```

The API uses PostgreSQL as persistent storage and Redis as a distributed cache.

MediatR is used for application commands, while Entity Framework Core provides database access.

## Redis Caching

`GET /api/tasks` uses Redis to reduce unnecessary database requests.

Request flow:

```text
GET /api/tasks
      |
      v
    Redis
    /   \
  HIT   MISS
   |      |
Response PostgreSQL
          |
          v
        Redis
          |
          v
       Response
```

The task list is cached for 5 minutes.

The cache is invalidated after:

- POST
- PUT
- DELETE

This prevents clients from receiving outdated task data after changes.

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/tasks` | Get all tasks |
| GET | `/api/tasks/{id}` | Get task by ID |
| POST | `/api/tasks` | Create a task |
| PUT | `/api/tasks/{id}` | Update a task |
| DELETE | `/api/tasks/{id}` | Delete a task |

## API Demo

### Swagger

The API is documented and can be tested using Swagger UI.

![Swagger API](docs/swagger-overview.png)

### Get Tasks

`GET /api/tasks` returns the current task list.

![GET tasks](docs/get-tasks.png)

### Create Task

`POST /api/tasks` creates a new task.

![Create task](docs/create-task.png)

## Example Request

```json
{
  "title": "Add Redis caching",
  "isCompleted": false
}
```

## Running the Project

### Requirements

- .NET 9 SDK
- Docker
- Docker Compose

Start PostgreSQL:

```bash
docker compose up -d
```

Start Redis:

```bash
docker run -d --name taskflow-redis -p 6379:6379 redis:7
```

Run the API:

```bash
cd TaskFlow.Api
dotnet run
```

Open Swagger in the browser using the URL displayed by the application.

## Tests

Run automated tests:

```bash
dotnet test
```

## What This Project Demonstrates

- Backend development with C# and ASP.NET Core
- REST API design
- Relational database integration
- Distributed caching with Redis
- Cache invalidation
- Asynchronous programming
- Background services
- Containerization with Docker
- Unit testing
- Git-based development workflow

## Author

**Serafima Saltanova**

GitHub: [S1materp](https://github.com/S1materp)