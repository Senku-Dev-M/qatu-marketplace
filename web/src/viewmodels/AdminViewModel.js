import { BaseViewModel } from "./BaseViewModel.js";
import { api } from "../api/client.js";
import { appState } from "../state/appState.js";
import { requireLogin } from "../auth/session.js";
import { toast } from "../components/toast.js";
import { adminView } from "../views/adminView.js";

export class AdminViewModel extends BaseViewModel {
  constructor() {
    super();
    this.state = { loading: true, report: null, applications: [] };
  }
  async load() {
    if (!requireLogin()) return;
    const [report, applications] = await Promise.all([
      api("/admin/report"),
      api("/seller-applications"),
    ]);
    Object.assign(this.state, { loading: false, report, applications });
    this.render();
  }
  template() {
    return adminView(this.state);
  }
  bind() {
    this.el.querySelectorAll("[data-decision]").forEach((button) =>
      button.addEventListener("click", async () => {
        try {
          await api(
            `/seller-applications/${button.dataset.decision}/decision`,
            {
              method: "POST",
              body: { approve: button.dataset.approve === "true" },
            },
          );
          toast("Solicitud resuelta.");
          await this.load();
        } catch (error) {
          toast(error.message, "error");
        }
      }),
    );
    this.el
      .querySelector("#download-csv")
      .addEventListener("click", async (event) => {
        event.preventDefault();
        try {
          const response = await fetch("/api/admin/report.csv", {
            headers: {
              Authorization: `Bearer ${appState.session.access_token}`,
            },
          });
          if (!response.ok) throw new Error("No se pudo descargar el reporte.");
          const url = URL.createObjectURL(await response.blob());
          const link = document.createElement("a");
          link.href = url;
          link.download = "qatu-ventas.csv";
          link.click();
          URL.revokeObjectURL(url);
        } catch (error) {
          toast(error.message, "error");
        }
      });
  }
}
