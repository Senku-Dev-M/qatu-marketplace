import { AdminViewModel } from "../viewmodels/AdminViewModel.js";
import { AuthViewModel } from "../viewmodels/AuthViewModel.js";
import { CatalogViewModel } from "../viewmodels/CatalogViewModel.js";
import { CheckoutViewModel } from "../viewmodels/CheckoutViewModel.js";
import { InboxViewModel } from "../viewmodels/InboxViewModel.js";
import { OrdersViewModel } from "../viewmodels/OrdersViewModel.js";
import { PolicyViewModel } from "../viewmodels/PolicyViewModel.js";
import { ProductViewModel } from "../viewmodels/ProductViewModel.js";
import { SellerViewModel } from "../viewmodels/SellerViewModel.js";

export class ViewModelFactory {
  constructor(application) {
    this.application = application;
  }

  create(path) {
    const clean = path.split("?")[0];
    if (clean === "/") return new CatalogViewModel(this.application);
    if (clean === "/acceso") return new AuthViewModel(this.application);
    if (clean === "/privacidad") return new PolicyViewModel("privacy");
    if (clean === "/condiciones") return new PolicyViewModel("terms");
    if (clean === "/pedidos") return new OrdersViewModel(this.application);
    if (clean === "/vender") return new SellerViewModel(this.application);
    if (clean === "/admin") return new AdminViewModel(this.application);
    if (clean === "/mensajes")
      return new InboxViewModel(this.application, null);
    if (clean.startsWith("/mensajes/"))
      return new InboxViewModel(this.application, clean.split("/")[2]);
    if (clean.startsWith("/producto/"))
      return new ProductViewModel(this.application, clean.split("/")[2]);
    if (clean.startsWith("/pago/"))
      return new CheckoutViewModel(
        this.application,
        clean.split("/")[2],
        path.split("?")[1] || "",
      );
    return new CatalogViewModel(this.application);
  }
}
