import { adminStats } from "../components/admin/adminStats.js";
import { sellerApplications } from "../components/admin/sellerApplications.js";

export function adminView(state) {
  const report = state.report;
  return `<section class="wrap inner-section">
    <p class="eyebrow">PANEL INTERNO</p>
    <h1>Administración<span class="period">.</span></h1>
    <p class="section-lead">Una vista clara del mercado y las solicitudes pendientes.</p>
    ${adminStats(report)}
    <a class="outline-button csv-link" href="/api/admin/report.csv" id="download-csv">Descargar CSV ↗</a>
    ${sellerApplications(state.applications)}
  </section>`;
}
