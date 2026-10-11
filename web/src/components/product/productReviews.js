import { esc } from "../../utils/format.js";

function reviewCard(review) {
  return `<article class="review">
    <strong>${"★".repeat(review.stars)}${"☆".repeat(5 - review.stars)}</strong>
    <p>${esc(review.comment || "Buena experiencia de compra.")}</p>
  </article>`;
}

export function productReviews(reviews) {
  const content = reviews.length
    ? `<div class="review-list">${reviews.map(reviewCard).join("")}</div>`
    : '<p class="muted">Aún no hay reseñas. Las opiniones aparecen tras una compra confirmada.</p>';

  return `<div class="detail-lower">
    <div><p class="eyebrow">LA COMUNIDAD OPINA</p><h2>Reseñas del producto</h2></div>
    ${content}
  </div>`;
}
