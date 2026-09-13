import type { Task } from "../types/task";

type Props = {
  task: Task;
  onComplete: (task: Task) => void;
  onDelete: (id: number) => void;
  isUpdating: boolean;
  isDeleting: boolean;
};

export default function TaskItem({
  task,
  onComplete,
  onDelete,
  isUpdating,
  isDeleting,
}: Props) {
  return (
    <article
      className={`task ${task.isCompleted ? "task-completed" : ""}`}
    >
      <div className="task-content">
        <button
          className={`check-button ${task.isCompleted ? "checked" : ""}`}
          disabled={task.isCompleted || isUpdating}
          onClick={() => onComplete(task)}
          aria-label={`Complete ${task.title}`}
        >
          {task.isCompleted ? "✓" : ""}
        </button>

        <div>
          <h3>{task.title}</h3>

          <span className={task.isCompleted ? "done" : "pending"}>
            {task.isCompleted ? "Completed" : "In progress"}
          </span>
        </div>
      </div>

      <button
        className="delete-button"
        onClick={() => onDelete(task.id)}
        disabled={isDeleting}
      >
        Delete
      </button>
    </article>
  );
}