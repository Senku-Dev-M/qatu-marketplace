import { BaseViewModel } from "./BaseViewModel.js";
import { ordersView } from "../views/ordersView.js";

export class OrdersViewModel extends BaseViewModel {
  constructor({ getOrders, createReview, sessionGuard, notifier, state }) {
    super();
    this.getOrders = getOrders;
    this.createReview = createReview;
    this.sessionGuard = sessionGuard;
    this.notifier = notifier;
    this.state = {
      loading: true,
      orders: [],
      currentUserId: state.profile?.id ?? null,
    };
  }
  async load() {
    if (!this.sessionGuard.requireLogin()) return;
    this.state.orders = await this.getOrders.execute();
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
          await this.createReview.execute(form.dataset.order, values);
          this.notifier("Gracias por compartir tu opinión.");
          form.reset();
        } catch (error) {
          this.notifier(error.message, "error");
        }
      }),
    );
  }
}
