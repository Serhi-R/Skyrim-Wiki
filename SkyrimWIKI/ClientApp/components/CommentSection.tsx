import { useEffect, useState, type FormEvent } from "react";
import {
  createComment,
  deleteComment,
  fetchComments,
  type CommentsResponse,
} from "../api";
import { Stars, Star } from "./Stars";

const LABELS = [
  "Poor / Слабо",
  "Fair / Задоволително",
  "Good / Добро",
  "Great / Много добро",
  "Excellent / Отлично",
];

// Loads the comments of one quest from the API and lets a signed in user add
// or delete their own comment with a star rating.
export function CommentSection({
  questId,
  isAuthenticated,
}: {
  questId: number;
  isAuthenticated: boolean;
}) {
  const [data, setData] = useState<CommentsResponse | null>(null);
  const [text, setText] = useState("");
  const [rating, setRating] = useState(5);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [notice, setNotice] = useState<string | null>(null);

  const load = () => {
    fetchComments(questId)
      .then((response) => {
        setData(response);
        setError(null);
      })
      .catch(() => setError("Could not load the comments."));
  };

  useEffect(load, [questId]);

  const submit = async (event: FormEvent) => {
    event.preventDefault();
    setBusy(true);
    setError(null);
    setNotice(null);

    try {
      await createComment({ questId, text, rating });
      setText("");
      setRating(5);
      setNotice("Comment saved. / Коментарът е запазен.");
      load();
    } catch {
      setError("Could not save the comment. / Коментарът не беше запазен.");
    } finally {
      setBusy(false);
    }
  };

  const remove = async (id: number) => {
    if (!window.confirm("Delete this comment? / Да се изтрие ли коментарът?")) {
      return;
    }

    try {
      await deleteComment(id);
      load();
    } catch {
      setError("Could not delete the comment.");
    }
  };

  return (
    <section className="mt-8 rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h2 className="text-lg font-bold text-slate-900">
          <span className="t-en">Comments</span>
          <span className="t-bg">Коментари</span>
        </h2>
        <div className="flex items-center gap-2">
          <Stars rating={data?.averageRating ?? 0} />
          <span className="text-sm text-slate-500">
            {(data?.averageRating ?? 0).toFixed(1)} / 5 ({data?.count ?? 0})
          </span>
        </div>
      </div>

      {isAuthenticated ? (
        <form onSubmit={submit} className="mt-5 rounded-lg bg-slate-50 p-4">
          <div className="flex flex-wrap items-center gap-3">
            <span className="text-sm font-medium text-slate-700">
              <span className="t-en">Your rating</span>
              <span className="t-bg">Твоята оценка</span>
            </span>
            <div className="flex items-center gap-1">
              {[1, 2, 3, 4, 5].map((star) => (
                <button
                  key={star}
                  type="button"
                  onClick={() => setRating(star)}
                  aria-label={`${star} / 5`}
                  title={LABELS[star - 1]}
                  className="transition hover:scale-110"
                >
                  <Star filled={star <= rating} size="h-6 w-6" />
                </button>
              ))}
            </div>
          </div>

          <textarea
            value={text}
            onChange={(event) => setText(event.target.value)}
            rows={3}
            required
            minLength={3}
            maxLength={1000}
            placeholder="Share your experience... / Сподели опита си..."
            className="mt-3 w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none focus:ring-1 focus:ring-emerald-500"
          />

          <button
            type="submit"
            disabled={busy || text.trim().length < 3}
            className="mt-3 rounded-md bg-emerald-600 px-4 py-2 text-sm font-semibold text-white hover:bg-emerald-700 disabled:cursor-not-allowed disabled:opacity-50"
          >
            <span className="t-en">Post comment</span>
            <span className="t-bg">Публикувай</span>
          </button>
        </form>
      ) : (
        <p className="mt-4 text-sm text-slate-500">
          <a
            href="/Identity/Account/Login"
            className="font-medium text-emerald-700 hover:underline"
          >
            <span className="t-en">Sign in</span>
            <span className="t-bg">Влез</span>
          </a>{" "}
          <span className="t-en">to leave a rating and a comment.</span>
          <span className="t-bg">за да оставиш оценка и коментар.</span>
        </p>
      )}

      {notice && <p className="mt-3 text-sm text-emerald-700">{notice}</p>}
      {error && <p className="mt-3 text-sm text-rose-700">{error}</p>}

      {data && data.items.length === 0 && (
        <p className="mt-4 rounded-lg border border-dashed border-slate-300 p-6 text-center text-sm text-slate-500">
          <span className="t-en">No comments yet. Be the first.</span>
          <span className="t-bg">Още няма коментари. Бъди първи.</span>
        </p>
      )}

      {data && data.items.length > 0 && (
        <ul className="mt-4 space-y-4">
          {data.items.map((comment) => (
            <li key={comment.id} className="rounded-lg bg-slate-50 p-4">
              <div className="flex flex-wrap items-center justify-between gap-2">
                <p className="break-words text-sm font-semibold text-slate-800">{comment.author}</p>
                <div className="flex items-center gap-3">
                  <Stars rating={comment.rating} />
                  {comment.canDelete && (
                    <button
                      type="button"
                      onClick={() => remove(comment.id)}
                      className="text-xs font-medium text-rose-600 hover:underline"
                    >
                      <span className="t-en">Delete</span>
                      <span className="t-bg">Изтрий</span>
                    </button>
                  )}
                </div>
              </div>
              <p className="mt-1 text-xs text-slate-400">
                {new Date(comment.createdAt).toLocaleString()}
              </p>
              <p className="mt-2 whitespace-pre-line break-words text-sm text-slate-700">{comment.text}</p>
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}
