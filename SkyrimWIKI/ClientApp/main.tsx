import { createRoot } from "react-dom/client";
import { CommentSection } from "./components/CommentSection";
import { QuestCatalog } from "./components/QuestCatalog";

const STORAGE_KEY = "skyrimwiki.lang";
const root = document.documentElement;

function applyLang(lang: "en" | "bg"): void {
  root.dataset.lang = lang;
  root.lang = lang;
}

const saved = localStorage.getItem(STORAGE_KEY);
if (saved === "en" || saved === "bg") {
  applyLang(saved);
}

document.getElementById("lang-toggle")?.addEventListener("click", () => {
  const next = root.dataset.lang === "en" ? "bg" : "en";
  applyLang(next);
  localStorage.setItem(STORAGE_KEY, next);
});

// React islands: each one mounts only if its placeholder is on the page.
const catalog = document.getElementById("quest-catalog");
if (catalog) {
  createRoot(catalog).render(<QuestCatalog />);
}

const comments = document.getElementById("quest-comments");
if (comments) {
  createRoot(comments).render(
    <CommentSection
      questId={Number(comments.dataset.questId)}
      isAuthenticated={comments.dataset.auth === "true"}
    />,
  );
}

