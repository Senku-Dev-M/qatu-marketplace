export function bindProductActions({
  root,
  changeProductStatus,
  uploadProductImage,
  notifier,
  reload,
}) {
  root.querySelectorAll("[data-status]").forEach((button) =>
    button.addEventListener("click", async () => {
      try {
        await changeProductStatus.execute(
          button.dataset.status,
          button.dataset.next,
        );
        await reload();
      } catch (error) {
        notifier(error.message, "error");
      }
    }),
  );

  root.querySelectorAll("[data-upload]").forEach((input) =>
    input.addEventListener("change", async () => {
      if (!input.files[0]) return;
      try {
        await uploadProductImage.execute(input.dataset.upload, input.files[0]);
        notifier("Imagen subida.");
        await reload();
      } catch (error) {
        notifier(error.message, "error");
      }
    }),
  );
}
