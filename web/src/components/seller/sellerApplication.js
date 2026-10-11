export function sellerApplication() {
  return `<section class="wrap seller-apply">
    <div>
      <p class="eyebrow">PARA COMERCIANTES</p>
      <h1>Abre tu puesto en Qatu<span>.</span></h1>
      <p>Solicita ser vendedor y comparte tus productos con la comunidad. Un administrador revisará tu solicitud.</p>
    </div>
    <form id="seller-form" class="paper-card form-stack">
      <h2>Solicita tu espacio</h2>
      <label>Nombre de tu tienda<input name="shopName" required maxlength="80" placeholder="Ej. Katu de la Esquina"></label>
      <label>Cuéntanos qué ofreces<textarea name="description" rows="4" maxlength="500" placeholder="Bebidas, abarrotes, artículos del hogar..."></textarea></label>
      <button class="button button--dark" type="submit">Enviar solicitud ↗</button>
    </form>
  </section>`;
}
