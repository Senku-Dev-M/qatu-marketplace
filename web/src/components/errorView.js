import { esc } from "../utils/format.js";

export function errorView(message) {
  return `<div class="wrap state-card"><span class="state-icon">!</span><h1>Algo no salió como esperábamos</h1><p>${esc(message)}</p><a class="button" href="#/">Volver al catálogo</a></div>`;
}
