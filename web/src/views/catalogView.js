import { catalogFilters } from "../components/catalog/catalogFilters.js";
import { catalogHero } from "../components/catalog/catalogHero.js";
import { sellerBanner } from "../components/catalog/sellerBanner.js";
import { empty } from "../components/empty.js";
import { productCard } from "../components/productCard.js";

export function catalogView(state) {
  return `${catalogHero()}
    <section class="catalog-section wrap" id="productos">
      <div class="section-heading"><div><p class="eyebrow">HECHO PARA TU DÍA</p><h2>El mercado de hoy<span class="period">.</span></h2></div><p>Encuentra lo que necesitas, a tu ritmo.</p></div>
      ${catalogFilters(state)}
      <div class="catalog-count"><span>${state.products.length} ${state.products.length === 1 ? "producto" : "productos"} disponibles</span><span>PRECIOS EN Bs · BOLIVIA</span></div>
      ${state.products.length ? `<div class="product-grid">${state.products.map(productCard).join("")}</div>` : empty("No encontramos productos", "Prueba otra marca, categoría o término de búsqueda.")}
    </section>
    ${sellerBanner()}`;
}
