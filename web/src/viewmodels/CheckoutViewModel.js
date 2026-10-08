import { BaseViewModel } from "./BaseViewModel.js";
import { api } from "../api/client.js";
import { go } from "../utils/navigation.js";
import { requireLogin } from "../auth/session.js";
import { toast } from "../components/toast.js";
import { checkoutView } from "../views/checkoutView.js";

export class CheckoutViewModel extends BaseViewModel {
  constructor(id, query) {
    super();
    this.id = id;
    this.state = {
      loading: true,
      product: null,
      quantity: Math.max(1, Number(new URLSearchParams(query).get("qty")) || 1),
    };
  }
  async load() {
    if (!requireLogin()) return;
    const result = await api(`/products/${this.id}`);
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
          await api("/orders", {
            method: "POST",
            body: {
              productId: this.id,
              quantity: this.state.quantity,
              paymentSucceeds,
            },
          });
          toast(
            paymentSucceeds
              ? "Compra simulada confirmada."
              : "Pago simulado fallido; no se descontó stock.",
            paymentSucceeds ? "success" : "error",
          );
          go("/pedidos");
        } catch (error) {
          toast(error.message, "error");
        }
      });
    }
  }
}
