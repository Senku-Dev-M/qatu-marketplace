import { money } from "../../utils/format.js";
import { reviewForm } from "./reviewForm.js";

const statusLabel = {
  Paid: "Pagado",
  Failed: "Pago fallido",
  Pending: "Pendiente",
};

export function orderCard(order, currentUserId) {
  const canReview = order.status === "Paid" && order.buyerId === currentUserId;
  return `<article class="order-card">
    <div>
      <span class="badge badge--${order.status.toLowerCase()}">${statusLabel[order.status]}</span>
      <h3>Pedido ${order.id.slice(0, 8).toUpperCase()}</h3>
      <p>${order.quantity} unidad(es) · ${money(order.unitPriceBob * order.quantity)}</p>
      <small>${new Date(order.createdAt).toLocaleDateString("es-BO")}</small>
    </div>
    ${canReview ? reviewForm(order.id) : ""}
  </article>`;
}
