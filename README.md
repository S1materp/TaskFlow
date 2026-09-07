# TaskFlow API

TaskFlow is a REST API for task management built with C# and ASP.NET Core.

The project was created as a pet project to practice backend development, REST API design, relational databases, asynchronous programming, testing, and containerization.

## Features

- Create, read, update and delete tasks (CRUD)
- REST API with GET, POST, PUT and DELETE endpoints
- PostgreSQL database
- Entity Framework Core
- EF Core migrations
- CQRS with MediatR
- Async/await
- Background processing with BackgroundService
- Graceful cancellation with CancellationToken
- Swagger / OpenAPI documentation
- xUnit tests
- Docker containerization
- Docker Compose for PostgreSQL

## Technologies

- C#
- .NET 9
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- MediatR
- Swagger / OpenAPI
- xUnit
- Docker
- Docker Compose
- Git

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/tasks` | Get all tasks |
| GET | `/api/tasks/{id}` | Get task by ID |
| POST | `/api/tasks` | Create a task |
| PUT | `/api/tasks/{id}` | Update a task |
| DELETE | `/api/tasks/{id}` | Delete a task |

## Example Request

POST `/api/tasks`

```json
{
  "title": "Prepare for .NET internship",
  "isCompleted": false
}
```

## Background Processing

The application contains a background worker implemented with ASP.NET Core `BackgroundService`.

The worker asynchronously queries the database for incomplete tasks using Entity Framework Core and supports graceful shutdown using `CancellationToken`.

## Database

TaskFlow uses PostgreSQL as a relational database.

Database schema changes are managed using Entity Framework Core migrations.

Apply migrations with:

```bash
dotnet ef database update --project TaskFlow.Api/TaskFlow.Api.csproj
```

## Run PostgreSQL with Docker

Start PostgreSQL:

```bash
docker compose up -d postgres
```

Check running containers:

```bash
docker ps
```

## Run the API

```bash
dotnet run --project TaskFlow.Api/TaskFlow.Api.csproj
```

After starting the application, open the Swagger UI using the URL shown in the console.

## Tests

Run the test suite with:

```bash
dotnet test
```

## Project Structure

```text
TaskFlow/
├── TaskFlow.Api/       # ASP.NET Core REST API
├── TaskFlow.Tests/     # Automated tests
├── taskflow-client/    # React + TypeScript client
├── docker-compose.yml
└── TaskFlow.sln
```