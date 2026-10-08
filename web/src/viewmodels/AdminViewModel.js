import { BaseViewModel } from "./BaseViewModel.js";
import { adminView } from "../views/adminView.js";

export class AdminViewModel extends BaseViewModel {
  constructor({ admin, auth, notifier }) {
    super();
    this.admin = admin;
    this.auth = auth;
    this.notifier = notifier;
    this.state = { loading: true, report: null, applications: [] };
  }
  async load() {
    if (!this.auth.requireLogin()) return;
    Object.assign(this.state, await this.admin.dashboard(), { loading: false });
    this.render();
  }
  template() {
    return adminView(this.state);
  }
  bind() {
    this.el.querySelectorAll("[data-decision]").forEach((button) =>
      button.addEventListener("click", async () => {
        try {
          await this.admin.decide(
            button.dataset.decision,
            button.dataset.approve === "true",
          );
          this.notifier("Solicitud resuelta.");
          await this.load();
        } catch (error) {
          this.notifier(error.message, "error");
        }
      }),
    );
    this.el
      .querySelector("#download-csv")
      .addEventListener("click", async (event) => {
        event.preventDefault();
        try {
          const url = URL.createObjectURL(await this.admin.downloadReport());
          const link = document.createElement("a");
          link.href = url;
          link.download = "qatu-ventas.csv";
          link.click();
          URL.revokeObjectURL(url);
        } catch (error) {
          this.notifier(error.message, "error");
        }
      });
  }
}
