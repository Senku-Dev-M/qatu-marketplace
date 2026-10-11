export function bindSellerApplication({ root, applyAsSeller, notifier }) {
  const form = root.querySelector("#seller-form");
  if (!form) return;

  form.addEventListener("submit", async (event) => {
    event.preventDefault();
    try {
      await applyAsSeller.execute(Object.fromEntries(new FormData(form)));
      notifier("Solicitud enviada. Un administrador la revisará.");
      form.reset();
    } catch (error) {
      notifier(error.message, "error");
    }
  });
}
