export class AdminService {
  constructor(apiClient) {
    this.apiClient = apiClient;
  }

  async dashboard() {
    const [report, applications] = await Promise.all([
      this.apiClient.request("/admin/report"),
      this.apiClient.request("/seller-applications"),
    ]);
    return { report, applications };
  }

  decide(id, approve) {
    return this.apiClient.request(`/seller-applications/${id}/decision`, {
      method: "POST",
      body: { approve },
    });
  }

  downloadReport() {
    return this.apiClient.download("/admin/report.csv");
  }
}
