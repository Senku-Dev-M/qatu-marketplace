import { esc } from "../../utils/format.js";

const categories = ["Todo", "Bebidas", "Snacks", "Despensa", "Limpieza"];

export function catalogFilters(state) {
  const chips = categories
    .map((category) => {
      const active =
        state.category === category || (!state.category && category === "Todo")
          ? "active"
          : "";
      return `<button type="button" class="chip ${active}" data-category="${esc(category)}">${esc(category)}</button>`;
    })
    .join("");

  return `<form id="search-form" class="search-row">
      <label class="search-field"><span>⌕</span><input name="q" placeholder="Busca marcas o productos..." value="${esc(state.query)}" aria-label="Buscar productos"></label>
      <button type="submit" class="button button--dark">Buscar ↗</button>
    </form>
    <div class="filters" role="group" aria-label="Filtrar por categoría">${chips}</div>`;
}
