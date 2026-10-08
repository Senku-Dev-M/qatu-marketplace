import { errorView } from "../components/errorView.js";
import { loading } from "../components/loading.js";

export class BaseViewModel {
  constructor() {
    this.state = { loading: true, error: "" };
    this.el = null;
  }
  mount(el) {
    this.el = el;
    this.render();
    this.load().catch((error) => this.fail(error));
  }
  destroy() {}
  fail(error) {
    this.state.error = error.message || String(error);
    this.state.loading = false;
    this.render();
  }
  async load() {
    this.state.loading = false;
    this.render();
  }
  template() {
    return "";
  }
  bind() {}
  render() {
    if (!this.el) return;
    this.el.innerHTML = this.state.error
      ? errorView(this.state.error)
      : this.state.loading
        ? loading()
        : this.template();
    if (!this.state.loading && !this.state.error) this.bind();
  }
}
