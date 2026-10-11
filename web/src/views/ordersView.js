import { empty } from "../components/empty.js";
import { orderCard } from "../components/orders/orderCard.js";

export function ordersView(state) {
  const content = state.orders.length
    ? `<div class="order-list">${state.orders
        .map((order) => orderCard(order, state.currentUserId))
        .join("")}</div>`
    : empty(
        "Aún no tienes pedidos",
        "Explora el mercado y encuentra lo que necesitas.",
        '<a class="button" href="#/">Ver productos ↗</a>',
      );

  return `<section class="wrap inner-section">
    <p class="eyebrow">MI CUENTA</p>
    <h1>Mis pedidos<span class="period">.</span></h1>
    <p class="section-lead">Tus compras y ventas quedan registradas aquí.</p>
    ${content}
  </section>`;
}
