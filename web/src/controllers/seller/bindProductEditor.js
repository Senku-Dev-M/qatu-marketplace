function productData(form) {
  const values = Object.fromEntries(new FormData(form));
  const id = values.id;
  delete values.id;
  return {
    id,
    values: {
      ...values,
      priceBob: Number(values.priceBob),
      stock: Number(values.stock),
    },
  };
}

export function bindProductEditor({
  root,
  state,
  createProduct,
  updateProduct,
  notifier,
  render,
  reload,
}) {
  const form = root.querySelector("#product-form");
  if (!form) return;

  form.addEventListener("submit", async (event) => {
    event.preventDefault();
    const { id, values } = productData(form);
    try {
      if (id) await updateProduct.execute(id, values);
      else await createProduct.execute(values);
      notifier("Producto guardado.");
      state.editing = null;
      await reload();
    } catch (error) {
      notifier(error.message, "error");
    }
  });

  root.querySelector("#cancel-edit")?.addEventListener("click", () => {
    state.editing = null;
    render();
  });

  root.querySelectorAll("[data-edit]").forEach((button) =>
    button.addEventListener("click", () => {
      state.editing = state.products.find(
        (product) => product.id === button.dataset.edit,
      );
      render();
      root
        .querySelector("#product-form")
        ?.scrollIntoView({ behavior: "smooth" });
    }),
  );
}
