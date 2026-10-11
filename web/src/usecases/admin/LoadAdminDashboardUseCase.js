export class LoadAdminDashboardUseCase {
  constructor(apiClient) {
    this.apiClient = apiClient;
  }

  async execute() {
    const [report, applications] = await Promise.all([
      this.apiClient.request("/admin/report"),
      this.apiClient.request("/seller-applications"),
    ]);
    return { report, applications };
  }
}
