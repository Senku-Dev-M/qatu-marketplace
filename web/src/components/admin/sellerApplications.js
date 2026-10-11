import { empty } from "../empty.js";
import { sellerApplicationCard } from "./sellerApplicationCard.js";

export function sellerApplications(applications) {
  const content = applications.length
    ? applications.map(sellerApplicationCard).join("")
    : empty(
        "No hay solicitudes todavía",
        "Las solicitudes de vendedores aparecerán aquí.",
      );

  return `<div class="admin-requests">
    <div class="heading-row">
      <div><p class="eyebrow">GESTIÓN DE VENDEDORES</p><h2>Solicitudes</h2></div>
      <span>${applications.length} en total</span>
    </div>
    ${content}
  </div>`;
}
