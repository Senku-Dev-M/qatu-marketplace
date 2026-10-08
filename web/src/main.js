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
import { initializeAuth } from "./auth/session.js";
import { shell } from "./components/shell.js";
import { toast } from "./components/toast.js";
import { appState } from "./state/appState.js";
import { esc } from "./utils/format.js";
import { currentPath, go } from "./utils/navigation.js";
import { AdminViewModel } from "./viewmodels/AdminViewModel.js";
import { AuthViewModel } from "./viewmodels/AuthViewModel.js";
import { CatalogViewModel } from "./viewmodels/CatalogViewModel.js";
import { CheckoutViewModel } from "./viewmodels/CheckoutViewModel.js";
import { InboxViewModel } from "./viewmodels/InboxViewModel.js";
import { OrdersViewModel } from "./viewmodels/OrdersViewModel.js";
import { PolicyViewModel } from "./viewmodels/PolicyViewModel.js";
import { ProductViewModel } from "./viewmodels/ProductViewModel.js";
import { SellerViewModel } from "./viewmodels/SellerViewModel.js";

let active = null;
let lastPath = null;

function route(path) {
  const clean = path.split("?")[0];
  if (clean === "/") return new CatalogViewModel();
  if (clean === "/acceso") return new AuthViewModel();
  if (clean === "/privacidad") return new PolicyViewModel("privacy");
  if (clean === "/condiciones") return new PolicyViewModel("terms");
  if (clean === "/pedidos") return new OrdersViewModel();
  if (clean === "/vender") return new SellerViewModel();
  if (clean === "/admin") return new AdminViewModel();
  if (clean === "/mensajes") return new InboxViewModel(null);
  if (clean.startsWith("/mensajes/"))
    return new InboxViewModel(clean.split("/")[2]);
  if (clean.startsWith("/producto/"))
    return new ProductViewModel(clean.split("/")[2]);
  if (clean.startsWith("/pago/"))
    return new CheckoutViewModel(clean.split("/")[2], path.split("?")[1] || "");
  return new CatalogViewModel();
}

function render() {
  const path = currentPath();
  if (path === lastPath && active) {
    const actions = document.querySelector(".header-actions");
    actions.innerHTML = appState.profile
      ? `<span class="hello">Hola, ${esc(appState.profile.displayName)}</span><button type="button" class="text-button" id="sign-out">Salir ↗</button>`
      : '<a class="button button--small" href="#/acceso">Entrar ↗</a>';
    bindShell();
    return;
  }
  active?.destroy();
  lastPath = path;
  document.querySelector("#app").innerHTML = shell("");
  document.querySelectorAll("[data-nav]").forEach((link) => {
    if (link.dataset.nav === path) link.classList.add("active");
  });
  bindShell();
  active = route(path);
  active.mount(document.querySelector("#page"));
  window.scrollTo({ top: 0, behavior: "instant" });
}

function bindShell() {
  document.querySelector("#sign-out")?.addEventListener("click", async () => {
    await appState.client.auth.signOut();
    toast("Sesión cerrada.");
    lastPath = null;
    go("/");
  });
}

window.addEventListener("hashchange", render);
appState.subscribe(() => {
  lastPath = null;
  render();
});
document.querySelector("#app").innerHTML = shell(
  '<div class="wrap state-card"><span class="spinner"></span><p>Preparando Qatu...</p></div>',
);
initializeAuth()
  .then(render)
  .catch((error) => {
    document.querySelector("#page").innerHTML =
      `<div class="wrap state-card"><h1>Qatu necesita configuración</h1><p>${esc(error.message)}</p><p>Revisa la API y la conexión con Supabase.</p></div>`;
  });
