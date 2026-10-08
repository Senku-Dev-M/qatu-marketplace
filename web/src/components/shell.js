import { appState } from "../state/appState.js";
import { esc } from "../utils/format.js";

export function shell(content) {
  const profile = appState.profile;
  return `
    <div class="site-shell">
      <div class="announcement"><span>QATU · TU KATU DIGITAL</span><span>Lo bueno del mercado, cerca de ti</span><span>BOLIVIA · Bs</span></div>
      <header class="header wrap">
        <a class="brand" href="#/" aria-label="Qatu, inicio"><span class="brand-mark"><img src="/art/qatu-llama-mark.png" alt="" /></span><span>qatu<small>EL MERCADO DE CADA DÍA</small></span></a>
        <nav class="nav" aria-label="Navegación principal">
          <a href="#/" data-nav="/">Explorar</a>
          ${profile ? `<a href="#/pedidos">Mis pedidos</a><a href="#/mensajes">Mensajes</a><a href="#/vender">${profile.isSeller ? "Mi tienda" : "Quiero vender"}</a>` : ""}
          ${profile?.isAdmin ? '<a href="#/admin">Administrar</a>' : ""}
        </nav>
        <div class="header-actions">${profile ? `<span class="hello">Hola, ${esc(profile.displayName)}</span><button type="button" class="text-button" id="sign-out">Salir ↗</button>` : '<a class="button button--small" href="#/acceso">Entrar <span>↗</span></a>'}</div>
      </header>
      <main id="page" class="page">${content}</main>
      <footer class="footer"><div class="wrap footer-inner"><div><strong>qatu<span>.</span></strong><p>Tu katu digital, con el espíritu del Altiplano.</p></div><p>Proyecto académico · Desarrollo de Software 4</p><div class="footer-links"><a href="#/privacidad">Privacidad</a><a href="#/condiciones">Condiciones</a><a href="#/">Volver arriba ↑</a></div></div></footer>
      <div id="toasts" class="toasts" role="status" aria-live="polite"></div>
    </div>`;
}
