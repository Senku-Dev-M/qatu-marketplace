import { esc, money } from "../utils/format.js";
import { productArt } from "../components/productArt.js";
import { seedImage } from "../data/seedImages.js";

export function productCard(product, index = 0) {
  const source =
    seedImage(product) ||
    (product.hasImage ? `/api/products/${product.id}/image` : null);
  const image = source
    ? `<img src="${source}" alt="${esc(product.name)}" loading="${index < 4 ? "eager" : "lazy"}">`
    : productArt(product);
  return `<a class="product-card" href="#/producto/${product.id}" style="--delay:${Math.min(index, 9) * 55}ms">
      <div class="product-visual visual-${index % 5}">${image}<span class="product-tag">${esc(product.category)}</span></div>
      <div class="product-meta"><span>${esc(product.brand || product.shopName)}</span><span>${esc(product.presentation || "")}</span></div>
      <h3>${esc(product.name)}</h3><div class="product-bottom"><strong>${money(product.priceBob)}</strong><span>${product.stock > 0 ? `por ${esc(product.unit || "unidad")}` : "Agotado"}</span></div>
    </a>`;
}
