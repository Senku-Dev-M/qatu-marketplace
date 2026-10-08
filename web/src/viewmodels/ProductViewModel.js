import { BaseViewModel } from "./BaseViewModel.js";
import { detailView } from "../views/detailView.js";

export class ProductViewModel extends BaseViewModel {
  constructor({ catalog, conversations, auth, navigate, notifier }, id) {
    super();
    this.catalog = catalog;
    this.conversations = conversations;
    this.auth = auth;
    this.navigate = navigate;
    this.notifier = notifier;
    this.id = id;
    this.state = { loading: true, product: null, shopName: "", reviews: [] };
  }
  async load() {
    Object.assign(this.state, await this.catalog.product(this.id), {
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
      if (!this.auth.requireLogin()) return;
      const quantity = Number(new FormData(event.target).get("quantity"));
      this.navigate(`/pago/${this.id}?qty=${quantity}`);
    });
    this.el.querySelector("#open-chat").addEventListener("click", async () => {
      if (!this.auth.requireLogin()) return;
      try {
        const conversation = await this.conversations.open(this.id);
        this.navigate(`/mensajes/${conversation.id}`);
      } catch (error) {
        this.notifier(error.message, "error");
      }
    });
  }
}
