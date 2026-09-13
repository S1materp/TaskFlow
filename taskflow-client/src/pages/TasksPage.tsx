import { useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  getTasks,
  createTask,
  completeTask,
  deleteTask,
} from "../api/tasks";
import TaskForm from "../components/TaskForm";
import TaskFilter from "../components/TaskFilter";
import TaskItem from "../components/TaskItem";
import type { Task } from "../types/task";

type Filter = "all" | "active" | "completed";

export default function TasksPage() {
  const [title, setTitle] = useState("");
  const [filter, setFilter] = useState<Filter>("all");

  const queryClient = useQueryClient();

  const {
    data: tasks = [],
    isLoading,
    isError,
  } = useQuery({
    queryKey: ["tasks"],
    queryFn: getTasks,
  });

  const createMutation = useMutation({
    mutationFn: createTask,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["tasks"] });
      setTitle("");
    },
  });

  const completeMutation = useMutation({
    mutationFn: completeTask,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["tasks"] });
    },
  });

  const deleteMutation = useMutation({
    mutationFn: deleteTask,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["tasks"] });
    },
  });

  const handleCreateTask = () => {
    const trimmedTitle = title.trim();

    if (!trimmedTitle) return;

    createMutation.mutate(trimmedTitle);
  };

  const filteredTasks = tasks.filter((task: Task) => {
    if (filter === "active") {
      return !task.isCompleted;
    }

    if (filter === "completed") {
      return task.isCompleted;
    }

    return true;
  });

  const activeCount = tasks.filter(
    (task: Task) => !task.isCompleted
  ).length;

  if (isLoading) {
    return (
      <main className="container">
        <p className="state-message">Loading tasks...</p>
      </main>
    );
  }

  if (isError) {
    return (
      <main className="container">
        <p className="state-message error-message">
          Failed to load tasks. Check the API connection.
        </p>
      </main>
    );
  }

  return (
    <main className="container">
      <header className="header">
        <p className="eyebrow">FULL STACK TASK MANAGER</p>

        <h1>TaskFlow</h1>

        <p className="subtitle">
          Organize tasks with React, TypeScript and ASP.NET Core
        </p>
      </header>

      <TaskForm
        title={title}
        setTitle={setTitle}
        onSubmit={handleCreateTask}
        isPending={createMutation.isPending}
      />

      <TaskFilter
        filter={filter}
        setFilter={setFilter}
        activeCount={activeCount}
      />

      <section className="tasks">
        {filteredTasks.length === 0 ? (
          <div className="empty-state">
            <h2>No tasks here</h2>
            <p>Create a new task or change the current filter.</p>
          </div>
        ) : (
          filteredTasks.map((task) => (
            <TaskItem
              key={task.id}
              task={task}
              onComplete={(task) => completeMutation.mutate(task)}
              onDelete={(id) => deleteMutation.mutate(id)}
              isUpdating={completeMutation.isPending}
              isDeleting={deleteMutation.isPending}
            />
          ))
        )}
      </section>
    </main>
  );
}