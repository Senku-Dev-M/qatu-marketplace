import "@fontsource/fraunces/400.css";
import "@fontsource/fraunces/600.css";
import "@fontsource/dm-sans/400.css";
import "@fontsource/dm-sans/500.css";
import "@fontsource/dm-sans/700.css";
import "./styles/foundation.css";
import "./styles/catalog.css";
import "./styles/shared.css";
import "./styles/auth.css";
import "./styles/detail.css";
import "./styles/orders-seller.css";
import "./styles/admin-chat.css";
import "./styles/responsive.css";
import "./styles/theme-foundation.css";
import "./styles/theme-hero.css";
import "./styles/theme-catalog.css";
import "./styles/theme-shared.css";
import "./styles/theme-auth.css";
import "./styles/theme-detail.css";
import "./styles/theme-dashboard.css";
import "./styles/theme-responsive.css";
import { shell } from "./components/shell.js";
import { createApplication } from "./composition/createApplication.js";
import { ViewModelFactory } from "./routing/ViewModelFactory.js";
import { esc } from "./utils/format.js";
import { currentPath } from "./utils/navigation.js";

const application = createApplication();
const viewModelFactory = new ViewModelFactory(application);
let activeViewModel = null;
let lastPath = null;

function render() {
  const path = currentPath();
  if (path === lastPath && activeViewModel) {
    refreshHeader();
    return;
  }

  activeViewModel?.destroy();
  lastPath = path;
  document.querySelector("#app").innerHTML = shell(
    "",
    application.state.profile,
  );
  document.querySelectorAll("[data-nav]").forEach((link) => {
    if (link.dataset.nav === path) link.classList.add("active");
  });
  bindShell();
  activeViewModel = viewModelFactory.create(path);
  activeViewModel.mount(document.querySelector("#page"));
  window.scrollTo({ top: 0, behavior: "instant" });
}

function refreshHeader() {
  const actions = document.querySelector(".header-actions");
  if (!actions) return;
  const profile = application.state.profile;
  actions.innerHTML = profile
    ? `<span class="hello">Hola, ${esc(profile.displayName)}</span><button type="button" class="text-button" id="sign-out">Salir ↗</button>`
    : '<a class="button button--small" href="#/acceso">Entrar ↗</a>';
  bindShell();
}

function bindShell() {
  document
    .querySelector("#sign-out")
    ?.addEventListener("click", () => application.auth.signOut());
}

function showConfigurationError(error) {
  const page = document.querySelector("#page");
  if (!page) return;
  page.innerHTML = `<div class="wrap state-card"><h1>Qatu necesita configuración</h1><p>${esc(error.message)}</p><p>Revisa la API y la conexión con Supabase.</p></div>`;
}

window.addEventListener("hashchange", render);
application.state.subscribe(() => {
  lastPath = null;
  render();
});
document.querySelector("#app").innerHTML = shell(
  '<div class="wrap state-card"><span class="spinner"></span><p>Preparando Qatu...</p></div>',
);
application.auth.initialize().then(render).catch(showConfigurationError);
