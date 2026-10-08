import { BaseViewModel } from "./BaseViewModel.js";
import { api } from "../api/client.js";
import { go } from "../utils/navigation.js";
import { requireLogin } from "../auth/session.js";
import { toast } from "../components/toast.js";
import { detailView } from "../views/detailView.js";

export class ProductViewModel extends BaseViewModel {
  constructor(id) {
    super();
    this.id = id;
    this.state = { loading: true, product: null, shopName: "", reviews: [] };
  }
  async load() {
    Object.assign(this.state, await api(`/products/${this.id}`), {
      loading: false,
    });
    this.render();
  }
  template() {
    return detailView(this.state);
  }
  bind() {
    this.el.querySelector("#buy-form")?.addEventListener("submit", (event) => {
      event.preventDefault();
      if (!requireLogin()) return;
      const quantity = Number(new FormData(event.target).get("quantity"));
      go(`/pago/${this.id}?qty=${quantity}`);
    });
    this.el.querySelector("#open-chat").addEventListener("click", async () => {
      if (!requireLogin()) return;
      try {
        const conversation = await api("/conversations", {
          method: "POST",
          body: { productId: this.id },
        });
        go(`/mensajes/${conversation.id}`);
      } catch (error) {
        toast(error.message, "error");
      }
    });
  }
}
