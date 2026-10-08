import { BaseViewModel } from "./BaseViewModel.js";
import { sellerView } from "../views/sellerView.js";

export class SellerViewModel extends BaseViewModel {
  constructor({ seller, state, auth, notifier }) {
    super();
    this.seller = seller;
    this.auth = auth;
    this.notifier = notifier;
    this.state = {
      loading: true,
      shop: null,
      products: [],
      editing: null,
      profile: state.profile,
    };
  }
  async load() {
    if (!this.auth.requireLogin()) return;
    if (this.state.profile?.isSeller) {
      Object.assign(this.state, await this.seller.workspace());
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
          await this.seller.apply(data);
          this.notifier("Solicitud enviada. Un administrador la revisará.");
          sellerForm.reset();
        } catch (error) {
          this.notifier(error.message, "error");
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
        await this.seller.save(id, data);
        this.notifier("Producto guardado.");
        this.state.editing = null;
        await this.load();
      } catch (error) {
        this.notifier(error.message, "error");
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
          await this.seller.setStatus(
            button.dataset.status,
            button.dataset.next,
          );
          await this.load();
        } catch (error) {
          this.notifier(error.message, "error");
        }
      }),
    );
    this.el.querySelectorAll("[data-upload]").forEach((input) =>
      input.addEventListener("change", async () => {
        if (!input.files[0]) return;
        try {
          await this.seller.uploadImage(input.dataset.upload, input.files[0]);
          this.notifier("Imagen subida.");
          await this.load();
        } catch (error) {
          this.notifier(error.message, "error");
        }
      }),
    );
  }
}
