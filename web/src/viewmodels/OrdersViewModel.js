import { BaseViewModel } from "./BaseViewModel.js";
import { ordersView } from "../views/ordersView.js";

export class OrdersViewModel extends BaseViewModel {
  constructor({ orders, auth, notifier, state }) {
    super();
    this.orders = orders;
    this.auth = auth;
    this.notifier = notifier;
    this.state = {
      loading: true,
      orders: [],
      currentUserId: state.profile?.id ?? null,
    };
  }
  async load() {
    if (!this.auth.requireLogin()) return;
    this.state.orders = await this.orders.list();
    this.state.loading = false;
    this.render();
  }
  template() {
    return ordersView(this.state);
  }
  bind() {
    this.el.querySelectorAll(".review-form").forEach((form) =>
      form.addEventListener("submit", async (event) => {
        event.preventDefault();
        const values = Object.fromEntries(new FormData(form));
        try {
          await this.orders.review(form.dataset.order, values);
          this.notifier("Gracias por compartir tu opinión.");
          form.reset();
        } catch (error) {
          this.notifier(error.message, "error");
        }
      }),
    );
  }
}
