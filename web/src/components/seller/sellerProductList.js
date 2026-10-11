import { empty } from "../empty.js";
import { sellerProductCard } from "./sellerProductCard.js";

export function sellerProductList(products) {
  const content = products.length
    ? `<div class="seller-products">${products.map(sellerProductCard).join("")}</div>`
    : empty(
        "Comienza con tu primer producto",
        "Completa el formulario para crear tu catálogo.",
      );

  return `<div>
    <h2 class="list-title">Tus productos <span>${products.length}</span></h2>
    ${content}
  </div>`;
}
