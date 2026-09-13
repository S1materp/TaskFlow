type Props = {
  title: string;
  setTitle: (value: string) => void;
  onSubmit: () => void;
  isPending: boolean;
};

export default function TaskForm({
  title,
  setTitle,
  onSubmit,
  isPending,
}: Props) {
  return (
    <section className="create-task">
      <input
        value={title}
        onChange={(e) => setTitle(e.target.value)}
        onKeyDown={(e) => {
          if (e.key === "Enter") {
            onSubmit();
          }
        }}
        placeholder="What needs to be done?"
      />

      <button
        onClick={onSubmit}
        disabled={isPending || !title.trim()}
      >
        {isPending ? "Adding..." : "Add task"}
      </button>
    </section>
  );
}