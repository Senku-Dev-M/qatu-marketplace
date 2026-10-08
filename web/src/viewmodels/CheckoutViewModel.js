import { BaseViewModel } from "./BaseViewModel.js";
import { checkoutView } from "../views/checkoutView.js";

export class CheckoutViewModel extends BaseViewModel {
  constructor({ catalog, orders, auth, navigate, notifier }, id, query) {
    super();
    this.catalog = catalog;
    this.orders = orders;
    this.auth = auth;
    this.navigate = navigate;
    this.notifier = notifier;
    this.id = id;
    this.state = {
      loading: true,
      product: null,
      quantity: Math.max(1, Number(new URLSearchParams(query).get("qty")) || 1),
    };
  }
  async load() {
    if (!this.auth.requireLogin()) return;
    const result = await this.catalog.product(this.id);
    this.state.product = result.product;
    this.state.loading = false;
    this.render();
  }
  template() {
    return checkoutView(this.state);
  }
  bind() {
    for (const [button, paymentSucceeds] of [
      ["#pay-success", true],
      ["#pay-fail", false],
    ]) {
      this.el.querySelector(button).addEventListener("click", async () => {
        try {
          await this.orders.buy(this.id, this.state.quantity, paymentSucceeds);
          this.notifier(
            paymentSucceeds
              ? "Compra simulada confirmada."
              : "Pago simulado fallido; no se descontó stock.",
            paymentSucceeds ? "success" : "error",
          );
          this.navigate("/pedidos");
        } catch (error) {
          this.notifier(error.message, "error");
        }
      });
    }
  }
}
