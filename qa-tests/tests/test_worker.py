"""Check that the background worker reports task statistics in application logs.

Requires TASKFLOW_API_LOG_PATH pointing at the running API's captured log file.
The worker currently logs every 60 seconds, so this check may take up to 75 seconds.
"""
import os
import time

import pytest


@pytest.mark.integration
def test_background_worker_emits_statistics():
    path = os.getenv("TASKFLOW_API_LOG_PATH")
    if not path:
        pytest.skip("Set TASKFLOW_API_LOG_PATH to enable worker log verification")

    deadline = time.monotonic() + 75
    while time.monotonic() < deadline:
        try:
            with open(path, encoding="utf-8", errors="replace") as stream:
                if "Incomplete tasks:" in stream.read():
                    return
        except FileNotFoundError:
            pass
        time.sleep(2)
    pytest.fail("Background worker did not log 'Incomplete tasks:' within 75 seconds")
