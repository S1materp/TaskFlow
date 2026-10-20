"""Black-box REST API tests, including error and boundary cases."""
import uuid

import pytest


pytestmark = pytest.mark.integration


def test_create_returns_201_location_and_body(client, task_factory):
    title = f"create-{uuid.uuid4().hex}"
    task = task_factory(title=title)
    assert isinstance(task["id"], int)
    assert task["title"] == title
    assert task["isCompleted"] is False


def test_get_created_task(client, task_factory):
    task = task_factory()
    response = client.get(f"/api/tasks/{task['id']}")
    assert response.status_code == 200
    assert response.json() == task


def test_list_contains_new_task(client, task_factory):
    task = task_factory()
    response = client.get("/api/tasks")
    assert response.status_code == 200
    assert any(item["id"] == task["id"] for item in response.json())


@pytest.mark.parametrize("title", ["", " ", "\t", "\n"])
def test_create_rejects_blank_title(client, title):
    response = client.post("/api/tasks", json={"title": title, "isCompleted": False})
    assert response.status_code == 400


def test_create_preserves_completed_flag(client, task_factory):
    task = task_factory(is_completed=True)
    assert task["isCompleted"] is True


def test_update_task(client, task_factory):
    task = task_factory()
    response = client.put(
        f"/api/tasks/{task['id']}",
        json={"id": task["id"], "title": "updated", "isCompleted": True},
    )
    assert response.status_code == 200
    assert response.json()["title"] == "updated"
    assert response.json()["isCompleted"] is True


def test_update_rejects_blank_title(client, task_factory):
    task = task_factory()
    response = client.put(
        f"/api/tasks/{task['id']}",
        json={"id": task["id"], "title": " ", "isCompleted": False},
    )
    assert response.status_code == 400


@pytest.mark.parametrize("method", ["get", "put", "delete"])
def test_unknown_task_returns_404(client, method):
    task_id = 2147483647
    if method == "put":
        response = client.put(
            f"/api/tasks/{task_id}",
            json={"id": task_id, "title": "not found", "isCompleted": False},
        )
    else:
        response = getattr(client, method)(f"/api/tasks/{task_id}")
    assert response.status_code == 404


def test_delete_returns_204_and_subsequent_get_404(client, task_factory):
    task = task_factory()
    response = client.delete(f"/api/tasks/{task['id']}")
    assert response.status_code == 204
    assert client.get(f"/api/tasks/{task['id']}").status_code == 404


def test_health_endpoint(client):
    assert client.get("/health").status_code == 200
