import { esc } from "../../utils/format.js";

const statusLabel = {
  Pending: "Pendiente",
  Approved: "Aprobada",
  Rejected: "Rechazada",
};

export function sellerApplicationCard(application) {
  const actions =
    application.status === "Pending"
      ? `<div class="request-actions">
          <button data-decision="${application.id}" data-approve="true" class="button button--small">Aprobar</button>
          <button data-decision="${application.id}" data-approve="false" class="outline-button">Rechazar</button>
        </div>`
      : "";

  return `<article class="request-card">
    <div>
      <span class="badge badge--${application.status.toLowerCase()}">${statusLabel[application.status]}</span>
      <h3>${esc(application.shopName)}</h3>
      <p>${esc(application.description)}</p>
      <small>Usuario ${application.applicantId.slice(0, 8)}</small>
    </div>
    ${actions}
  </article>`;
}
