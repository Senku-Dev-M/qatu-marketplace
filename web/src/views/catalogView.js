import { esc } from "../utils/format.js";
import { productCard } from "../components/productCard.js";
import { empty } from "../components/empty.js";

export function catalogView(state) {
  const categories = ["Todo", "Bebidas", "Snacks", "Despensa", "Limpieza"];
  return `<section class="hero wrap">
      <div class="hero-copy"><p class="eyebrow"><span class="eyebrow-line"></span> EL KATU DE TODOS LOS DÍAS</p>
        <h1>Lo que buscas,<br><em>a la vuelta del mercado.</em></h1>
        <p class="hero-sub">Bebidas, alimentos y productos cotidianos de tiendas cercanas. Explora, conversa y compra en un solo lugar.</p>
        <a class="button button--dark" href="#productos">Recorrer el mercado <span>↗</span></a>
        <div class="hero-foot"><span>✳ Precios en bolivianos</span><span>✳ Pago de demostración</span></div>
      </div>
      <div class="hero-art" aria-hidden="true"><img src="/art/market-still-life.png" alt="" /><span class="art-note">Tu mercado de cada día.</span></div>
    </section>
    <section class="catalog-section wrap" id="productos">
      <div class="section-heading"><div><p class="eyebrow">HECHO PARA TU DÍA</p><h2>El mercado de hoy<span class="period">.</span></h2></div><p>Encuentra lo que necesitas, a tu ritmo.</p></div>
      <form id="search-form" class="search-row"><label class="search-field"><span>⌕</span><input name="q" placeholder="Busca marcas o productos..." value="${esc(state.query)}" aria-label="Buscar productos"></label><button type="submit" class="button button--dark">Buscar ↗</button></form>
      <div class="filters" role="group" aria-label="Filtrar por categoría">${categories.map((category) => `<button type="button" class="chip ${state.category === category || (!state.category && category === "Todo") ? "active" : ""}" data-category="${esc(category)}">${esc(category)}</button>`).join("")}</div>
      <div class="catalog-count"><span>${state.products.length} ${state.products.length === 1 ? "producto" : "productos"} disponibles</span><span>PRECIOS EN Bs · BOLIVIA</span></div>
      ${state.products.length ? `<div class="product-grid">${state.products.map(productCard).join("")}</div>` : empty("No encontramos productos", "Prueba otra marca, categoría o término de búsqueda.")}
    </section>
    <section class="seller-banner wrap"><div><p class="eyebrow">EL MERCADO CRECE CONTIGO</p><h2>Tu tienda también<br>tiene lugar aquí.</h2></div><a class="button button--light" href="#/vender">Quiero vender <span>↗</span></a></section>`;
}
