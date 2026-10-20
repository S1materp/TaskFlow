# TaskFlow

Full-stack task management application built with **React, TypeScript and ASP.NET Core**.

TaskFlow demonstrates a modern frontend architecture, REST API integration, PostgreSQL persistence, Redis caching and containerized infrastructure.

![TaskFlow Frontend](docs/taskflow-frontend.png)

## Features

### Frontend

- Create tasks
- Complete tasks
- Delete tasks
- Filter by All / Active / Completed
- Active task counter
- Loading, error and empty states
- Responsive UI
- Client-side routing
- Server-state management and caching with TanStack Query
- Environment-based API configuration

### Backend

- REST API with ASP.NET Core
- CRUD operations
- PostgreSQL persistence with Entity Framework Core
- Redis caching
- Cache invalidation after data changes
- MediatR for application commands
- Background service for task statistics
- Swagger / OpenAPI documentation
- Unit tests with xUnit
- Dockerized infrastructure

## Tech Stack

### Frontend

- React
- TypeScript
- Vite
- TanStack Query
- React Router
- CSS
- ESLint

### Backend

- C#
- .NET 9
- ASP.NET Core
- Entity Framework Core
- MediatR
- PostgreSQL
- Redis
- Swagger / OpenAPI
- xUnit

### DevOps & Tools

- Docker
- Docker Compose
- Git
- GitHub

## Architecture

```text
React + TypeScript
        |
        | HTTP / REST
        v
ASP.NET Core API
        |
        +------> Redis Cache
        |
        +------> PostgreSQL
```

The frontend is separated into pages, reusable UI components, API services and TypeScript models.

```text
taskflow-client/src/
├── api/
│   └── tasks.ts
├── components/
│   ├── TaskFilter.tsx
│   ├── TaskForm.tsx
│   └── TaskItem.tsx
├── pages/
│   ├── TasksPage.tsx
│   └── NotFoundPage.tsx
├── types/
│   └── task.ts
├── App.tsx
└── main.tsx
```

TanStack Query manages asynchronous server state and synchronizes the UI with the backend after mutations.

The backend uses PostgreSQL as persistent storage. Redis implements caching for task-list requests, with cache invalidation after create, update and delete operations.

## API

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/tasks` | Get all tasks |
| GET | `/api/tasks/{id}` | Get task by ID |
| POST | `/api/tasks` | Create a task |
| PUT | `/api/tasks/{id}` | Update a task |
| DELETE | `/api/tasks/{id}` | Delete a task |

## API Demo

### Swagger

![Swagger API](docs/swagger-overview.png)

### Getting tasks

![GET tasks](docs/get-tasks.png)

### Creating a task

![Create task](docs/create-task.png)

## Run locally

### Requirements

- .NET 9 SDK
- Node.js
- Docker
- Docker Compose

### 1. Start infrastructure

```bash
docker compose up -d
```

### 2. Start backend

```bash
cd TaskFlow.Api
dotnet run
```

### 3. Configure frontend

Inside `taskflow-client`, create `.env` based on `.env.example`:

```env
VITE_API_URL=http://localhost:5019/api/tasks
```

### 4. Start frontend

```bash
cd taskflow-client
npm install
npm run dev
```

Open:

```text
http://localhost:5173
```

## QA Automation portfolio

TaskFlow also serves as a **Junior QA Automation portfolio project**: a real React frontend and ASP.NET Core backend tested across API, database, browser, and CI layers.

| Area | Tools | What is verified |
|---|---|---|
| REST API | Python, pytest, httpx | CRUD, HTTP status codes, invalid input, boundary cases |
| Database | PostgreSQL, psycopg | Created, updated, and deleted records match API operations |
| Caching | Redis, pytest | Task-list cache invalidation after mutations |
| Browser E2E | Python, Playwright, Chromium | Create, complete, filter, and delete tasks in the React UI |
| Background processing | pytest, application logs | Periodic task-statistics worker produces its log output |
| CI/CD | GitHub Actions | Automated .NET/frontend checks, API integration tests, and browser tests |

### Automated test suites

- [Python QA test suite](qa-tests/tests) — pytest fixtures, API tests, database checks, worker verification, and UI E2E tests.
- [QA setup and commands](qa-tests/README.md) — environment variables, dependencies, and local execution.
- [Python API / database CI](.github/workflows/python-qa.yml) — PostgreSQL and Redis services plus API integration tests.
- [Python Playwright CI](.github/workflows/python-e2e.yml) — backend, frontend, database, cache, and headless Chromium.
- [Full-stack CI](.github/workflows/ci.yml) — backend build/tests and frontend lint/build.

### Run the Python QA tests

Use a **disposable local database**: integration tests create and delete records.

```bash
python -m pip install -r qa-tests/requirements.txt
python -m playwright install chromium

# Once the backend, PostgreSQL, Redis and frontend are running:
python -m pytest -c qa-tests/pytest.ini qa-tests/tests/test_api.py qa-tests/tests/test_database.py -v
python -m pytest -c qa-tests/pytest.ini qa-tests/tests/test_ui.py -v
```

The optional worker-log check uses `TASKFLOW_API_LOG_PATH` to read captured API logs. See [QA setup](qa-tests/README.md) for connection configuration. The GitHub Actions workflows provide reproducible examples of the complete test environment.

## Quality checks

Frontend:

```bash
npm run build
npm run lint
```

Backend tests:

```bash
dotnet test
```

## Project purpose

TaskFlow was created as a portfolio full-stack project to practice building an application across the entire stack: frontend architecture, typed API integration, backend development, database persistence, caching, testing and containerized infrastructure.