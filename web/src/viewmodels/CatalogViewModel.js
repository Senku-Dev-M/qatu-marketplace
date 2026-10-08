import { BaseViewModel } from "./BaseViewModel.js";
import { catalogView } from "../views/catalogView.js";

export class CatalogViewModel extends BaseViewModel {
  constructor({ catalog, notifier }) {
    super();
    this.catalog = catalog;
    this.notifier = notifier;
    this.state = { loading: true, products: [], query: "", category: "Todo" };
  }
  async load() {
    await this.fetchProducts();
  }
  async fetchProducts() {
    this.state.products = await this.catalog.search(
      this.state.query,
      this.state.category,
    );
    this.state.loading = false;
    this.render();
  }
  template() {
    return catalogView(this.state);
  }
  bind() {
    const focusCatalog = () =>
      this.el
        .querySelector("#productos")
        ?.scrollIntoView({ behavior: "smooth" });
    this.el
      .querySelector(".hero-copy .button")
      ?.addEventListener("click", (event) => {
        event.preventDefault();
        focusCatalog();
      });
    this.el
      .querySelector("#search-form")
      .addEventListener("submit", async (event) => {
        event.preventDefault();
        this.state.query = new FormData(event.target)
          .get("q")
          .toString()
          .trim();
        try {
          await this.fetchProducts();
          focusCatalog();
        } catch (error) {
          this.notifier(error.message, "error");
        }
      });
    this.el.querySelectorAll("[data-category]").forEach((button) =>
      button.addEventListener("click", async () => {
        this.state.category = button.dataset.category;
        try {
          await this.fetchProducts();
          focusCatalog();
        } catch (error) {
          this.notifier(error.message, "error");
        }
      }),
    );
  }
}
