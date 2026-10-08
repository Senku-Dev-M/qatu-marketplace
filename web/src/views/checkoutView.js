import { esc, money } from "../utils/format.js";

export function checkoutView(state) {
  return `<section class="wrap narrow-section"><div class="breadcrumbs"><a href="#/producto/${state.product.id}">Producto</a><span>/</span><strong>Pago simulado</strong></div><p class="eyebrow">PASO FINAL</p><h1>Confirma tu compra<span class="period">.</span></h1><div class="checkout-card"><div><strong>${esc(state.product.name)}</strong><p>${state.quantity} × ${money(state.product.priceBob)}</p></div><strong>${money(state.quantity * state.product.priceBob)}</strong></div><div class="simulation-note"><span>ⓘ</span><p>Esta es una simulación académica. No se cobrarán fondos ni se solicitarán datos de tarjeta.</p></div><div class="checkout-actions"><button id="pay-success" class="button button--dark">Simular pago aprobado ↗</button><button id="pay-fail" class="outline-button">Simular pago fallido</button></div></section>`;
}
