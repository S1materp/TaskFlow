# Python QA Automation — TaskFlow

Automated checks for a running **ASP.NET Core API, PostgreSQL, Redis, and React frontend**, using Python, pytest, httpx, psycopg, and Playwright.

## Implemented coverage

- **API:** CRUD, HTTP status codes (200, 201, 204, 400, 404), invalid input, and boundary cases.
- **PostgreSQL:** consistency between API responses and persisted records.
- **Redis:** cache invalidation after task mutations.
- **Browser E2E:** create, complete, filter, and delete tasks; prevent blank submissions.
- **Background worker:** verify periodic statistics logging when an API log file is provided.
- **Isolation:** unique task data and pytest cleanup fixtures.

## Run locally

Prerequisites: Python 3.11+, .NET 9, Node.js, Docker Compose, and Chromium for Playwright.

**Use a disposable test database.** Tests create and delete records; never point them at production.

1. From the repository root, start PostgreSQL and Redis:

   ```bash
   docker compose up -d postgres redis
   ```

2. Configure the backend connection string for the Compose PostgreSQL port. For example, in a shell environment:

   ```text
   ConnectionStrings__DefaultConnection=Host=localhost;Port=55432;Database=taskflow;Username=taskflow;Password=taskflow123
   ```

3. Apply EF Core migrations with the same connection string: `dotnet ef database update --project TaskFlow.Api` (install `dotnet-ef` version 9 if necessary).
4. Start the API with the same connection string: `dotnet run --project TaskFlow.Api --urls http://127.0.0.1:5019`.
5. Install test dependencies and Chromium:

   ```bash
   python -m pip install -r qa-tests/requirements.txt
   python -m playwright install chromium
   ```

6. Run API and database tests while the backend is running:

   ```bash
   python -m pytest -c qa-tests/pytest.ini qa-tests/tests/test_api.py qa-tests/tests/test_database.py -v
   ```

7. For browser E2E tests, configure `taskflow-client/.env` with `VITE_API_URL=http://127.0.0.1:5019/api/tasks`, start the frontend (`cd taskflow-client && npm install && npm run dev -- --host 127.0.0.1`), and run from the repository root:

   ```bash
   python -m pytest -c qa-tests/pytest.ini qa-tests/tests/test_ui.py -v
   ```

8. To test worker logging, capture the API output in a file, set `TASKFLOW_API_LOG_PATH` to its path, and run:

   ```bash
   python -m pytest -c qa-tests/pytest.ini qa-tests/tests/test_worker.py -v
   ```

   Without the log path, the worker test is skipped.

## Environment variables

- `TASKFLOW_API_URL` — default `http://127.0.0.1:5019`.
- `TASKFLOW_DATABASE_URL` — PostgreSQL connection URL for the disposable database (see pytest fixtures).
- `TASKFLOW_FRONTEND_URL` — default `http://127.0.0.1:5173`.
- `TASKFLOW_API_LOG_PATH` — optional API log file for the background worker check.

## CI workflows

- [API and database integration tests](../.github/workflows/python-qa.yml) — start backend, PostgreSQL, and Redis, then run pytest.
- [Playwright browser tests](../.github/workflows/python-e2e.yml) — start backend and frontend, install Chromium, and run UI tests.
- [Full-stack checks](../.github/workflows/ci.yml) — backend and frontend build/test checks.

## Potential future improvements

API schema validation, additional malformed-request cases, concurrency scenarios, and broader worker failure/retry coverage.
