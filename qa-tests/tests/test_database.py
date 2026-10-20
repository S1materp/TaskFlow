"""Cross-layer assertions: API response must match PostgreSQL persistence."""
import pytest


pytestmark = pytest.mark.integration


def test_created_task_is_persisted(client, db, task_factory):
    task = task_factory()
    row = db.execute(
        'SELECT "Title", "IsCompleted" FROM "Tasks" WHERE "Id" = %s',
        (task["id"],),
    ).fetchone()
    assert row == (task["title"], False)


def test_update_is_persisted(client, db, task_factory):
    task = task_factory()
    response = client.put(
        f"/api/tasks/{task['id']}",
        json={"id": task["id"], "title": "db-updated", "isCompleted": True},
    )
    assert response.status_code == 200
    row = db.execute(
        'SELECT "Title", "IsCompleted" FROM "Tasks" WHERE "Id" = %s',
        (task["id"],),
    ).fetchone()
    assert row == ("db-updated", True)


def test_delete_removes_database_record(client, db, task_factory):
    task = task_factory()
    assert client.delete(f"/api/tasks/{task['id']}").status_code == 204
    row = db.execute(
        'SELECT "Id" FROM "Tasks" WHERE "Id" = %s',
        (task["id"],),
    ).fetchone()
    assert row is None


def test_cache_is_invalidated_after_create_and_update(client, task_factory):
    assert client.get("/api/tasks").status_code == 200  # warm Redis cache
    task = task_factory()
    response = client.get("/api/tasks")
    assert any(item["id"] == task["id"] for item in response.json())

    response = client.put(
        f"/api/tasks/{task['id']}",
        json={"id": task["id"], "title": "fresh-cache", "isCompleted": True},
    )
    assert response.status_code == 200
    tasks = client.get("/api/tasks").json()
    updated = next(item for item in tasks if item["id"] == task["id"])
    assert updated["title"] == "fresh-cache"
    assert updated["isCompleted"] is True
