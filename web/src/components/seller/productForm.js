import { esc } from "../../utils/format.js";

const categories = ["Bebidas", "Snacks", "Despensa", "Limpieza", "Otros"];
const units = [
  "botella",
  "lata",
  "paquete",
  "bolsa",
  "tubo",
  "tableta",
  "unidad",
];

function options(values, selected) {
  return values
    .map(
      (value) =>
        `<option ${selected === value ? "selected" : ""}>${value}</option>`,
    )
    .join("");
}

export function productForm(product) {
  return `<form id="product-form" class="paper-card form-stack">
    <h2>${product ? "Editar producto" : "Nuevo producto"}</h2>
    <input type="hidden" name="id" value="${esc(product?.id || "")}">
    <label>Nombre comercial<input name="name" required maxlength="120" value="${esc(product?.name || "")}" placeholder="Ej. Coca-Cola Original"></label>
    <div class="form-pair">
      <label>Marca<input name="brand" maxlength="80" value="${esc(product?.brand || "")}" placeholder="Ej. Coca-Cola"></label>
      <label>Presentación<input name="presentation" maxlength="60" value="${esc(product?.presentation || "")}" placeholder="Ej. 500 ml"></label>
    </div>
    <label>Descripción<textarea name="description" rows="3" required placeholder="Describe el producto brevemente">${esc(product?.description || "")}</textarea></label>
    <div class="form-pair">
      <label>Categoría<select name="category">${options(categories, product?.category)}</select></label>
      <label>Unidad<select name="unit">${options(units, product?.unit)}</select></label>
    </div>
    <div class="form-pair">
      <label>Precio (Bs)<input name="priceBob" type="number" step="0.01" min="0.01" required value="${esc(product?.priceBob || "")}"></label>
      <label>Existencias<input name="stock" type="number" min="0" required value="${esc(product?.stock ?? 1)}"></label>
    </div>
    <button class="button button--dark" type="submit">Guardar producto ↗</button>
    ${product ? '<button id="cancel-edit" class="link-button" type="button">Cancelar edición</button>' : ""}
  </form>`;
}
