import { seedImage } from "../../data/seedImages.js";
import { esc, money } from "../../utils/format.js";
import { productArt } from "../productArt.js";

function productImage(product) {
  const source =
    seedImage(product) ||
    (product.imagePath ? `/api/products/${product.id}/image` : null);
  return source
    ? `<img src="${source}" alt="${esc(product.name)}">`
    : productArt(product, true);
}

export function productDetails(product, shopName) {
  const purchase =
    product.stock > 0
      ? `<form id="buy-form" class="buy-row"><label>Cantidad<input name="quantity" type="number" min="1" max="${product.stock}" value="1" required></label><button class="button button--dark" type="submit">Comprar ahora ↗</button></form>`
      : '<p class="stock-out">Agotado por ahora.</p>';

  return `<div class="detail-grid">
    <div class="detail-image">${productImage(product)}</div>
    <div class="detail-info">
      <p class="eyebrow">${esc(product.brand || shopName || "TIENDA QATU")}</p>
      <h1>${esc(product.name)}</h1>
      <p class="detail-price">${money(product.priceBob)} <small>/ ${esc(product.unit || "unidad")}</small></p>
      <p class="detail-description">${esc(product.description)}</p>
      <div class="detail-facts"><span>✳ ${esc(product.category)}</span><span>▣ ${esc(product.presentation || "Presentación individual")}</span><span>◒ ${product.stock} en existencia</span></div>
      <p class="detail-shop">Vendido por ${esc(shopName || "Tienda Qatu")}</p>
      ${purchase}
      <button id="open-chat" class="outline-button" type="button">Consultar al vendedor <span>↗</span></button>
      <p class="detail-disclaimer">El pago se simula para fines del curso. No se solicitan tarjetas.</p>
    </div>
  </div>`;
}
