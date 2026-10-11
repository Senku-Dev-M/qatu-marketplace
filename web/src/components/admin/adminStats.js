import { money } from "../../utils/format.js";

export function adminStats(report) {
  return `<div class="stats-grid">
    <div><span>Usuarios</span><strong>${report.users}</strong></div>
    <div><span>Vendedores</span><strong>${report.sellers}</strong></div>
    <div><span>Productos</span><strong>${report.products}</strong></div>
    <div><span>Pedidos pagados</span><strong>${report.paidOrders}</strong></div>
    <div class="stat-wide"><span>Ventas simuladas</span><strong>${money(report.salesBob)}</strong></div>
  </div>`;
}
