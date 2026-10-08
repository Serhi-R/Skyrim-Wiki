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
