export function reviewForm(orderId) {
  return `<form class="review-form" data-order="${orderId}">
    <label>Calificar<select name="kind"><option value="Product">Producto</option><option value="Seller">Vendedor</option></select></label>
    <label>Estrellas<select name="stars">${[5, 4, 3, 2, 1].map((stars) => `<option value="${stars}">${stars} ★</option>`).join("")}</select></label>
    <label class="grow">Comentario<input name="comment" maxlength="500" placeholder="¿Cómo fue tu experiencia?"></label>
    <button class="button button--small" type="submit">Enviar reseña</button>
  </form>`;
}
