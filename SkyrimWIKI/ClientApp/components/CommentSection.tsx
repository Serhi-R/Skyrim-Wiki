import { useEffect, useState, type FormEvent } from "react";
import {
  createComment,
  deleteComment,
  fetchComments,
  type CommentsResponse,
} from "../api";
import { Stars, Star } from "./Stars";

const LABELS = [
  "Poor",
  "Fair",
  "Good",
  "Great",
  "Excellent",
];

const STRINGS = {
  en: {
    title: "Comments",
    rating: "Your rating",
    placeholder: "Share your experience...",
    post: "Post comment",
    signIn: "Sign in",
    toComment: "to leave a rating and a comment.",
    saved: "Comment saved.",
    loadError: "Could not load the comments.",
    saveError: "Could not save the comment.",
    deleteError: "Could not delete the comment.",
    confirmDelete: "Delete this comment?",
    none: "No comments yet. Be the first.",
    delete: "Delete",
  },
  bg: {
    title: "Коментари",
    rating: "Вашата оценка",
    placeholder: "Споделете впечатленията си...",
    post: "Публикувай коментар",
    signIn: "Влезте",
    toComment: "за да оставите оценка и коментар.",
    saved: "Коментарът е запазен.",
    loadError: "Неуспешно зареждане на коментарите.",
    saveError: "Неуспешно запазване на коментара.",
    deleteError: "Неуспешно изтриване на коментара.",
    confirmDelete: "Изтрийте този коментар?",
    none: "Все още няма коментари. Бъдете първи.",
    delete: "Изтрий",
  },
} as const;

function getLang(): "en" | "bg" {
  const root = document.getElementById("quest-comments");
  const value = root?.dataset.lang;
  return value === "bg" ? "bg" : "en";
}

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

  const t = STRINGS[getLang()];

  const load = () => {
    fetchComments(questId)
      .then((response) => {
        setData(response);
        setError(null);
      })
      .catch(() => setError(t.loadError));
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
      setNotice(t.saved);
      load();
    } catch {
      setError(t.saveError);
    } finally {
      setBusy(false);
    }
  };

  const remove = async (id: number) => {
    if (!window.confirm(t.confirmDelete)) {
      return;
    }

    try {
      await deleteComment(id);
      load();
    } catch {
      setError(t.deleteError);
    }
  };

  return (
    <section className="frame mt-8 p-6">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h2 className="section-title text-lg">
          {t.title}
        </h2>
        <div className="flex items-center gap-2">
          <Stars rating={data?.averageRating ?? 0} />
          <span className="text-sm text-dust">
            {(data?.averageRating ?? 0).toFixed(1)} / 5 ({data?.count ?? 0})
          </span>
        </div>
      </div>
      <hr className="divider-gold mt-3 w-full" />

      {isAuthenticated ? (
        <form onSubmit={submit} className="mt-5 rounded-sm border border-gold/15 bg-black/25 p-4">
          <div className="flex flex-wrap items-center gap-3">
            <span className="meta">
              {t.rating}
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
            placeholder={t.placeholder}
            className="input mt-3"
          />

          <button
            type="submit"
            disabled={busy || text.trim().length < 3}
            className="btn btn-solid mt-3"
          >
            {t.post}
          </button>
        </form>
      ) : (
        <p className="mt-4 text-sm text-dust">
          <a href="/Identity/Account/Login" className="link">
            {t.signIn}
          </a>{" "}
          {t.toComment}
        </p>
      )}

      {notice && <p className="note-ok mt-3 text-sm">{notice}</p>}
      {error && <p className="note-error mt-3">{error}</p>}

      {data && data.items.length === 0 && (
        <p className="empty mt-4">
          {t.none}
        </p>
      )}

      {data && data.items.length > 0 && (
        <ul className="mt-4 space-y-4">
          {data.items.map((comment) => (
            <li key={comment.id} className="rounded-sm border border-gold/15 bg-black/25 p-4">
              <div className="flex flex-wrap items-center justify-between gap-2">
                <p className="break-words font-display text-sm font-semibold text-ivory">{comment.author}</p>
                <div className="flex items-center gap-3">
                  <Stars rating={comment.rating} />
                  {comment.canDelete && (
                    <button
                      type="button"
                      onClick={() => remove(comment.id)}
                      className="link text-xs font-medium text-ember-text hover:text-ember-2"
                    >
                      {t.delete}
                    </button>
                  )}
                </div>
              </div>
              <p className="mt-1 text-xs text-dust">
                {new Date(comment.createdAt).toLocaleString()}
              </p>
              <p className="mt-2 whitespace-pre-line break-words text-sm text-parchment/90">{comment.text}</p>
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}
