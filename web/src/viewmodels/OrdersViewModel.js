import { BaseViewModel } from "./BaseViewModel.js";
import { api } from "../api/client.js";
import { requireLogin } from "../auth/session.js";
import { toast } from "../components/toast.js";
import { ordersView } from "../views/ordersView.js";

export class OrdersViewModel extends BaseViewModel {
  constructor() {
    super();
    this.state = { loading: true, orders: [] };
  }
  async load() {
    if (!requireLogin()) return;
    this.state.orders = await api("/orders");
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
          await api("/reviews", {
            method: "POST",
            body: {
              orderId: form.dataset.order,
              kind: values.kind,
              stars: Number(values.stars),
              comment: values.comment,
            },
          });
          toast("Gracias por compartir tu opinión.");
          form.reset();
        } catch (error) {
          toast(error.message, "error");
        }
      }),
    );
  }
}
