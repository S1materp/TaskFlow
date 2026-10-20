"""Fixtures for testing the running TaskFlow application.

Never point these tests at production: they create and delete task records.
"""
import os
import time
import uuid

import httpx
import psycopg
import pytest


API_URL = os.getenv("TASKFLOW_API_URL", "http://127.0.0.1:5019")
DATABASE_URL = os.getenv(
    "TASKFLOW_DATABASE_URL",
    "postgresql://taskflow:taskflow123@127.0.0.1:55432/taskflow",
)


@pytest.fixture(scope="session")
def client():
    with httpx.Client(base_url=API_URL, timeout=10.0) as api:
        for attempt in range(30):
            try:
                response = api.get("/health")
                if response.status_code == 200:
                    break
            except httpx.RequestError:
                pass
            time.sleep(1)
        else:
            pytest.fail("TaskFlow API /health did not become healthy within 30 seconds")
        yield api


@pytest.fixture(scope="session")
def db():
    with psycopg.connect(DATABASE_URL, autocommit=True) as connection:
        yield connection


@pytest.fixture()
def task_factory(client):
    created_ids = []

    def create(title=None, is_completed=False):
        title = title or f"qa-{uuid.uuid4().hex[:12]}"
        response = client.post(
            "/api/tasks", json={"title": title, "isCompleted": is_completed}
        )
        assert response.status_code == 201, response.text
        task = response.json()
        created_ids.append(task["id"])
        return task

    yield create

    for task_id in created_ids:
        response = client.delete(f"/api/tasks/{task_id}")
        assert response.status_code in (204, 404), response.text
