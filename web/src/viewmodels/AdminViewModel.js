import { BaseViewModel } from "./BaseViewModel.js";
import { adminView } from "../views/adminView.js";

export class AdminViewModel extends BaseViewModel {
  constructor({
    loadAdminDashboard,
    decideSellerApplication,
    downloadAdminReport,
    sessionGuard,
    notifier,
  }) {
    super();
    this.loadAdminDashboard = loadAdminDashboard;
    this.decideSellerApplication = decideSellerApplication;
    this.downloadAdminReport = downloadAdminReport;
    this.sessionGuard = sessionGuard;
    this.notifier = notifier;
    this.state = { loading: true, report: null, applications: [] };
  }
  async load() {
    if (!this.sessionGuard.requireLogin()) return;
    Object.assign(this.state, await this.loadAdminDashboard.execute(), {
      loading: false,
    });
    this.render();
  }
  template() {
    return adminView(this.state);
  }
  bind() {
    this.el.querySelectorAll("[data-decision]").forEach((button) =>
      button.addEventListener("click", async () => {
        try {
          await this.decideSellerApplication.execute(
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
          const url = URL.createObjectURL(
            await this.downloadAdminReport.execute(),
          );
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
