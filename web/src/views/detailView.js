import { productDetails } from "../components/product/productDetails.js";
import { productReviews } from "../components/product/productReviews.js";
import { esc } from "../utils/format.js";

export function detailView(state) {
  const p = state.product;
  return `<section class="wrap detail">
    <div class="breadcrumbs"><a href="#/">Mercado</a><span>/</span><span>${esc(p.category)}</span><span>/</span><strong>${esc(p.name)}</strong></div>
    ${productDetails(p, state.shopName)}
    ${productReviews(state.reviews)}
  </section>`;
}
