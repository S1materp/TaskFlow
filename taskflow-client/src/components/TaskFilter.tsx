type Filter = "all" | "active" | "completed";

type Props = {
  filter: Filter;
  setFilter: (filter: Filter) => void;
  activeCount: number;
};

export default function TaskFilter({
  filter,
  setFilter,
  activeCount,
}: Props) {
  return (
    <section className="toolbar">
      <div className="filters">
        <button
          className={filter === "all" ? "filter-active" : ""}
          onClick={() => setFilter("all")}
        >
          All
        </button>

        <button
          className={filter === "active" ? "filter-active" : ""}
          onClick={() => setFilter("active")}
        >
          Active
        </button>

        <button
          className={filter === "completed" ? "filter-active" : ""}
          onClick={() => setFilter("completed")}
        >
          Completed
        </button>
      </div>

      <span className="task-count">
        {activeCount} active
      </span>
    </section>
  );
}