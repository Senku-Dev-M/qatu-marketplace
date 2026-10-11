import { productForm } from "../components/seller/productForm.js";
import { sellerApplication } from "../components/seller/sellerApplication.js";
import { sellerProductList } from "../components/seller/sellerProductList.js";
import { esc } from "../utils/format.js";

export function sellerView(state) {
  const profile = state.profile;
  if (!profile?.isSeller) return sellerApplication();

  return `<section class="wrap inner-section">
    <div class="heading-row">
      <div>
        <p class="eyebrow">ESPACIO VENDEDOR</p>
        <h1>Mi tienda<span class="period">.</span></h1>
        <p class="section-lead">${esc(state.shop?.name || "Tu espacio en Qatu")}</p>
      </div>
      <span class="shop-seal">QATU<br>VENDEDOR ✳</span>
    </div>
    <div class="seller-columns">
      ${productForm(state.editing)}
      ${sellerProductList(state.products)}
    </div>
  </section>`;
}
