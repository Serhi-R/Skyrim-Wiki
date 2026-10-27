import { createRoot } from "react-dom/client";
import { CommentSection } from "./components/CommentSection";
import { QuestCatalog } from "./components/QuestCatalog";

const MAP: Record<string, string> = {
  "Input.Email": "_em",
  "Input.Password": "_pw",
  "Input.ConfirmPassword": "_cp",
  "Input.RememberMe": "_rm",
  "__RequestVerificationToken": "_tok",
  "ReturnUrl": "_ret",
};

function obfuscateAuthForms() {
  const forms = document.querySelectorAll<HTMLFormElement>("form#account, form#registerForm");
  forms.forEach((form) => {
    form.addEventListener("submit", async (e) => {
      e.preventDefault();

      const submitBtn = form.querySelector<HTMLButtonElement>("button[type=submit], input[type=submit]");
      if (submitBtn) submitBtn.disabled = true;

      // Backup and rename field names right before sending
      const backup: Array<{ el: Element; name: string }> = [];
      form.querySelectorAll<HTMLElement>("[name]").forEach((el) => {
        const current = el.getAttribute("name");
        if (!current) return;
        backup.push({ el, name: current });
        const mapped = MAP[current];
        if (mapped) {
          el.setAttribute("name", mapped);
        } else if (current.startsWith("Input.")) {
          el.setAttribute("name", current.replace("Input.", "_"));
        }
      });

      const fd = new FormData(form);
      const params = new URLSearchParams();
      for (const [key, value] of fd.entries()) {
        params.append(key, value.toString());
      }

      // Restore original names back to DOM so validation/UI stays normal
      backup.forEach(({ el, name }) => {
        el.setAttribute("name", name);
      });

      try {
        const res = await fetch(form.action, {
          method: "POST",
          body: params,
          headers: {
            "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8",
            "X-Requested-With": "XMLHttpRequest",
          },
          credentials: "same-origin",
          redirect: "follow",
        });

        if (res.redirected) {
          window.location.href = res.url;
          return;
        }

        // Razor Pages returns the full page on validation errors - reload to render it
        const text = await res.text();
        if (text.includes("asp-validation") || text.includes("validation-summary")) {
          document.open();
          document.write(text);
          document.close();
          return;
        }

        window.location.reload();
      } catch {
        if (submitBtn) submitBtn.disabled = false;
        // Last resort
        backup.forEach(({ el, name }) => el.setAttribute("name", name));
        form.submit();
      }
    });
  });
}

obfuscateAuthForms();

// Mobile navigation: below the lg breakpoint the links are hidden behind a
// button, and opening adds both display classes the layout expects.
const navToggle = document.getElementById("nav-toggle");
const navMenu = document.getElementById("nav-menu");

navToggle?.addEventListener("click", () => {
  if (!navMenu) {
    return;
  }

  const opening = navMenu.classList.contains("hidden");
  navMenu.classList.toggle("hidden", !opening);
  navMenu.classList.toggle("flex", opening);
  navToggle.setAttribute("aria-expanded", String(opening));
});

// Password fields expose a Show/Hide button that targets the input by id.
document.addEventListener("click", (event) => {
  const button = (event.target as HTMLElement).closest<HTMLButtonElement>(
    "[data-toggle-password]",
  );
  if (!button) {
    return;
  }

  const selector = button.dataset.togglePassword;
  const input = selector
    ? document.querySelector<HTMLInputElement>(selector)
    : null;
  if (!input) {
    return;
  }

  const revealed = input.type === "password";
  input.type = revealed ? "text" : "password";
  button.textContent = revealed ? "Hide" : "Show";
  button.setAttribute("aria-pressed", String(revealed));
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
