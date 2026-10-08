import { esc } from "../utils/format.js";

const colors = {
  "Coca-Cola": ["#bb3028", "#f8eee1"],
  Pepsi: ["#1759a4", "#f9f0e6"],
  Sprite: ["#377b50", "#f4e6a2"],
  Fanta: ["#df712e", "#fff0d4"],
  Monster: ["#1e2b2a", "#b5d761"],
  Powerade: ["#2a568f", "#dce8ec"],
  Activade: ["#9a345b", "#f3dce2"],
  Aquarius: ["#5c9db2", "#eef8e7"],
  "Del Valle": ["#e4a552", "#46302b"],
  Vital: ["#4d97b4", "#e8f5f2"],
  Mendocina: ["#7296a7", "#f0f5ed"],
  Ovando: ["#be8749", "#fff2d9"],
  Lazzaroni: ["#a64b38", "#f9eccf"],
  Oreo: ["#2f5e9b", "#eaf0f5"],
  "Mabel's": ["#b4424b", "#f9e9d8"],
  Breick: ["#745248", "#f1e1c9"],
  Ola: ["#4b8b74", "#f4e7c0"],
};

export function productArt(product, detail = false) {
  const [main, pale] = colors[product.brand] || ["#7a695b", "#f4ebdb"];
  const unit = (product.unit || "").toLowerCase();
  const shape =
    unit === "lata"
      ? "can"
      : unit === "botella"
        ? "bottle"
        : unit === "bolsa"
          ? "bag"
          : unit === "tableta"
            ? "bar"
            : "pack";
  const shortBrand = (product.brand || product.category || "Qatu").trim();
  return `<div class="product-art ${detail ? "product-art--detail" : ""}" style="--pack:${main};--pack-light:${pale}" role="img" aria-label="Ilustración de ${esc(product.name)} ${esc(product.presentation || "")}">
    <div class="product-art-glow"></div><div class="product-package product-package--${shape}"><div class="package-band"><span>${esc(shortBrand)}</span><small>${esc(product.presentation || "")}</small></div></div>
    <span class="product-art-caption">${esc(product.category || "Mercado")}</span>
  </div>`;
}
