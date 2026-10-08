import { esc, money } from "../utils/format.js";
import { productArt } from "../components/productArt.js";
import { seedImage } from "../data/seedImages.js";

export function detailView(state) {
  const p = state.product;
  const imageSource =
    seedImage(p) || (p.imagePath ? `/api/products/${p.id}/image` : null);
  const image = imageSource
    ? `<img src="${imageSource}" alt="${esc(p.name)}">`
    : productArt(p, true);
  return `<section class="wrap detail"><div class="breadcrumbs"><a href="#/">Mercado</a><span>/</span><span>${esc(p.category)}</span><span>/</span><strong>${esc(p.name)}</strong></div>
    <div class="detail-grid"><div class="detail-image">${image}</div><div class="detail-info"><p class="eyebrow">${esc(p.brand || state.shopName || "TIENDA QATU")}</p><h1>${esc(p.name)}</h1><p class="detail-price">${money(p.priceBob)} <small>/ ${esc(p.unit || "unidad")}</small></p><p class="detail-description">${esc(p.description)}</p><div class="detail-facts"><span>✳ ${esc(p.category)}</span><span>▣ ${esc(p.presentation || "Presentación individual")}</span><span>◒ ${p.stock} en existencia</span></div><p class="detail-shop">Vendido por ${esc(state.shopName || "Tienda Qatu")}</p>
      ${p.stock > 0 ? `<form id="buy-form" class="buy-row"><label>Cantidad<input name="quantity" type="number" min="1" max="${p.stock}" value="1" required></label><button class="button button--dark" type="submit">Comprar ahora ↗</button></form>` : '<p class="stock-out">Agotado por ahora.</p>'}
      <button id="open-chat" class="outline-button" type="button">Consultar al vendedor <span>↗</span></button><p class="detail-disclaimer">El pago se simula para fines del curso. No se solicitan tarjetas.</p></div></div>
    <div class="detail-lower"><div><p class="eyebrow">LA COMUNIDAD OPINA</p><h2>Reseñas del producto</h2></div>${state.reviews.length ? `<div class="review-list">${state.reviews.map((r) => `<article class="review"><strong>${"★".repeat(r.stars)}${"☆".repeat(5 - r.stars)}</strong><p>${esc(r.comment || "Buena experiencia de compra.")}</p></article>`).join("")}</div>` : '<p class="muted">Aún no hay reseñas. Las opiniones aparecen tras una compra confirmada.</p>'}</div></section>`;
}
