\# TaskFlow API



TaskFlow is a REST API for task management built with ASP.NET Core.



The project was created as a pet project to practice backend development with C# and .NET.



\## Features



\- Create tasks

\- Get all tasks

\- Get task by id

\- Update tasks

\- Delete tasks

\- HTTP status code handling

\- DTO validation

\- SQLite database

\- Entity Framework Core

\- EF Core migrations

\- CQRS with MediatR

\- Swagger / OpenAPI

\- xUnit tests

\- Async/await



\## Technologies



\- C#

\- .NET 9

\- ASP.NET Core

\- Entity Framework Core

\- SQLite

\- MediatR

\- Swagger

\- xUnit

\- Git



\## API Endpoints



| Method | Endpoint | Description |

|---|---|---|

| GET | `/api/tasks` | Get all tasks |

| GET | `/api/tasks/{id}` | Get task by id |

| POST | `/api/tasks` | Create task |

| PUT | `/api/tasks/{id}` | Update task |

| DELETE | `/api/tasks/{id}` | Delete task |



\## Example request



POST `/api/tasks`



```json

{

&#x20; "title": "Learn ASP.NET Core",

&#x20; "isCompleted": false

}

