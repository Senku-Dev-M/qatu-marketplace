import { BaseViewModel } from "./BaseViewModel.js";
import { api } from "../api/client.js";
import { appState } from "../state/appState.js";
import { requireLogin } from "../auth/session.js";
import { toast } from "../components/toast.js";
import { sellerView } from "../views/sellerView.js";

export class SellerViewModel extends BaseViewModel {
  constructor() {
    super();
    this.state = { loading: true, shop: null, products: [], editing: null };
  }
  async load() {
    if (!requireLogin()) return;
    if (appState.profile?.isSeller) {
      this.state.shop = await api("/shops/mine");
      this.state.products = await api("/products/mine");
    }
    this.state.loading = false;
    this.render();
  }
  template() {
    return sellerView(this.state);
  }
  bind() {
    const sellerForm = this.el.querySelector("#seller-form");
    if (sellerForm)
      sellerForm.addEventListener("submit", async (event) => {
        event.preventDefault();
        const data = Object.fromEntries(new FormData(sellerForm));
        try {
          await api("/seller-applications", { method: "POST", body: data });
          toast("Solicitud enviada. Un administrador la revisará.");
          sellerForm.reset();
        } catch (error) {
          toast(error.message, "error");
        }
      });
    const productForm = this.el.querySelector("#product-form");
    if (!productForm) return;
    productForm.addEventListener("submit", async (event) => {
      event.preventDefault();
      const data = Object.fromEntries(new FormData(productForm));
      const id = data.id;
      delete data.id;
      data.priceBob = Number(data.priceBob);
      data.stock = Number(data.stock);
      try {
        await api(id ? `/products/${id}` : "/products", {
          method: id ? "PUT" : "POST",
          body: data,
        });
        toast("Producto guardado.");
        this.state.editing = null;
        await this.load();
      } catch (error) {
        toast(error.message, "error");
      }
    });
    this.el.querySelector("#cancel-edit")?.addEventListener("click", () => {
      this.state.editing = null;
      this.render();
    });
    this.el.querySelectorAll("[data-edit]").forEach((button) =>
      button.addEventListener("click", () => {
        this.state.editing = this.state.products.find(
          (product) => product.id === button.dataset.edit,
        );
        this.render();
        this.el
          .querySelector("#product-form")
          ?.scrollIntoView({ behavior: "smooth" });
      }),
    );
    this.el.querySelectorAll("[data-status]").forEach((button) =>
      button.addEventListener("click", async () => {
        try {
          await api(`/products/${button.dataset.status}/status`, {
            method: "POST",
            body: { status: button.dataset.next },
          });
          await this.load();
        } catch (error) {
          toast(error.message, "error");
        }
      }),
    );
    this.el.querySelectorAll("[data-upload]").forEach((input) =>
      input.addEventListener("change", async () => {
        if (!input.files[0]) return;
        const formData = new FormData();
        formData.set("file", input.files[0]);
        try {
          await api(`/products/${input.dataset.upload}/image`, {
            method: "POST",
            formData,
          });
          toast("Imagen subida.");
          await this.load();
        } catch (error) {
          toast(error.message, "error");
        }
      }),
    );
  }
}
