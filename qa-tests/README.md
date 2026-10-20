# Python QA Automation — TaskFlow

Integration test suite for the **real running** ASP.NET Core API, PostgreSQL and Redis.

## Coverage

- REST CRUD and status codes (200, 201, 204, 400, 404)
- Validation and whitespace boundaries
- API-to-PostgreSQL data consistency
- Redis cache invalidation after create/update
- Isolated test data using pytest fixtures and cleanup

## Local run

Prerequisites: Python 3.11+, .NET 9, Docker Compose.

1. From the repository root: `docker compose up -d postgres redis`
2. Configure backend connection string for the Compose PostgreSQL port:
   `ConnectionStrings__DefaultConnection="Host=localhost;Port=55432;Database=taskflow;Username=taskflow;Password=taskflow123"`
3. Apply migrations with `dotnet ef database update --project TaskFlow.Api` (install `dotnet-ef` version 9 if needed).
4. Start API with `dotnet run --project TaskFlow.Api --urls http://127.0.0.1:5019` using the same connection-string environment variable.
5. Install Python dependencies: `python -m pip install -r qa-tests/requirements.txt`
6. Run `python -m pytest -c qa-tests/pytest.ini qa-tests/tests -v`.

Environment variables:
- `TASKFLOW_API_URL` (default `http://127.0.0.1:5019`)
- `TASKFLOW_DATABASE_URL` (default Compose PostgreSQL at port 55432)

**Safety:** these tests write to the configured database. Use a disposable test environment, never production.

CI runs these integration tests against a temporary PostgreSQL + Redis stack. A failing check is a signal to investigate, not a reason to remove assertions.

## Next steps

- Python Playwright E2E coverage for the React UI
- Background worker log/retry/failure behavior tests (worker currently only counts tasks and logs)
- API schema validation, malformed request and concurrency scenarios
