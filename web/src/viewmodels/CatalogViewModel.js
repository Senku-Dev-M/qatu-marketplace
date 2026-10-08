import { BaseViewModel } from "./BaseViewModel.js";
import { api } from "../api/client.js";
import { toast } from "../components/toast.js";
import { catalogView } from "../views/catalogView.js";

export class CatalogViewModel extends BaseViewModel {
  constructor() {
    super();
    this.state = { loading: true, products: [], query: "", category: "Todo" };
  }
  async load() {
    await this.fetchProducts();
  }
  async fetchProducts() {
    const params = new URLSearchParams();
    if (this.state.query) params.set("q", this.state.query);
    if (this.state.category !== "Todo")
      params.set("category", this.state.category);
    this.state.products = await api(`/catalog?${params}`);
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
          toast(error.message, "error");
        }
      });
    this.el.querySelectorAll("[data-category]").forEach((button) =>
      button.addEventListener("click", async () => {
        this.state.category = button.dataset.category;
        try {
          await this.fetchProducts();
          focusCatalog();
        } catch (error) {
          toast(error.message, "error");
        }
      }),
    );
  }
}
