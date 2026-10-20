"""Real-browser end-to-end scenarios for the React frontend.

The test creates unique records and removes them even when assertions fail.
"""
import os
import uuid

import pytest
from playwright.sync_api import expect, sync_playwright


pytestmark = pytest.mark.integration
FRONTEND_URL = os.getenv("TASKFLOW_FRONTEND_URL", "http://127.0.0.1:5173")


@pytest.fixture()
def browser_page():
    with sync_playwright() as playwright:
        browser = playwright.chromium.launch(headless=True)
        page = browser.new_page()
        yield page
        browser.close()


@pytest.fixture()
def ui_task(browser_page, client):
    title = f"e2e-{uuid.uuid4().hex[:12]}"
    created_ids = []
    browser_page.goto(FRONTEND_URL)
    expect(browser_page.get_by_role("heading", name="TaskFlow")).to_be_visible()
    browser_page.get_by_placeholder("What needs to be done?").fill(title)
    browser_page.get_by_role("button", name="Add task").click()
    task = browser_page.locator("article.task").filter(has_text=title)
    expect(task).to_be_visible(timeout=10000)
    for item in client.get("/api/tasks").json():
        if item["title"] == title:
            created_ids.append(item["id"])
    assert created_ids, "Created task was not found in API"
    yield title, task
    for task_id in created_ids:
        response = client.delete(f"/api/tasks/{task_id}")
        assert response.status_code in (204, 404), response.text


def test_create_task_through_ui_is_visible_in_api(ui_task, client):
    title, task = ui_task
    expect(task.get_by_role("heading", name=title)).to_be_visible()
    assert any(item["title"] == title for item in client.get("/api/tasks").json())


def test_complete_task_and_filter(ui_task, browser_page, client):
    title, task = ui_task
    task.get_by_role("button", name=f"Complete {title}").click()
    expect(task.get_by_text("Completed", exact=True)).to_be_visible()
    browser_page.get_by_role("button", name="Active", exact=True).click()
    expect(browser_page.locator("article.task").filter(has_text=title)).to_have_count(0)
    browser_page.get_by_role("button", name="Completed", exact=True).click()
    expect(browser_page.locator("article.task").filter(has_text=title)).to_be_visible()
    assert next(item for item in client.get("/api/tasks").json() if item["title"] == title)["isCompleted"]


def test_delete_task_through_ui(ui_task, browser_page, client):
    title, task = ui_task
    task.get_by_role("button", name="Delete").click()
    expect(browser_page.locator("article.task").filter(has_text=title)).to_have_count(0)
    assert all(item["title"] != title for item in client.get("/api/tasks").json())


def test_blank_task_cannot_be_submitted(browser_page):
    browser_page.goto(FRONTEND_URL)
    expect(browser_page.get_by_role("button", name="Add task")).to_be_disabled()
    browser_page.get_by_placeholder("What needs to be done?").fill("   ")
    expect(browser_page.get_by_role("button", name="Add task")).to_be_disabled()
