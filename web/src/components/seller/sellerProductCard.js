import { esc, money } from "../../utils/format.js";

const icons = { Bebidas: "◒", Snacks: "✦", Despensa: "▥", Limpieza: "✳" };
const statusLabel = {
  Published: "Publicado",
  Hidden: "Oculto",
  Draft: "Borrador",
};

export function sellerProductCard(product) {
  const nextStatus = product.status === "Published" ? "Hidden" : "Published";
  const statusAction = product.status === "Published" ? "Ocultar" : "Publicar";

  return `<article class="seller-product">
    <div class="seller-product-icon">${icons[product.category] || "✦"}</div>
    <div class="seller-product-main">
      <h3>${esc(product.name)}</h3>
      <p>${esc(product.brand || product.category)} · ${esc(product.presentation || product.unit || "unidad")}</p>
      <p>${money(product.priceBob)} · ${product.stock} disponibles</p>
      <small>${statusLabel[product.status]}</small>
    </div>
    <div class="seller-product-actions">
      <button type="button" data-edit="${product.id}">Editar</button>
      <button type="button" data-status="${product.id}" data-next="${nextStatus}">${statusAction}</button>
      <label class="upload-label">Imagen<input type="file" accept="image/png,image/jpeg,image/webp" data-upload="${product.id}"></label>
    </div>
  </article>`;
}
